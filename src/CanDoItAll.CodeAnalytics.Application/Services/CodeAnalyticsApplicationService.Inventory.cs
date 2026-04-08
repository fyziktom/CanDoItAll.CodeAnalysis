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
        var projectRolesById = projects.ToDictionary(project => project.ProjectId, ClassifyProjectRole, StringComparer.Ordinal);
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
            .Select(project => CreateProjectInventoryItem(project))
            .ToArray();

        ProjectInventoryItem CreateProjectInventoryItem(ProjectFact project) {
            var directProjectReferences = project.ProjectReferences
                .Select(projectId => CreateProjectLink(projectId, projectsById, projectRolesById))
                .Where(link => link is not null)
                .Cast<ProjectLinkItem>()
                .OrderBy(link => link.ProjectName, StringComparer.OrdinalIgnoreCase)
                .ToArray();
            var referencedByProjects = referencedByLookup.TryGetValue(project.ProjectId, out var callers)
                ? callers
                    .OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
                    .Select(item => new ProjectLinkItem(item.ProjectId, item.Name, item.Path, projectRolesById[item.ProjectId]))
                    .ToArray()
                : [];
            var productDirectProjectReferences = directProjectReferences
                .Where(static item => item.ProjectRole == ProjectRoleKind.Product)
                .ToArray();
            var supportingDirectProjectReferences = directProjectReferences
                .Where(static item => item.ProjectRole != ProjectRoleKind.Product)
                .ToArray();
            var productReferencedByProjects = referencedByProjects
                .Where(static item => item.ProjectRole == ProjectRoleKind.Product)
                .ToArray();
            var supportingReferencedByProjects = referencedByProjects
                .Where(static item => item.ProjectRole != ProjectRoleKind.Product)
                .ToArray();
            return new ProjectInventoryItem(
                project,
                projectRolesById[project.ProjectId],
                productDirectProjectReferences,
                supportingDirectProjectReferences,
                productReferencedByProjects,
                supportingReferencedByProjects,
                includeDocuments && documentsByProjectId.TryGetValue(project.ProjectId, out var projectDocuments)
                    ? projectDocuments
                    : []);
        }
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
        IReadOnlyDictionary<string, ProjectFact> projectsById,
        IReadOnlyDictionary<string, ProjectRoleKind> projectRolesById) {
        return projectsById.TryGetValue(projectId, out var project)
            ? new ProjectLinkItem(project.ProjectId, project.Name, project.Path, projectRolesById[project.ProjectId])
            : null;
    }

    private static ProjectRoleKind ClassifyProjectRole(ProjectFact project) {
        var normalizedName = project.Name;
        var normalizedPath = NormalizeProjectPath(project.Path);
        if (project.PackageReferences.Any(static package => string.Equals(package, "BenchmarkDotNet", StringComparison.OrdinalIgnoreCase))
            || normalizedPath.Contains("/benchmarks/", StringComparison.Ordinal)
            || normalizedName.Contains(".Benchmark", StringComparison.OrdinalIgnoreCase)) {
            return ProjectRoleKind.Benchmark;
        }

        if (project.PackageReferences.Any(static package => string.Equals(package, "Microsoft.NET.Test.Sdk", StringComparison.OrdinalIgnoreCase))
            || normalizedPath.Contains("/tests/", StringComparison.Ordinal)
            || normalizedName.Contains(".Test", StringComparison.OrdinalIgnoreCase)
            || normalizedName.EndsWith("Tests", StringComparison.OrdinalIgnoreCase)) {
            return ProjectRoleKind.Test;
        }

        return ProjectRoleKind.Product;
    }

    private static string NormalizeProjectPath(string path) {
        return path.Replace('\\', '/');
    }
}
