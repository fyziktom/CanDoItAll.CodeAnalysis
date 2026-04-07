using System.Diagnostics;
using System.Text;
using CanDoItAll.CodeAnalytics.Abstractions;
using CanDoItAll.CodeAnalytics.Abstractions.Commands;
using CanDoItAll.CodeAnalytics.Abstractions.Options;
using CanDoItAll.CodeAnalytics.Abstractions.Progress;
using CanDoItAll.CodeAnalytics.Abstractions.Responses;
using CanDoItAll.CodeAnalytics.Analysis.Rules;
using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Exports;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Insights;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Facts.Dependencies;
using CanDoItAll.CodeAnalytics.Facts.Persistence;
using CanDoItAll.CodeAnalytics.Facts.Services;
using CanDoItAll.CodeAnalytics.Facts.Symbols;
using CanDoItAll.CodeAnalytics.Rendering.Exports;
using CanDoItAll.CodeAnalytics.Storage.Paths;
using CanDoItAll.CodeAnalytics.Storage.Snapshots;
using CanDoItAll.CodeAnalytics.Workspace.Loading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CanDoItAll.CodeAnalytics.Application.Services;

public sealed partial class CodeAnalyticsApplicationService : ICodeAnalyticsApplicationService {
    private const string SchemaVersion = "1.1.0";
    private readonly CodeAnalyticsApplicationOptions _options;
    private readonly MsBuildWorkspaceLoader _workspaceLoader;
    private readonly SymbolFactsCollector _symbolFactsCollector;
    private readonly DependencyFactCollector _dependencyFactCollector;
    private readonly ServiceRegistrationCollector _serviceRegistrationCollector;
    private readonly PersistenceFactCollector _persistenceFactCollector;
    private readonly ArchitectureInsightBuilder _insightBuilder;
    private readonly ExportBundleBuilder _exportBundleBuilder;
    private readonly FileSnapshotRepository _snapshotRepository;
    private readonly ILogger<CodeAnalyticsApplicationService> _logger;

    public CodeAnalyticsApplicationService(
        CodeAnalyticsApplicationOptions options,
        MsBuildWorkspaceLoader workspaceLoader,
        SymbolFactsCollector symbolFactsCollector,
        DependencyFactCollector dependencyFactCollector,
        ServiceRegistrationCollector serviceRegistrationCollector,
        PersistenceFactCollector persistenceFactCollector,
        ArchitectureInsightBuilder insightBuilder,
        ExportBundleBuilder exportBundleBuilder,
        FileSnapshotRepository snapshotRepository,
        ILogger<CodeAnalyticsApplicationService>? logger = null) {
        _options = options;
        _workspaceLoader = workspaceLoader;
        _symbolFactsCollector = symbolFactsCollector;
        _dependencyFactCollector = dependencyFactCollector;
        _serviceRegistrationCollector = serviceRegistrationCollector;
        _persistenceFactCollector = persistenceFactCollector;
        _insightBuilder = insightBuilder;
        _exportBundleBuilder = exportBundleBuilder;
        _snapshotRepository = snapshotRepository;
        _logger = logger ?? NullLogger<CodeAnalyticsApplicationService>.Instance;
    }

