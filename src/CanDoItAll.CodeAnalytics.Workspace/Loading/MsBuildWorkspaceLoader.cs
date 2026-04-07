using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Identifiers;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Domain.Sources;
using CanDoItAll.CodeAnalytics.Workspace.Inventory;
using CanDoItAll.CodeAnalytics.Workspace.Normalization;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace CanDoItAll.CodeAnalytics.Workspace.Loading;

public sealed class MsBuildWorkspaceLoader {
    private static readonly object RegistrationLock = new();
    private readonly AnalysisRequestNormalizer _requestNormalizer;
    private readonly ProjectFileInventoryReader _projectFileInventoryReader;

    public MsBuildWorkspaceLoader(
        AnalysisRequestNormalizer requestNormalizer,
        ProjectFileInventoryReader projectFileInventoryReader) {
        _requestNormalizer = requestNormalizer;
        _projectFileInventoryReader = projectFileInventoryReader;
    }

    public async Task<WorkspaceLoadResult> LoadAsync(
        AnalysisRequest request,
        CancellationToken cancellationToken = default) {
        var normalizedRequest = _requestNormalizer.Normalize(request);
        if (!File.Exists(normalizedRequest.SolutionPath)) {
            var diagnostics = new[]
            {
                new AnalysisDiagnostic(
                    "WS0001",
                    AnalysisDiagnosticSeverity.Error,
                    $"Solution path does not exist: {normalizedRequest.SolutionPath}"),
            };

            return new WorkspaceLoadResult(normalizedRequest, null, [], [], [], diagnostics, null, null);
        }

        EnsureMsBuildRegistered();

        var workspaceDiagnostics = new List<AnalysisDiagnostic>();
        var workspace = MSBuildWorkspace.Create();
        workspace.RegisterWorkspaceFailedHandler(eventArgs => {
            var severity = eventArgs.Diagnostic.Kind == WorkspaceDiagnosticKind.Failure
                ? AnalysisDiagnosticSeverity.Warning
                : AnalysisDiagnosticSeverity.Info;

            workspaceDiagnostics.Add(
                new AnalysisDiagnostic(
                    "WS0002",
                    severity,
                    eventArgs.Diagnostic.Message));
        },
        null);

        try {
            var solution = await workspace.OpenSolutionAsync(normalizedRequest.SolutionPath, cancellationToken: cancellationToken);
            var solutionDirectory = Path.GetDirectoryName(normalizedRequest.SolutionPath)!;
            var sourceProjects = solution.Projects
                .Where(project => string.Equals(project.Language, LanguageNames.CSharp, StringComparison.Ordinal))
                .Where(project => !string.IsNullOrWhiteSpace(project.FilePath))
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

            var projectFacts = inventories
                .Select(
                    item => CreateProjectFact(
                        item.Project,
                        item.Inventory,
                        projectIdsByPath,
                        solutionDirectory))
                .OrderBy(project => project.Name, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var documentContexts = inventories
                .SelectMany(
                    item => item.Project.Documents
                        .Where(document => document.SourceCodeKind == SourceCodeKind.Regular)
                        .Where(document => !string.IsNullOrWhiteSpace(document.FilePath))
                        .Select(
                            document => new {
                                Project = item.Project,
                                Document = document,
                            }))
                .Select(item => CreateDocumentContext(item.Project, item.Document, solutionDirectory))
                .OrderBy(document => document.Fact.Path, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var projectContexts = sourceProjects
                .Select(
                    project => {
                        var projectFact = projectFacts.Single(fact => fact.ProjectId == projectIdsByPath[Path.GetFullPath(project.FilePath!)]);
                        var documents = documentContexts
                            .Where(document => document.Fact.ProjectId == projectFact.ProjectId)
                            .ToArray();
                        return new WorkspaceProjectContext(project, projectFact, documents);
                    })
                .ToArray();

            var solutionFact = new SolutionFact(
                Path.GetFileNameWithoutExtension(normalizedRequest.SolutionPath),
                normalizedRequest.SolutionPath,
                projectFacts.Length,
                documentContexts.Length);

            return new WorkspaceLoadResult(
                normalizedRequest,
                solutionFact,
                projectFacts,
                documentContexts.Select(context => context.Fact).ToArray(),
                projectContexts,
                workspaceDiagnostics.OrderBy(diagnostic => diagnostic.Code, StringComparer.Ordinal).ToArray(),
                solution,
                workspace);
        }
        catch (Exception exception) {
            workspace.Dispose();

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

    private static WorkspaceDocumentContext CreateDocumentContext(Project project, Document document, string solutionDirectory) {
        var path = document.FilePath!;
        var lineCount = File.Exists(path)
            ? File.ReadLines(path).Count()
            : 0;

        var fact = new DocumentFact(
            StableId.ForDocument(Path.GetRelativePath(solutionDirectory, path)),
            StableId.ForProject(project.Name),
            Path.GetRelativePath(solutionDirectory, path).Replace('\\', '/'),
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
}
