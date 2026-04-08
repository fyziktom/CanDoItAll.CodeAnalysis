using CanDoItAll.CodeAnalytics.Abstractions;
using CanDoItAll.CodeAnalytics.Abstractions.Commands;
using CanDoItAll.CodeAnalytics.Abstractions.Progress;
using CanDoItAll.CodeAnalytics.Abstractions.Responses;
using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Exports;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Facts.Dependencies;
using CanDoItAll.CodeAnalytics.Facts.Members;
using CanDoItAll.CodeAnalytics.Facts.Persistence;
using CanDoItAll.CodeAnalytics.Facts.Services;
using CanDoItAll.CodeAnalytics.Facts.Symbols;
using CanDoItAll.CodeAnalytics.Rendering.Exports;
using CanDoItAll.CodeAnalytics.Storage.Paths;
using CanDoItAll.CodeAnalytics.Workspace.Loading;

namespace CanDoItAll.CodeAnalytics.Application.Services;

public sealed partial class CodeAnalyticsApplicationService {
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

        var memberRelationships = await ExecuteStageAsync(
            "member-context",
            "Collecting member-level invocation and construction relationships.",
            () => _memberRelationshipCollector.CollectAsync(workspace, symbols, cancellationToken),
            new MemberRelationshipCollectionResult([], []),
            diagnostics,
            "APP1007",
            progressEvents,
            progressReporter,
            cancellationToken);
        diagnostics.AddRange(memberRelationships.Diagnostics);

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
            memberRelationships.Relationships,
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
}
