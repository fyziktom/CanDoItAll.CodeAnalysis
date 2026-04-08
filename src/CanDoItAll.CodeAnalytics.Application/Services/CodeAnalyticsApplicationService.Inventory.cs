using CanDoItAll.CodeAnalytics.Abstractions.Queries;
using CanDoItAll.CodeAnalytics.Abstractions.Responses;
using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Application.Services;

public sealed partial class CodeAnalyticsApplicationService {
    public async Task<SolutionInventoryResponse?> GetSolutionInventoryAsync(
        SolutionInventoryQuery query,
        CancellationToken cancellationToken = default) {
        var snapshot = await GetSnapshotAsync(query.SnapshotId, cancellationToken);
        if (snapshot is null) {
            return null;
        }

        var projectItems = BuildProjectInventoryItems(
            snapshot.Facts.Projects,
            snapshot.Facts.Documents,
            includeDocuments: query.IncludeDocuments);
        return new SolutionInventoryResponse(snapshot.SnapshotId, snapshot.Facts.Solution, projectItems);
    }

    public async Task<ProjectInventoryResponse?> GetProjectInventoryAsync(
        ProjectInventoryQuery query,
        CancellationToken cancellationToken = default) {
        var snapshot = await GetSnapshotAsync(query.SnapshotId, cancellationToken);
        if (snapshot is null) {
            return null;
        }

        var projects = BuildProjectInventoryItems(
            snapshot.Facts.Projects,
            snapshot.Facts.Documents,
            includeDocuments: query.IncludeDocuments);
        var project = ResolveProjectInventoryItem(projects, query);
        return project is null
            ? null
            : new ProjectInventoryResponse(snapshot.SnapshotId, project);
    }

    private static IReadOnlyList<ProjectInventoryItem> BuildProjectInventoryItems(
        IReadOnlyList<ProjectFact> projects,
        IReadOnlyList<DocumentFact> documents,
        bool includeDocuments) {
        var projectsById = projects.ToDictionary(project => project.ProjectId, StringComparer.Ordinal);
        var documentsByProjectId = documents
            .GroupBy(document => document.ProjectId, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<DocumentFact>)group
                    .OrderBy(document => document.Path, StringComparer.OrdinalIgnoreCase)
                    .ToArray(),
                StringComparer.Ordinal);
        var referencedByLookup = new Dictionary<string, List<ProjectFact>>(StringComparer.Ordinal);
        foreach (var project in projects) {
            foreach (var referencedProjectId in project.ProjectReferences) {
                if (!projectsById.ContainsKey(referencedProjectId)) {
                    continue;
                }

                if (!referencedByLookup.TryGetValue(referencedProjectId, out var referencedByProjects)) {
                    referencedByProjects = [];
                    referencedByLookup.Add(referencedProjectId, referencedByProjects);
                }

                referencedByProjects.Add(project);
            }
        }

        return projects
            .OrderBy(project => project.Name, StringComparer.OrdinalIgnoreCase)
            .Select(
                project => new ProjectInventoryItem(
                    project,
                    project.ProjectReferences
                        .Select(projectId => CreateProjectLink(projectId, projectsById))
                        .Where(link => link is not null)
                        .Cast<ProjectLinkItem>()
                        .OrderBy(link => link.ProjectName, StringComparer.OrdinalIgnoreCase)
                        .ToArray(),
                    referencedByLookup.TryGetValue(project.ProjectId, out var referencedByProjects)
                        ? referencedByProjects
                            .OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
                            .Select(item => new ProjectLinkItem(item.ProjectId, item.Name, item.Path))
                            .ToArray()
                        : [],
                    includeDocuments && documentsByProjectId.TryGetValue(project.ProjectId, out var projectDocuments)
                        ? projectDocuments
                        : []))
            .ToArray();
    }

    private static ProjectInventoryItem? ResolveProjectInventoryItem(
        IReadOnlyList<ProjectInventoryItem> projects,
        ProjectInventoryQuery query) {
        if (!string.IsNullOrWhiteSpace(query.ProjectId)) {
            return projects.FirstOrDefault(
                item => string.Equals(item.Project.ProjectId, query.ProjectId.Trim(), StringComparison.Ordinal));
        }

        if (!string.IsNullOrWhiteSpace(query.ProjectName)) {
            return projects.FirstOrDefault(
                item => string.Equals(item.Project.Name, query.ProjectName.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        return null;
    }

    private static ProjectLinkItem? CreateProjectLink(
        string projectId,
        IReadOnlyDictionary<string, ProjectFact> projectsById) {
        return projectsById.TryGetValue(projectId, out var project)
            ? new ProjectLinkItem(project.ProjectId, project.Name, project.Path)
            : null;
    }
}
