using System.Text;
using CanDoItAll.CodeAnalytics.Abstractions;
using CanDoItAll.CodeAnalytics.Abstractions.Commands;
using CanDoItAll.CodeAnalytics.Abstractions.Options;
using CanDoItAll.CodeAnalytics.Abstractions.Queries;
using CanDoItAll.CodeAnalytics.Abstractions.Responses;
using CanDoItAll.CodeAnalytics.Analysis.Rules;
using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Exports;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Facts.Dependencies;
using CanDoItAll.CodeAnalytics.Facts.Persistence;
using CanDoItAll.CodeAnalytics.Facts.Services;
using CanDoItAll.CodeAnalytics.Facts.Symbols;
using CanDoItAll.CodeAnalytics.Rendering.Exports;
using CanDoItAll.CodeAnalytics.Storage.Paths;
using CanDoItAll.CodeAnalytics.Storage.Recent;
using CanDoItAll.CodeAnalytics.Storage.Snapshots;
using CanDoItAll.CodeAnalytics.Workspace.Loading;

namespace CanDoItAll.CodeAnalytics.Application.Services;

public sealed class CodeAnalyticsApplicationService : ICodeAnalyticsApplicationService {
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

    public CodeAnalyticsApplicationService(
        CodeAnalyticsApplicationOptions options,
        MsBuildWorkspaceLoader workspaceLoader,
        SymbolFactsCollector symbolFactsCollector,
        DependencyFactCollector dependencyFactCollector,
        ServiceRegistrationCollector serviceRegistrationCollector,
        PersistenceFactCollector persistenceFactCollector,
        ArchitectureInsightBuilder insightBuilder,
        ExportBundleBuilder exportBundleBuilder,
        FileSnapshotRepository snapshotRepository) {
        _options = options;
        _workspaceLoader = workspaceLoader;
        _symbolFactsCollector = symbolFactsCollector;
        _dependencyFactCollector = dependencyFactCollector;
        _serviceRegistrationCollector = serviceRegistrationCollector;
        _persistenceFactCollector = persistenceFactCollector;
        _insightBuilder = insightBuilder;
        _exportBundleBuilder = exportBundleBuilder;
        _snapshotRepository = snapshotRepository;
    }

    public async Task<SnapshotBuildResponse> BuildSnapshotAsync(
        BuildArchitectureSnapshotCommand command,
        CancellationToken cancellationToken = default) {
        var request = new AnalysisRequest(
            command.SolutionPath,
            command.ScopeProjectNames ?? [],
            command.ScopeNamespacePrefixes ?? [],
            command.IncludeDi,
            command.IncludePersistence,
            command.IncludeRisks,
            command.IncludeXmlDocs,
            command.IncludeMermaidExports);
        var pathResolver = new SnapshotPathResolver(_options.OutputRootPath);
        var requestHash = _snapshotRepository.ComputeRequestHash(request, _options.GeneratorVersion, SchemaVersion);

        if (!command.ForceRefresh) {
            var cachedSnapshot = await _snapshotRepository.TryGetCachedSnapshotAsync(pathResolver, requestHash, cancellationToken);
            if (cachedSnapshot is not null) {
                return new SnapshotBuildResponse(cachedSnapshot.Snapshot, true, cachedSnapshot.Snapshot.Diagnostics);
            }
        }

        using var workspace = await _workspaceLoader.LoadAsync(request, cancellationToken);
        var diagnostics = new List<AnalysisDiagnostic>(workspace.Diagnostics);
        ArchitectureFacts facts;

        if (workspace.RoslynSolution is null || workspace.Solution is null) {
            facts = CreateEmptyFacts(workspace.Request);
        }
        else {
            var symbols = await _symbolFactsCollector.CollectAsync(workspace, cancellationToken);
            diagnostics.AddRange(symbols.Diagnostics);

            var dependencies = await _dependencyFactCollector.CollectAsync(workspace, symbols, cancellationToken);
            diagnostics.AddRange(dependencies.Diagnostics);

            var services = await _serviceRegistrationCollector.CollectAsync(workspace, cancellationToken);
            diagnostics.AddRange(services.Diagnostics);

            var persistence = await _persistenceFactCollector.CollectAsync(workspace, symbols, cancellationToken);
            diagnostics.AddRange(persistence.Diagnostics);

            facts = new ArchitectureFacts(
                workspace.Solution,
                workspace.Projects,
                workspace.Documents,
                dependencies.Modules,
                symbols.Namespaces,
                symbols.Types,
                symbols.Members,
                services.Services,
                persistence.DbContexts,
                persistence.Entities,
                dependencies.Dependencies);
        }

        diagnostics = diagnostics
            .OrderBy(diagnostic => diagnostic.Code, StringComparer.Ordinal)
            .ThenBy(diagnostic => diagnostic.Message, StringComparer.Ordinal)
            .ToList();

        var snapshotId = $"snap-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}-{requestHash[..8]}";
        var draftSnapshot = new ArchitectureSnapshot(
            SchemaVersion,
            _options.GeneratorVersion,
            snapshotId,
            DateTimeOffset.UtcNow,
            workspace.Request,
            facts,
            _insightBuilder.Build(workspace.Request, facts, diagnostics),
            ArchitectureExports.Empty,
            diagnostics);

        var rendering = _exportBundleBuilder.Build(draftSnapshot, _options.MaxDiagramNodes);
        diagnostics.AddRange(rendering.Diagnostics);
        diagnostics = diagnostics
            .OrderBy(diagnostic => diagnostic.Code, StringComparer.Ordinal)
            .ThenBy(diagnostic => diagnostic.Message, StringComparer.Ordinal)
            .ToList();

        var exports = CreateExports(rendering.Exports);
        var finalSnapshot = draftSnapshot with {
            Insights = _insightBuilder.Build(workspace.Request, facts, diagnostics),
            Exports = exports,
            Diagnostics = diagnostics,
        };

        await _snapshotRepository.StoreAsync(pathResolver, finalSnapshot, requestHash, rendering.Exports, cancellationToken);

        return new SnapshotBuildResponse(finalSnapshot, false, finalSnapshot.Diagnostics);
    }

