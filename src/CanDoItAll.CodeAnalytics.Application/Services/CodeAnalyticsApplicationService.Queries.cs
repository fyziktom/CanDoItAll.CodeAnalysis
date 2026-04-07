using CanDoItAll.CodeAnalytics.Abstractions.Queries;
using CanDoItAll.CodeAnalytics.Abstractions.Responses;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Storage.Paths;
using CanDoItAll.CodeAnalytics.Storage.Recent;

namespace CanDoItAll.CodeAnalytics.Application.Services;

public sealed partial class CodeAnalyticsApplicationService {
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
        return _snapshotRepository.LoadSnapshotAsync(new SnapshotPathResolver(_options.OutputRootPath), snapshotId, cancellationToken);
    }

    public async Task<IReadOnlyList<RecentSnapshotItem>> ListRecentSnapshotsAsync(
        int take,
        CancellationToken cancellationToken = default) {
        var recent = await _snapshotRepository.ListRecentAsync(new SnapshotPathResolver(_options.OutputRootPath), take, cancellationToken);
        return recent.Select(MapRecentItem).ToArray();
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