    public async Task<SnapshotBuildResponse> BuildSnapshotAsync(
        BuildArchitectureSnapshotCommand command,
        IAnalysisProgressReporter? progressReporter = null,
        CancellationToken cancellationToken = default) {
        var progressEvents = new List<AnalysisProgressEvent>();
        var diagnostics = new List<AnalysisDiagnostic>();
        var request = new AnalysisRequest(
            command.SolutionPath,
            command.ScopeProjectNames ?? [],
            command.ScopeNamespacePrefixes ?? [],
            command.IncludeDi,
            command.IncludePersistence,
            command.IncludeRisks,
            command.IncludeXmlDocs,
            command.IncludeMermaidExports);
        var facts = CreateEmptyFacts(request);
        var pathResolver = new SnapshotPathResolver(_options.OutputRootPath);
        var requestHash = _snapshotRepository.ComputeRequestHash(request, _options.GeneratorVersion, SchemaVersion);

        ReportProgress(progressEvents, progressReporter, "build", AnalysisProgressState.Started, $"Building snapshot for {command.SolutionPath}.");

        try {
            var cachedSnapshot = await TryGetCachedSnapshotAsync(command, requestHash, pathResolver, progressEvents, progressReporter, cancellationToken);
            if (cachedSnapshot is not null) {
                return cachedSnapshot;
            }

            using var workspace = await LoadWorkspaceAsync(request, progressEvents, progressReporter, cancellationToken);
            diagnostics.AddRange(workspace.Diagnostics);

            if (workspace.RoslynSolution is null || workspace.Solution is null) {
                ReportProgress(progressEvents, progressReporter, "workspace", AnalysisProgressState.Info, "Workspace load did not produce a Roslyn solution. Building an error snapshot.");
                facts = CreateEmptyFacts(workspace.Request);
            }
            else {
                facts = await CollectFactsAsync(workspace, diagnostics, progressEvents, progressReporter, cancellationToken);
            }

            diagnostics = SortDiagnostics(diagnostics).ToList();
            var finalSnapshot = await BuildSnapshotArtifactAsync(
                workspaceRequest: workspace.Request,
                facts,
                diagnostics,
                requestHash,
                pathResolver,
                progressEvents,
                progressReporter,
                cancellationToken);

            ReportProgress(progressEvents, progressReporter, "build", AnalysisProgressState.Completed, $"Snapshot {finalSnapshot.SnapshotId} completed.");
            return new SnapshotBuildResponse(
                finalSnapshot,
                false,
                finalSnapshot.Diagnostics,
                finalSnapshot.Diagnostics.Any(diagnostic => diagnostic.Severity == AnalysisDiagnosticSeverity.Error),
                progressEvents.ToArray());
        }
        catch (OperationCanceledException) {
            ReportProgress(progressEvents, progressReporter, "build", AnalysisProgressState.Failed, "Snapshot build was canceled.");
            throw;
        }
        catch (Exception exception) {
            _logger.LogError(exception, "Snapshot build failed for {WorkspacePath}", request.SolutionPath);
            diagnostics.Add(
                new AnalysisDiagnostic(
                    "APP1099",
                    AnalysisDiagnosticSeverity.Error,
                    exception.Message));
            diagnostics = SortDiagnostics(diagnostics).ToList();
            ReportProgress(progressEvents, progressReporter, "build", AnalysisProgressState.Failed, $"Snapshot build failed: {exception.Message}");

            var failedSnapshot = CreateFallbackSnapshot(
                request,
                $"snap-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}-{requestHash[..8]}-failed",
                facts,
                diagnostics);
            return new SnapshotBuildResponse(
                failedSnapshot,
                false,
                failedSnapshot.Diagnostics,
                true,
                progressEvents.ToArray());
        }
    }

    private async Task<SnapshotBuildResponse?> TryGetCachedSnapshotAsync(
        BuildArchitectureSnapshotCommand command,
        string requestHash,
        SnapshotPathResolver pathResolver,
        IList<AnalysisProgressEvent> progressEvents,
        IAnalysisProgressReporter? progressReporter,
        CancellationToken cancellationToken) {
        if (command.ForceRefresh) {
            ReportProgress(progressEvents, progressReporter, "cache", AnalysisProgressState.Info, "Force refresh requested. Cache lookup skipped.");
            return null;
        }

        ReportProgress(progressEvents, progressReporter, "cache", AnalysisProgressState.Started, "Checking the deterministic file cache.");
        var cachedSnapshot = await _snapshotRepository.TryGetCachedSnapshotAsync(pathResolver, requestHash, cancellationToken);
        if (cachedSnapshot is null) {
            ReportProgress(progressEvents, progressReporter, "cache", AnalysisProgressState.Info, "No cached snapshot matched the current request.");
            return null;
        }

        ReportProgress(progressEvents, progressReporter, "cache", AnalysisProgressState.Completed, $"Loaded cached snapshot {cachedSnapshot.Snapshot.SnapshotId}.");
        ReportProgress(progressEvents, progressReporter, "build", AnalysisProgressState.Completed, "Snapshot build completed from cache.");
        return new SnapshotBuildResponse(
            cachedSnapshot.Snapshot,
            true,
            cachedSnapshot.Snapshot.Diagnostics,
            cachedSnapshot.Snapshot.Diagnostics.Any(diagnostic => diagnostic.Severity == AnalysisDiagnosticSeverity.Error),
            progressEvents.ToArray());
    }