    public async Task<SnapshotDashboardResponse?> GetDashboardAsync(
        string snapshotId,
        int recentTake = 10,
        CancellationToken cancellationToken = default) {
        var snapshot = await GetSnapshotAsync(snapshotId, cancellationToken);
        if (snapshot is null) {
            return null;
        }

        var recent = await ListRecentSnapshotsAsync(recentTake, cancellationToken);
        return new SnapshotDashboardResponse(
            snapshot,
            snapshot.Insights.Findings.Take(5).ToArray(),
            snapshot.Diagnostics.Take(5).ToArray(),
            recent);
    }

    public async Task<DependencyViewResponse?> GetDependenciesAsync(
        SnapshotQuery query,
        CancellationToken cancellationToken = default) {
        var snapshot = await GetSnapshotAsync(query.SnapshotId, cancellationToken);
        if (snapshot is null) {
            return null;
        }

        var modules = ApplyTextFilter(snapshot.Facts.Modules, query.SearchText, module => module.Name);
        var includedIds = modules.Select(module => module.ModuleId).ToHashSet(StringComparer.Ordinal);
        var dependencies = string.IsNullOrWhiteSpace(query.SearchText)
            ? snapshot.Facts.Dependencies
            : snapshot.Facts.Dependencies
                .Where(edge => includedIds.Contains(edge.FromId) || includedIds.Contains(edge.ToId))
                .ToArray();

        return new DependencyViewResponse(snapshot.SnapshotId, query.SearchText, modules, dependencies, snapshot.Insights.Cycles);
    }

    public async Task<ServiceViewResponse?> GetServicesAsync(
        SnapshotQuery query,
        CancellationToken cancellationToken = default) {
        var snapshot = await GetSnapshotAsync(query.SnapshotId, cancellationToken);
        if (snapshot is null) {
            return null;
        }

        var services = ApplyTextFilter(
            snapshot.Facts.ServiceRegistrations,
            query.SearchText,
            service => $"{service.ServiceTypeDisplayName} {service.ImplementationTypeDisplayName}");
        var diagnostics = snapshot.Diagnostics
            .Where(diagnostic => diagnostic.Code.StartsWith("DI", StringComparison.Ordinal))
            .ToArray();

        return new ServiceViewResponse(snapshot.SnapshotId, query.SearchText, services, diagnostics);
    }

