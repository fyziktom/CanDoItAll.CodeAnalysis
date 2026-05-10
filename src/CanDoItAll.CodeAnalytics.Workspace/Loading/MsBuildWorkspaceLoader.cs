using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Identifiers;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Workspace.Inventory;
using CanDoItAll.CodeAnalytics.Workspace.Normalization;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CanDoItAll.CodeAnalytics.Workspace.Loading;

public sealed class MsBuildWorkspaceLoader {
    private static readonly object RegistrationLock = new();
    private readonly AnalysisRequestNormalizer _requestNormalizer;
    private readonly ProjectFileInventoryReader _projectFileInventoryReader;
    private readonly ILogger<MsBuildWorkspaceLoader> _logger;

    public MsBuildWorkspaceLoader(
        AnalysisRequestNormalizer requestNormalizer,
        ProjectFileInventoryReader projectFileInventoryReader,
        ILogger<MsBuildWorkspaceLoader>? logger = null) {
        _requestNormalizer = requestNormalizer;
        _projectFileInventoryReader = projectFileInventoryReader;
        _logger = logger ?? NullLogger<MsBuildWorkspaceLoader>.Instance;
    }

    public async Task<WorkspaceLoadResult> LoadAsync(
        AnalysisRequest request,
        CancellationToken cancellationToken = default) {
        var normalizedRequest = _requestNormalizer.Normalize(request);
        if (!File.Exists(normalizedRequest.SolutionPath)) {
            _logger.LogWarning("Workspace path does not exist: {WorkspacePath}", normalizedRequest.SolutionPath);

            var diagnostics = new[] {
                new AnalysisDiagnostic(
                    "WS0001",
                    AnalysisDiagnosticSeverity.Error,
                    $"Workspace path does not exist: {normalizedRequest.SolutionPath}"),
            };

            return new WorkspaceLoadResult(normalizedRequest, null, [], [], [], diagnostics, null, null);
        }

        EnsureMsBuildRegistered();

        var workspaceDiagnostics = new List<AnalysisDiagnostic>();
        var workspace = MSBuildWorkspace.Create();
        workspace.RegisterWorkspaceFailedHandler(
            eventArgs => {
                var severity = eventArgs.Diagnostic.Kind == WorkspaceDiagnosticKind.Failure
                    ? AnalysisDiagnosticSeverity.Warning
                    : AnalysisDiagnosticSeverity.Info;

                workspaceDiagnostics.Add(
                    new AnalysisDiagnostic(
                        "WS0002",
                        severity,
                        eventArgs.Diagnostic.Message));
                _logger.LogWarning("MSBuild workspace diagnostic {Kind}: {Message}", eventArgs.Diagnostic.Kind, eventArgs.Diagnostic.Message);
            },
            null);

        try {
            var loadMode = GetLoadMode(normalizedRequest.SolutionPath);
            var openResult = await OpenWorkspaceAsync(workspace, normalizedRequest.SolutionPath, loadMode, cancellationToken);
            var effectiveRequest = CreateEffectiveRequest(normalizedRequest, loadMode, openResult.ProjectName);
            var solutionDirectory = Path.GetDirectoryName(effectiveRequest.SolutionPath)!;
            var sourceProjects = openResult.Solution.Projects
                .Where(project => string.Equals(project.Language, LanguageNames.CSharp, StringComparison.Ordinal))
                .Where(project => !string.IsNullOrWhiteSpace(project.FilePath))
                .GroupBy(project => Path.GetFullPath(project.FilePath!), StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .OrderBy(project => project.Name, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var inventories = sourceProjects
                .Select(
                    project => new {
                        Project = project,
                        Inventory = _projectFileInventoryReader.Read(project.FilePath!),
                    })
                .ToArray();

            var projectIdsByPath = inventories.ToDictionary(
                item => Path.GetFullPath(item.Project.FilePath!),
                item => StableId.ForProject(item.Project.Name),
                StringComparer.OrdinalIgnoreCase);

            var allProjectFacts = inventories
                .Select(item => CreateProjectFact(item.Project, item.Inventory, projectIdsByPath, solutionDirectory))
                .OrderBy(project => project.Name, StringComparer.OrdinalIgnoreCase)
                .ToArray();
            var includedProjectIds = allProjectFacts
                .Where(project => ShouldIncludeProject(effectiveRequest, project))
                .Select(project => project.ProjectId)
                .ToHashSet(StringComparer.Ordinal);

            if (includedProjectIds.Count == 0) {
                workspaceDiagnostics.Add(
                    new AnalysisDiagnostic(
                        "WS0004",
                        AnalysisDiagnosticSeverity.Error,
                        $"No source projects matched the current scope for {effectiveRequest.SolutionPath}."));
                _logger.LogWarning("No source projects matched the current scope for {WorkspacePath}", effectiveRequest.SolutionPath);
            }

            var projectFacts = allProjectFacts
                .Where(project => includedProjectIds.Contains(project.ProjectId))
                .Select(
                    project => project with {
                        ProjectReferences = project.ProjectReferences
                            .Where(includedProjectIds.Contains)
                            .OrderBy(value => value, StringComparer.Ordinal)
                            .ToArray(),
                    })
                .OrderBy(project => project.Name, StringComparer.OrdinalIgnoreCase)
                .ToArray();
            var documentContexts = inventories
                .SelectMany(
                    item => item.Project.Documents
                        .Where(document => document.SourceCodeKind == SourceCodeKind.Regular)
                        .Where(document => !string.IsNullOrWhiteSpace(document.FilePath))
                        .Select(document => CreateDocumentContext(item.Project, document, solutionDirectory)))
                .Where(document => includedProjectIds.Contains(document.Fact.ProjectId))
                .OrderBy(document => document.Fact.Path, StringComparer.OrdinalIgnoreCase)
                .ToArray();
            var projectContexts = sourceProjects
                .Where(project => includedProjectIds.Contains(projectIdsByPath[Path.GetFullPath(project.FilePath!)]))
                .Select(
                    project => {
                        var projectId = projectIdsByPath[Path.GetFullPath(project.FilePath!)];
                        var projectFact = projectFacts.Single(fact => string.Equals(fact.ProjectId, projectId, StringComparison.Ordinal));
                        var documents = documentContexts
                            .Where(document => string.Equals(document.Fact.ProjectId, projectId, StringComparison.Ordinal))
                            .ToArray();

                        return new WorkspaceProjectContext(project, projectFact, documents);
                    })
                .ToArray();

            var workspaceName = loadMode == WorkspaceLoadMode.Project && !string.IsNullOrWhiteSpace(openResult.ProjectName)
                ? openResult.ProjectName
                : Path.GetFileNameWithoutExtension(effectiveRequest.SolutionPath);
            var solutionFact = new SolutionFact(
                workspaceName,
                effectiveRequest.SolutionPath,
                projectFacts.Length,
                documentContexts.Length);

            _logger.LogInformation(
                "Loaded workspace {WorkspacePath} as {LoadMode} with {ProjectCount} source projects and {DocumentCount} source documents.",
                effectiveRequest.SolutionPath,
                loadMode,
                projectFacts.Length,
                documentContexts.Length);

            return new WorkspaceLoadResult(
                effectiveRequest,
                solutionFact,
                projectFacts,
                documentContexts.Select(context => context.Fact).ToArray(),
                projectContexts,
                workspaceDiagnostics.OrderBy(diagnostic => diagnostic.Code, StringComparer.Ordinal).ToArray(),
                openResult.Solution,
                workspace);
        }
        catch (Exception exception) {
            workspace.Dispose();
            _logger.LogError(exception, "Loading workspace {WorkspacePath} failed.", normalizedRequest.SolutionPath);

            var diagnostics = workspaceDiagnostics
                .Append(
                    new AnalysisDiagnostic(
                        "WS0003",
                        AnalysisDiagnosticSeverity.Error,
                        exception.Message))
                .OrderBy(diagnostic => diagnostic.Code, StringComparer.Ordinal)
                .ToArray();

            return new WorkspaceLoadResult(normalizedRequest, null, [], [], [], diagnostics, null, null);
        }
    }

    private static void EnsureMsBuildRegistered() {
        if (MSBuildLocator.IsRegistered) {
            return;
        }

        lock (RegistrationLock) {
            if (MSBuildLocator.IsRegistered) {
                return;
            }

            MSBuildLocator.RegisterDefaults();
        }
    }

    private static WorkspaceLoadMode GetLoadMode(string workspacePath) {
        return string.Equals(Path.GetExtension(workspacePath), ".csproj", StringComparison.OrdinalIgnoreCase)
            ? WorkspaceLoadMode.Project
            : WorkspaceLoadMode.Solution;
    }

    private static async Task<WorkspaceOpenResult> OpenWorkspaceAsync(
        MSBuildWorkspace workspace,
        string workspacePath,
        WorkspaceLoadMode loadMode,
        CancellationToken cancellationToken) {
        if (loadMode == WorkspaceLoadMode.Project) {
            var project = await workspace.OpenProjectAsync(workspacePath, cancellationToken: cancellationToken);
            return new WorkspaceOpenResult(project.Solution, project.Name);
        }

        return new WorkspaceOpenResult(
            await workspace.OpenSolutionAsync(workspacePath, cancellationToken: cancellationToken),
            null);
    }

    private static AnalysisRequest CreateEffectiveRequest(
        AnalysisRequest request,
        WorkspaceLoadMode loadMode,
        string? projectName) {
        if (loadMode != WorkspaceLoadMode.Project || request.ScopeProjectNames.Count > 0 || string.IsNullOrWhiteSpace(projectName)) {
            return request;
        }

        return request with {
            ScopeProjectNames = new[] { projectName },
        };
    }

    private static bool ShouldIncludeProject(AnalysisRequest request, ProjectFact project) {
        if (request.ScopeProjectNames.Count == 0) {
            return true;
        }

        return request.ScopeProjectNames.Contains(project.Name, StringComparer.OrdinalIgnoreCase);
    }

    private static WorkspaceDocumentContext CreateDocumentContext(Project project, Document document, string solutionDirectory) {
        var path = document.FilePath!;
        var lineCount = File.Exists(path)
            ? File.ReadLines(path).Count()
            : 0;

        var relativePath = Path.GetRelativePath(solutionDirectory, path).Replace('\\', '/');
        var fact = new DocumentFact(
            StableId.ForDocument(relativePath),
            StableId.ForProject(project.Name),
            relativePath,
            document.Name,
            lineCount);

        return new WorkspaceDocumentContext(document, fact);
    }

    private static ProjectFact CreateProjectFact(
        Project project,
        ProjectFileInventory inventory,
        IReadOnlyDictionary<string, string> projectIdsByPath,
        string solutionDirectory) {
        var projectPath = Path.GetFullPath(project.FilePath!);
        var projectReferenceIds = inventory.ProjectReferencePaths
            .Where(projectIdsByPath.ContainsKey)
            .Select(path => projectIdsByPath[path])
            .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var documentCount = project.Documents.Count(document => document.SourceCodeKind == SourceCodeKind.Regular);

        return new ProjectFact(
            projectIdsByPath[projectPath],
            project.Name,
            Path.GetRelativePath(solutionDirectory, projectPath).Replace('\\', '/'),
            inventory.TargetFrameworks,
            projectReferenceIds,
            inventory.PackageReferences,
            documentCount);
    }

    private enum WorkspaceLoadMode {
        Solution,
        Project,
    }

    private sealed record WorkspaceOpenResult(
        Solution Solution,
        string? ProjectName);
}
