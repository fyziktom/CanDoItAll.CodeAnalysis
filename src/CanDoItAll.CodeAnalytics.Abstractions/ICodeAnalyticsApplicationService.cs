using CanDoItAll.CodeAnalytics.Abstractions.Commands;
using CanDoItAll.CodeAnalytics.Abstractions.Queries;
using CanDoItAll.CodeAnalytics.Abstractions.Responses;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;

namespace CanDoItAll.CodeAnalytics.Abstractions;

public interface ICodeAnalyticsApplicationService {
    Task<SnapshotBuildResponse> BuildSnapshotAsync(
        BuildArchitectureSnapshotCommand command,
        IAnalysisProgressReporter? progressReporter = null,
        CancellationToken cancellationToken = default);

    Task<SnapshotDashboardResponse?> GetDashboardAsync(
        string snapshotId,
        int recentTake = 10,
        CancellationToken cancellationToken = default);

    Task<DependencyViewResponse?> GetDependenciesAsync(
        SnapshotQuery query,
        CancellationToken cancellationToken = default);

    Task<ServiceViewResponse?> GetServicesAsync(
        SnapshotQuery query,
        CancellationToken cancellationToken = default);

    Task<PersistenceViewResponse?> GetPersistenceAsync(
        SnapshotQuery query,
        CancellationToken cancellationToken = default);

    Task<FindingsViewResponse?> GetFindingsAsync(
        SnapshotQuery query,
        CancellationToken cancellationToken = default);

    Task<SolutionInventoryResponse?> GetSolutionInventoryAsync(
        SolutionInventoryQuery query,
        CancellationToken cancellationToken = default);

    Task<ProjectInventoryResponse?> GetProjectInventoryAsync(
        ProjectInventoryQuery query,
        CancellationToken cancellationToken = default);

    Task<DocumentSourceResponse?> GetDocumentSourceAsync(
        DocumentQuery query,
        CancellationToken cancellationToken = default);

    Task<DocumentSymbolsResponse?> GetDocumentSymbolsAsync(
        DocumentQuery query,
        CancellationToken cancellationToken = default);

    Task<TypesViewResponse?> GetTypesAsync(
        TypeSearchQuery query,
        CancellationToken cancellationToken = default);

    Task<SymbolSearchResponse?> SearchSymbolsAsync(
        SymbolSearchQuery query,
        CancellationToken cancellationToken = default);

    Task<SymbolDefinitionResponse?> GetSymbolDefinitionAsync(
        SymbolDefinitionQuery query,
        CancellationToken cancellationToken = default);

    Task<SymbolMembersResponse?> GetSymbolMembersAsync(
        SymbolMembersQuery query,
        CancellationToken cancellationToken = default);

    Task<SymbolImplementationsResponse?> GetSymbolImplementationsAsync(
        SymbolImplementationsQuery query,
        CancellationToken cancellationToken = default);

    Task<SymbolReferencesResponse?> GetSymbolReferencesAsync(
        SymbolReferencesQuery query,
        CancellationToken cancellationToken = default);

    Task<FocusedContextResponse?> GetFocusedContextAsync(
        FocusedContextQuery query,
        CancellationToken cancellationToken = default);

    Task<ExportsViewResponse?> GetExportsAsync(
        string snapshotId,
        CancellationToken cancellationToken = default);

    Task<ArchitectureSnapshot?> GetSnapshotAsync(
        string snapshotId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RecentSnapshotItem>> ListRecentSnapshotsAsync(
        int take,
        CancellationToken cancellationToken = default);
}