    public async Task<PersistenceViewResponse?> GetPersistenceAsync(
        SnapshotQuery query,
        CancellationToken cancellationToken = default) {
        var snapshot = await GetSnapshotAsync(query.SnapshotId, cancellationToken);
        if (snapshot is null) {
            return null;
        }

        var dbContexts = ApplyTextFilter(snapshot.Facts.DbContexts, query.SearchText, item => item.DisplayName);
        var entities = ApplyTextFilter(snapshot.Facts.Entities, query.SearchText, item => item.DisplayName);
        var diagnostics = snapshot.Diagnostics
            .Where(diagnostic => diagnostic.Code.StartsWith("EF", StringComparison.Ordinal))
            .ToArray();

        return new PersistenceViewResponse(snapshot.SnapshotId, query.SearchText, dbContexts, entities, diagnostics);
    }

    public async Task<FindingsViewResponse?> GetFindingsAsync(
        SnapshotQuery query,
        CancellationToken cancellationToken = default) {
        var snapshot = await GetSnapshotAsync(query.SnapshotId, cancellationToken);
        if (snapshot is null) {
            return null;
        }

        var findings = ApplyTextFilter(snapshot.Insights.Findings, query.SearchText, item => $"{item.Title} {item.Description}");
        var questions = ApplyTextFilter(snapshot.Insights.OpenQuestions, query.SearchText, item => $"{item.Title} {item.Description}");
        return new FindingsViewResponse(snapshot.SnapshotId, query.SearchText, findings, questions, snapshot.Insights.Hotspots);
    }

    public async Task<ExportsViewResponse?> GetExportsAsync(
        string snapshotId,
        CancellationToken cancellationToken = default) {
        var snapshot = await GetSnapshotAsync(snapshotId, cancellationToken);
        if (snapshot is null) {
            return null;
        }

        return new ExportsViewResponse(snapshot.SnapshotId, snapshot.Exports.Artifacts);
    }

    public Task<ArchitectureSnapshot?> GetSnapshotAsync(
        string snapshotId,
        CancellationToken cancellationToken = default) {
        var pathResolver = new SnapshotPathResolver(_options.OutputRootPath);
        return _snapshotRepository.LoadSnapshotAsync(pathResolver, snapshotId, cancellationToken);
    }

    public async Task<IReadOnlyList<RecentSnapshotItem>> ListRecentSnapshotsAsync(
        int take,
        CancellationToken cancellationToken = default) {
        var pathResolver = new SnapshotPathResolver(_options.OutputRootPath);
        var recent = await _snapshotRepository.ListRecentAsync(pathResolver, take, cancellationToken);
        return recent.Select(MapRecentItem).ToArray();
    }

    private static ArchitectureFacts CreateEmptyFacts(AnalysisRequest request) {
        var solutionName = Path.GetFileNameWithoutExtension(request.SolutionPath);
        return new ArchitectureFacts(
            new SolutionFact(solutionName, request.SolutionPath, 0, 0),
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            []);
    }

    private static ArchitectureExports CreateExports(IReadOnlyList<PreparedExport> exports) {
        var artifacts = exports
            .Select(
                export => new ExportArtifact(
                    export.Kind,
                    export.RelativePath,
                    export.Title,
                    export.Description,
                    Encoding.UTF8.GetByteCount(export.Content)))
            .OrderBy(artifact => artifact.RelativePath, StringComparer.Ordinal)
            .Append(
                new ExportArtifact(
                    ExportArtifactKind.SnapshotJson,
                    "snapshot.json",
                    "Snapshot JSON",
                    "Canonical architecture snapshot."))
            .ToArray();

        return new ArchitectureExports(artifacts);
    }

    private static IReadOnlyList<T> ApplyTextFilter<T>(
        IReadOnlyList<T> source,
        string? searchText,
        Func<T, string> textSelector) {
        if (string.IsNullOrWhiteSpace(searchText)) {
            return source;
        }

        return source
            .Where(item => textSelector(item).Contains(searchText, StringComparison.OrdinalIgnoreCase))
            .ToArray();
    }

    private static RecentSnapshotItem MapRecentItem(RecentSnapshotRecord record) {
        return new RecentSnapshotItem(
            record.SnapshotId,
            record.SolutionName,
            record.SolutionPath,
            record.CreatedUtc,
            record.FindingCount,
            record.DiagnosticCount,
            false);
    }
}
