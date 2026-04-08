using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Identifiers;
using CanDoItAll.CodeAnalytics.Facts.Symbols;
using CanDoItAll.CodeAnalytics.Workspace.Loading;

namespace CanDoItAll.CodeAnalytics.Facts.Dependencies;

public sealed partial class DependencyFactCollector {
    private static IReadOnlyList<ModuleFact> BuildModules(WorkspaceLoadResult workspace, SymbolCollectionResult symbols) {
        var projectById = workspace.Projects.ToDictionary(project => project.ProjectId, StringComparer.Ordinal);
        return symbols.Namespaces
            .GroupBy(item => new { item.ModuleId, item.ProjectId })
            .Select(
                group => {
                    var project = projectById[group.Key.ProjectId];
                    var namespaceNames = group.Select(item => item.Name)
                        .OrderBy(value => value, StringComparer.Ordinal)
                        .ToArray();
                    var moduleName = ModuleNameClassifier.GetModuleName(project.Name, namespaceNames[0]);

                    return new ModuleFact(
                        group.Key.ModuleId,
                        group.Key.ProjectId,
                        moduleName,
                        moduleName,
                        group.Select(item => item.NamespaceId).OrderBy(value => value, StringComparer.Ordinal).ToArray(),
                        group.SelectMany(item => item.TypeIds).Distinct(StringComparer.Ordinal).OrderBy(value => value, StringComparer.Ordinal).ToArray());
                })
            .OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static IReadOnlyList<DependencyEdgeFact> BuildDependencyFacts(
        IReadOnlyDictionary<(DependencyKind Kind, string FromId, string ToId), int> edges) {
        return edges
            .OrderBy(item => item.Key.Kind)
            .ThenBy(item => item.Key.FromId, StringComparer.Ordinal)
            .ThenBy(item => item.Key.ToId, StringComparer.Ordinal)
            .Select(
                item => new DependencyEdgeFact(
                    StableId.ForDependency($"{item.Key.Kind}:{item.Key.FromId}:{item.Key.ToId}"),
                    item.Key.Kind,
                    item.Key.FromId,
                    item.Key.ToId,
                    item.Value))
            .ToArray();
    }

    private static IReadOnlyList<TypeRelationshipFact> BuildTypeRelationships(
        IReadOnlyDictionary<(TypeRelationshipKind Kind, string FromTypeId, string ToTypeId), TypeRelationshipAggregate> relationshipWeights) {
        return relationshipWeights
            .OrderBy(item => item.Key.Kind)
            .ThenBy(item => item.Key.FromTypeId, StringComparer.Ordinal)
            .ThenBy(item => item.Key.ToTypeId, StringComparer.Ordinal)
            .Select(
                item => new TypeRelationshipFact(
                    StableId.ForTypeRelationship($"{item.Key.Kind}:{item.Key.FromTypeId}:{item.Key.ToTypeId}"),
                    item.Key.FromTypeId,
                    item.Key.ToTypeId,
                    item.Key.Kind,
                    item.Value.Weight,
                    item.Value.Source))
            .ToArray();
    }

    private static void AddEdge(
        IDictionary<(DependencyKind Kind, string FromId, string ToId), int> edges,
        DependencyKind kind,
        string fromId,
        string toId) {
        var key = (kind, fromId, toId);
        if (edges.TryGetValue(key, out var existing)) {
            edges[key] = existing + 1;
            return;
        }

        edges[key] = 1;
    }

    private static void AddTypeRelationship(
        IDictionary<(TypeRelationshipKind Kind, string FromTypeId, string ToTypeId), TypeRelationshipAggregate> relationshipWeights,
        TypeRelationshipKind kind,
        string fromTypeId,
        string toTypeId,
        CanDoItAll.CodeAnalytics.Domain.Sources.SourceReference? source) {
        var key = (kind, fromTypeId, toTypeId);
        if (relationshipWeights.TryGetValue(key, out var existing)) {
            relationshipWeights[key] = existing with {
                Weight = existing.Weight + 1,
            };
            return;
        }

        relationshipWeights[key] = new TypeRelationshipAggregate(1, source);
    }

    private sealed record TypeRelationshipAggregate(int Weight, CanDoItAll.CodeAnalytics.Domain.Sources.SourceReference? Source);
}