    private async Task<ArchitectureFacts> CollectFactsAsync(
        WorkspaceLoadResult workspace,
        List<AnalysisDiagnostic> diagnostics,
        IList<AnalysisProgressEvent> progressEvents,
        IAnalysisProgressReporter? progressReporter,
        CancellationToken cancellationToken) {
        var symbols = await ExecuteStageAsync(
            "symbols",
            "Collecting source symbols and XML documentation.",
            () => _symbolFactsCollector.CollectAsync(workspace, cancellationToken),
            new SymbolCollectionResult([], [], [], []),
            diagnostics,
            "APP1001",
            progressEvents,
            progressReporter,
            cancellationToken);
        diagnostics.AddRange(symbols.Diagnostics);

        var dependencies = await ExecuteStageAsync(
            "dependencies",
            "Collecting project, module, namespace, and type dependencies.",
            () => _dependencyFactCollector.CollectAsync(workspace, symbols, cancellationToken),
            new DependencyCollectionResult([], [], [], []),
            diagnostics,
            "APP1002",
            progressEvents,
            progressReporter,
            cancellationToken);
        diagnostics.AddRange(dependencies.Diagnostics);

        var services = await ExecuteStageAsync(
            "services",
            "Collecting dependency-injection registrations.",
            () => _serviceRegistrationCollector.CollectAsync(workspace, cancellationToken),
            new ServiceRegistrationCollectionResult([], []),
            diagnostics,
            "APP1003",
            progressEvents,
            progressReporter,
            cancellationToken);
        diagnostics.AddRange(services.Diagnostics);

        var persistence = await ExecuteStageAsync(
            "persistence",
            "Collecting EF Core persistence facts.",
            () => _persistenceFactCollector.CollectAsync(workspace, symbols, cancellationToken),
            new PersistenceCollectionResult([], [], [], []),
            diagnostics,
            "APP1004",
            progressEvents,
            progressReporter,
            cancellationToken);
        diagnostics.AddRange(persistence.Diagnostics);

        return new ArchitectureFacts(
            workspace.Solution!,
            workspace.Projects,
            workspace.Documents,
            dependencies.Modules,
            symbols.Namespaces,
            symbols.Types,
            symbols.Members,
            dependencies.TypeRelationships,
            services.Services,
            persistence.DbContexts,
            persistence.Entities,
            persistence.EntityRelationships,
            dependencies.Dependencies);
    }

    private async Task<ArchitectureSnapshot> BuildSnapshotArtifactAsync(
        AnalysisRequest workspaceRequest,
        ArchitectureFacts facts,
        List<AnalysisDiagnostic> diagnostics,
        string requestHash,
        SnapshotPathResolver pathResolver,
        IList<AnalysisProgressEvent> progressEvents,
        IAnalysisProgressReporter? progressReporter,
        CancellationToken cancellationToken) {
        var snapshotId = $"snap-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}-{requestHash[..8]}";
        var draftSnapshot = await ExecuteStageAsync(
            "insights",
            "Deriving architectural findings and summary insights.",
            () => Task.FromResult(
                new ArchitectureSnapshot(
                    SchemaVersion,
                    _options.GeneratorVersion,
                    snapshotId,
                    DateTimeOffset.UtcNow,
                    workspaceRequest,
                    facts,
                    _insightBuilder.Build(workspaceRequest, facts, diagnostics),
                    ArchitectureExports.Empty,
                    diagnostics)),
            CreateFallbackSnapshot(workspaceRequest, snapshotId, facts, diagnostics),
            diagnostics,
            "APP1005",
            progressEvents,
            progressReporter,
            cancellationToken);

        var rendering = await ExecuteStageAsync(
            "exports",
            "Rendering Markdown and Mermaid exports.",
            () => Task.FromResult(_exportBundleBuilder.Build(draftSnapshot, _options.MaxDiagramNodes)),
            new RenderingResult([], []),
            diagnostics,
            "APP1006",
            progressEvents,
            progressReporter,
            cancellationToken);
        diagnostics.AddRange(rendering.Diagnostics);
        diagnostics = SortDiagnostics(diagnostics).ToList();

        var finalSnapshot = draftSnapshot with {
            Insights = _insightBuilder.Build(workspaceRequest, facts, diagnostics),
            Exports = CreateExports(rendering.Exports),
            Diagnostics = diagnostics,
        };

        await PersistSnapshotAsync(pathResolver, finalSnapshot, requestHash, rendering.Exports, progressEvents, progressReporter, cancellationToken);
        return finalSnapshot;
    }

