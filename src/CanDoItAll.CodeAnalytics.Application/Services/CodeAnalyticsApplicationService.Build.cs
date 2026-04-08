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
using CanDoItAll.CodeAnalytics.Facts.Members;
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
    private readonly MemberRelationshipCollector _memberRelationshipCollector;
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
        MemberRelationshipCollector memberRelationshipCollector,
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
        _memberRelationshipCollector = memberRelationshipCollector;
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
}
