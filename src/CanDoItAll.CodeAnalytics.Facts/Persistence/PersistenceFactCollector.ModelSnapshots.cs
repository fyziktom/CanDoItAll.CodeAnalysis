using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Workspace.Loading;
using Microsoft.CodeAnalysis;

namespace CanDoItAll.CodeAnalytics.Facts.Persistence;

public sealed partial class PersistenceFactCollector {
    private async Task<IReadOnlyList<ModelSnapshotDiscovery>> BuildModelSnapshotDiscoveriesAsync(
        WorkspaceLoadResult workspace,
        ICollection<AnalysisDiagnostic> diagnostics,
        CancellationToken cancellationToken) {
        var discoveries = new List<ModelSnapshotDiscovery>();

        foreach (var projectContext in workspace.ProjectContexts.OrderBy(context => context.Fact.Name, StringComparer.OrdinalIgnoreCase)) {
            var compilation = await projectContext.Project.GetCompilationAsync(cancellationToken);
            if (compilation is null) {
                continue;
            }

            var projectDocumentPaths = projectContext.Project.Documents
                .Where(document => !string.IsNullOrWhiteSpace(document.FilePath))
                .Select(document => Path.GetFullPath(document.FilePath!))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var symbol in EnumerateTypes(compilation.GlobalNamespace).Where(item => IsOwnedByProject(item, projectDocumentPaths))) {
                if (!PersistenceSyntaxExplorer.TryDiscoverModelSnapshot(
                        symbol,
                        compilation,
                        workspace.Request,
                        cancellationToken,
                        out var discovery)) {
                    continue;
                }

                discoveries.Add(discovery);
            }
        }

        foreach (var duplicate in discoveries
            .GroupBy(item => item.DbContextDisplayName, StringComparer.Ordinal)
            .Where(group => group.Count() > 1)) {
            diagnostics.Add(
                new AnalysisDiagnostic(
                    "EF0007",
                    AnalysisDiagnosticSeverity.Info,
                    $"Multiple model snapshots were discovered for DbContext {duplicate.Key}. Their metadata was merged."));
        }

        return discoveries
            .OrderBy(item => item.DbContextDisplayName, StringComparer.Ordinal)
            .ThenBy(item => item.Source?.Path, StringComparer.Ordinal)
            .ToArray();
    }

    private static ModelSnapshotDiscovery? ResolveModelSnapshotDiscovery(
        IReadOnlyList<ModelSnapshotDiscovery> discoveries,
        string dbContextDisplayName) {
        var matches = discoveries
            .Where(item => string.Equals(item.DbContextDisplayName, dbContextDisplayName, StringComparison.Ordinal))
            .ToArray();
        if (matches.Length == 0) {
            return null;
        }

        return new ModelSnapshotDiscovery(
            dbContextDisplayName,
            matches
                .SelectMany(item => item.EntityDisplayNames)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(item => item, StringComparer.Ordinal)
                .ToArray(),
            matches
                .SelectMany(item => item.StoreObjectMappings)
                .GroupBy(item => item.EntityDisplayName, StringComparer.Ordinal)
                .Select(group => group.First())
                .OrderBy(item => item.EntityDisplayName, StringComparer.Ordinal)
                .ToArray(),
            matches
                .SelectMany(item => item.RelationshipMappings)
                .GroupBy(item => (item.FromEntityDisplayName, item.ToEntityDisplayName, item.Kind), EqualityComparer<(string FromEntityDisplayName, string ToEntityDisplayName, EntityRelationshipKind Kind)>.Default)
                .Select(group => group.First())
                .OrderBy(item => item.FromEntityDisplayName, StringComparer.Ordinal)
                .ThenBy(item => item.ToEntityDisplayName, StringComparer.Ordinal)
                .ThenBy(item => item.Kind)
                .ToArray(),
            matches.Select(item => item.Source).FirstOrDefault(item => item is not null));
    }

    private static IReadOnlyList<ConfiguredEntityRelationshipMapping> CreateModelSnapshotRelationshipMappings(
        ModelSnapshotDiscovery discovery,
        string projectId) {
        return discovery.RelationshipMappings
            .Select(
                item => new ConfiguredEntityRelationshipMapping(
                    projectId,
                    item.FromEntityDisplayName,
                    item.ToEntityDisplayName,
                    item.Kind,
                    item.NavigationPropertyNames,
                    item.Source))
            .ToArray();
    }
}