    private async Task<WorkspaceLoadResult> LoadWorkspaceAsync(
        AnalysisRequest request,
        IList<AnalysisProgressEvent> progressEvents,
        IAnalysisProgressReporter? progressReporter,
        CancellationToken cancellationToken) {
        ReportProgress(progressEvents, progressReporter, "workspace", AnalysisProgressState.Started, "Loading the MSBuild workspace.");

        try {
            var workspace = await _workspaceLoader.LoadAsync(request, cancellationToken);
            var workspaceState = workspace.HasBlockingErrors
                ? AnalysisProgressState.Failed
                : AnalysisProgressState.Completed;
            var workspaceMessage = workspace.RoslynSolution is null
                ? "Workspace load completed without a Roslyn solution."
                : $"Workspace load completed with {workspace.Projects.Count} source projects.";
            ReportProgress(progressEvents, progressReporter, "workspace", workspaceState, workspaceMessage);
            return workspace;
        }
        catch (OperationCanceledException) {
            ReportProgress(progressEvents, progressReporter, "workspace", AnalysisProgressState.Failed, "Workspace loading was canceled.");
            throw;
        }
        catch (Exception exception) {
            _logger.LogError(exception, "Workspace loading failed for {WorkspacePath}", request.SolutionPath);
            ReportProgress(progressEvents, progressReporter, "workspace", AnalysisProgressState.Failed, exception.Message);
            throw;
        }
    }

    private async Task<T> ExecuteStageAsync<T>(
        string stage,
        string description,
        Func<Task<T>> action,
        T fallback,
        ICollection<AnalysisDiagnostic> diagnostics,
        string diagnosticCode,
        IList<AnalysisProgressEvent> progressEvents,
        IAnalysisProgressReporter? progressReporter,
        CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ReportProgress(progressEvents, progressReporter, stage, AnalysisProgressState.Started, description);
        var stopwatch = Stopwatch.StartNew();

        try {
            var result = await action();
            stopwatch.Stop();
            ReportProgress(progressEvents, progressReporter, stage, AnalysisProgressState.Completed, $"{description} Completed in {stopwatch.ElapsedMilliseconds} ms.");
            return result;
        }
        catch (OperationCanceledException) {
            ReportProgress(progressEvents, progressReporter, stage, AnalysisProgressState.Failed, $"{description} Canceled.");
            throw;
        }
        catch (Exception exception) {
            stopwatch.Stop();
            _logger.LogError(exception, "{Stage} failed after {ElapsedMilliseconds} ms", stage, stopwatch.ElapsedMilliseconds);
            diagnostics.Add(
                new AnalysisDiagnostic(
                    diagnosticCode,
                    AnalysisDiagnosticSeverity.Error,
                    $"{description} {exception.Message}"));
            ReportProgress(progressEvents, progressReporter, stage, AnalysisProgressState.Failed, $"{description} Failed after {stopwatch.ElapsedMilliseconds} ms.");
            return fallback;
        }
    }

    private async Task PersistSnapshotAsync(
        SnapshotPathResolver pathResolver,
        ArchitectureSnapshot snapshot,
        string requestHash,
        IReadOnlyList<PreparedExport> exports,
        IList<AnalysisProgressEvent> progressEvents,
        IAnalysisProgressReporter? progressReporter,
        CancellationToken cancellationToken) {
        ReportProgress(progressEvents, progressReporter, "persist", AnalysisProgressState.Started, $"Persisting snapshot {snapshot.SnapshotId}.");

        try {
            await _snapshotRepository.StoreAsync(pathResolver, snapshot, requestHash, exports, cancellationToken);
            ReportProgress(progressEvents, progressReporter, "persist", AnalysisProgressState.Completed, $"Persisted snapshot {snapshot.SnapshotId}.");
        }
        catch (OperationCanceledException) {
            ReportProgress(progressEvents, progressReporter, "persist", AnalysisProgressState.Failed, $"Persisting snapshot {snapshot.SnapshotId} was canceled.");
            throw;
        }
        catch (Exception exception) {
            _logger.LogError(exception, "Persisting snapshot {SnapshotId} failed", snapshot.SnapshotId);
            ReportProgress(progressEvents, progressReporter, "persist", AnalysisProgressState.Failed, exception.Message);
            throw;
        }
    }

    private void ReportProgress(
        IList<AnalysisProgressEvent> progressEvents,
        IAnalysisProgressReporter? progressReporter,
        string stage,
        AnalysisProgressState state,
        string message) {
        var progressEvent = new AnalysisProgressEvent(stage, state, message, DateTimeOffset.UtcNow);
        progressEvents.Add(progressEvent);
        progressReporter?.Report(progressEvent);

        switch (state) {
            case AnalysisProgressState.Failed:
                _logger.LogError("{Stage}: {Message}", stage, message);
                break;
            default:
                _logger.LogInformation("{Stage}: {Message}", stage, message);
                break;
        }
    }

}
