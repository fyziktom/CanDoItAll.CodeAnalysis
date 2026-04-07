using CanDoItAll.CodeAnalytics.Abstractions.Queries;
using CanDoItAll.CodeAnalytics.Abstractions.Responses;
using CanDoItAll.CodeAnalytics.Domain.Facts;
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

        var entityNamesById = snapshot.Facts.Entities.ToDictionary(entity => entity.EntityId, entity => entity.DisplayName, StringComparer.Ordinal);
        var dbContexts = ApplyTextFilter(
            snapshot.Facts.DbContexts,
            query.SearchText,
            item => string.Join(
                ' ',
                item.DisplayName,
                string.Join(' ', item.EntityTypeIds.Select(entityId => entityNamesById.TryGetValue(entityId, out var entityName) ? entityName : string.Empty))));
        var entities = ApplyTextFilter(
            snapshot.Facts.Entities,
            query.SearchText,
            item => string.Join(
                ' ',
                item.DisplayName,
                item.TableName ?? string.Empty,
                item.Schema ?? string.Empty,
                item.Source.Path));
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

    public async Task<TypesViewResponse?> GetTypesAsync(
        TypeSearchQuery query,
        CancellationToken cancellationToken = default) {
        var snapshot = await GetSnapshotAsync(query.SnapshotId, cancellationToken);
        if (snapshot is null) {
            return null;
        }

        var availableProjects = snapshot.Facts.Projects
            .Select(project => project.Name)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var includeMembers = query.IncludeMembers || query.MethodsOnly || !string.IsNullOrWhiteSpace(query.MemberSearchText);
        var membersByTypeId = snapshot.Facts.Members
            .GroupBy(member => member.TypeId, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<MemberFact>)group
                    .OrderBy(member => member.DisplayName, StringComparer.Ordinal)
                    .ToArray(),
                StringComparer.Ordinal);
        var projectsById = snapshot.Facts.Projects.ToDictionary(project => project.ProjectId, project => project.Name, StringComparer.Ordinal);
        var modulesById = snapshot.Facts.Modules.ToDictionary(module => module.ModuleId, module => module.Name, StringComparer.Ordinal);
        var namespacesById = snapshot.Facts.Namespaces.ToDictionary(@namespace => @namespace.NamespaceId, @namespace => @namespace.Name, StringComparer.Ordinal);
        var searchText = query.SearchText?.Trim();
        var memberSearchText = query.MemberSearchText?.Trim();

        var types = snapshot.Facts.Types
            .Where(type => MatchesProject(projectsById, type, query.ProjectName))
            .Select(
                type => {
                    var allMembers = membersByTypeId.TryGetValue(type.TypeId, out var projectMembers)
                        ? projectMembers
                        : [];
                    var filteredMembers = includeMembers
                        ? FilterMembers(allMembers, memberSearchText, query.MethodsOnly)
                        : [];
                    var matchesType = MatchesType(type, searchText, projectsById, modulesById, namespacesById);
                    var matchesMemberSearch = string.IsNullOrWhiteSpace(memberSearchText) || filteredMembers.Count > 0;

                    return new {
                        Type = type,
                        Members = filteredMembers,
                        Matches = matchesType && matchesMemberSearch,
                    };
                })
            .Where(item => item.Matches)
            .OrderBy(item => projectsById[item.Type.ProjectId], StringComparer.OrdinalIgnoreCase)
            .ThenBy(item => namespacesById[item.Type.NamespaceId], StringComparer.Ordinal)
            .ThenBy(item => item.Type.DisplayName, StringComparer.Ordinal)
            .Select(
                item => new TypeSearchResultItem(
                    projectsById[item.Type.ProjectId],
                    modulesById[item.Type.ModuleId],
                    namespacesById[item.Type.NamespaceId],
                    item.Type,
                    item.Members))
            .ToArray();

        return new TypesViewResponse(
            snapshot.SnapshotId,
            query.SearchText,
            query.ProjectName,
            query.MemberSearchText,
            includeMembers,
            query.MethodsOnly,
            availableProjects,
            types);
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

    private static bool MatchesProject(
        IReadOnlyDictionary<string, string> projectsById,
        TypeFact type,
        string? projectName) {
        if (string.IsNullOrWhiteSpace(projectName)) {
            return true;
        }

        return projectsById.TryGetValue(type.ProjectId, out var currentProjectName)
            && string.Equals(currentProjectName, projectName, StringComparison.OrdinalIgnoreCase);
    }

    private static bool MatchesType(
        TypeFact type,
        string? searchText,
        IReadOnlyDictionary<string, string> projectsById,
        IReadOnlyDictionary<string, string> modulesById,
        IReadOnlyDictionary<string, string> namespacesById) {
        if (string.IsNullOrWhiteSpace(searchText)) {
            return true;
        }

        var text = string.Join(
            ' ',
            type.DisplayName,
            projectsById[type.ProjectId],
            modulesById[type.ModuleId],
            namespacesById[type.NamespaceId],
            type.XmlSummary ?? string.Empty,
            type.Source.Path);
        return text.Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }

    private static IReadOnlyList<MemberFact> FilterMembers(
        IReadOnlyList<MemberFact> members,
        string? memberSearchText,
        bool methodsOnly) {
        return members
            .Where(member => !methodsOnly || member.Kind == MemberKind.Method)
            .Where(member => MatchesMember(member, memberSearchText))
            .OrderBy(member => member.Kind)
            .ThenBy(member => member.DisplayName, StringComparer.Ordinal)
            .ToArray();
    }

    private static bool MatchesMember(MemberFact member, string? memberSearchText) {
        if (string.IsNullOrWhiteSpace(memberSearchText)) {
            return true;
        }

        var text = string.Join(
            ' ',
            member.DisplayName,
            member.ReturnTypeDisplayName,
            string.Join(' ', member.ParameterDisplayNames));
        return text.Contains(memberSearchText, StringComparison.OrdinalIgnoreCase);
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
