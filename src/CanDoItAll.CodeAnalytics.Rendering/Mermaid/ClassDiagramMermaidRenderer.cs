using System.Text;
using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Rendering.Mermaid;

public sealed class ClassDiagramMermaidRenderer {
    public string Render(
        IReadOnlyList<TypeFact> types,
        IReadOnlyList<TypeRelationshipFact> relationships,
        int maxNodes) {
        var selectedTypes = SelectTypes(types, relationships, maxNodes);
        var selectedTypeIds = selectedTypes.Select(type => type.TypeId).ToHashSet(StringComparer.Ordinal);
        var selectedRelationships = relationships
            .Where(item => selectedTypeIds.Contains(item.FromTypeId) && selectedTypeIds.Contains(item.ToTypeId))
            .ToArray();
        var aliasesByTypeId = selectedTypes
            .Select((type, index) => new { type.TypeId, Alias = $"T{index + 1:D4}" })
            .ToDictionary(item => item.TypeId, item => item.Alias, StringComparer.Ordinal);
        var typesByResolvedDisplayName = selectedTypes
            .GroupBy(type => type.DisplayName, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.ToArray(), StringComparer.Ordinal);
        var emittedEdges = new HashSet<string>(StringComparer.Ordinal);

        var builder = new StringBuilder();
        builder.AppendLine("classDiagram");

        foreach (var type in selectedTypes) {
            builder.AppendLine($"    class {aliasesByTypeId[type.TypeId]}[\"{EscapeLabel(type.DisplayName)}\"]");
        }

        foreach (var type in selectedTypes) {
            foreach (var interfaceDisplayName in type.InterfaceDisplayNames.OrderBy(value => value, StringComparer.Ordinal)) {
                var target = ResolveRelationshipTarget(type, interfaceDisplayName, typesByResolvedDisplayName);
                if (target is not null) {
                    AppendEdge(builder, emittedEdges, $"{target.TypeId}:implements:{type.TypeId}", $"{aliasesByTypeId[target.TypeId]} <|.. {aliasesByTypeId[type.TypeId]}");
                }
            }

            if (type.BaseTypeDisplayName is null) {
                continue;
            }

            var baseType = ResolveRelationshipTarget(type, type.BaseTypeDisplayName, typesByResolvedDisplayName);
            if (baseType is not null) {
                AppendEdge(builder, emittedEdges, $"{baseType.TypeId}:inherits:{type.TypeId}", $"{aliasesByTypeId[baseType.TypeId]} <|-- {aliasesByTypeId[type.TypeId]}");
            }
        }

        foreach (var relationshipGroup in selectedRelationships
            .GroupBy(
                item => new {
                    item.FromTypeId,
                    item.ToTypeId,
                    IsAssociation = IsAssociation(item.Kind),
                })
            .OrderBy(group => group.Key.FromTypeId, StringComparer.Ordinal)
            .ThenBy(group => group.Key.ToTypeId, StringComparer.Ordinal)) {
            var line = relationshipGroup.Key.IsAssociation ? "-->" : "..>";
            var label = BuildRelationshipLabel(relationshipGroup.Select(item => item.Kind));
            AppendEdge(
                builder,
                emittedEdges,
                $"{relationshipGroup.Key.FromTypeId}:{line}:{relationshipGroup.Key.ToTypeId}:{label}",
                $"{aliasesByTypeId[relationshipGroup.Key.FromTypeId]} {line} {aliasesByTypeId[relationshipGroup.Key.ToTypeId]} : {label}");
        }

        return builder.ToString().TrimEnd() + Environment.NewLine;
    }

    private static IReadOnlyList<TypeFact> SelectTypes(
        IReadOnlyList<TypeFact> types,
        IReadOnlyList<TypeRelationshipFact> relationships,
        int maxNodes) {
        var relationshipScores = relationships
            .SelectMany(item => new[] { item.FromTypeId, item.ToTypeId })
            .GroupBy(item => item, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.Ordinal);

        return types
            .GroupBy(type => type.TypeId, StringComparer.Ordinal)
            .Select(group => group.First())
            .OrderByDescending(
                type => relationshipScores.TryGetValue(type.TypeId, out var score)
                    ? score + type.InterfaceDisplayNames.Count + (type.BaseTypeDisplayName is null ? 0 : 1)
                    : type.InterfaceDisplayNames.Count + (type.BaseTypeDisplayName is null ? 0 : 1))
            .ThenBy(type => type.DisplayName, StringComparer.Ordinal)
            .Take(maxNodes)
            .ToArray();
    }

    private static TypeFact? ResolveRelationshipTarget(
        TypeFact currentType,
        string targetDisplayName,
        IReadOnlyDictionary<string, TypeFact[]> typesByDisplayName) {
        if (!typesByDisplayName.TryGetValue(targetDisplayName, out var candidates) || candidates.Length == 0) {
            return null;
        }

        if (candidates.Length == 1) {
            return candidates[0];
        }

        return candidates.Count(candidate => string.Equals(candidate.ProjectId, currentType.ProjectId, StringComparison.Ordinal)) == 1
            ? candidates.Single(candidate => string.Equals(candidate.ProjectId, currentType.ProjectId, StringComparison.Ordinal))
            : null;
    }

    private static bool IsAssociation(TypeRelationshipKind kind) {
        return kind == TypeRelationshipKind.Field || kind == TypeRelationshipKind.Property;
    }

    private static string BuildRelationshipLabel(IEnumerable<TypeRelationshipKind> kinds) {
        var labels = kinds
            .Distinct()
            .OrderBy(kind => kind)
            .Select(
                kind => kind switch {
                    TypeRelationshipKind.Field => "field",
                    TypeRelationshipKind.Property => "property",
                    TypeRelationshipKind.Event => "event",
                    TypeRelationshipKind.ConstructorParameter => "ctor",
                    TypeRelationshipKind.MethodParameter => "param",
                    TypeRelationshipKind.MethodReturn => "returns",
                    _ => "uses",
                })
            .ToArray();

        return string.Join("/", labels);
    }

    private static void AppendEdge(
        StringBuilder builder,
        ISet<string> emittedEdges,
        string key,
        string content) {
        if (!emittedEdges.Add(key)) {
            return;
        }

        builder.AppendLine($"    {content}");
    }

    private static string EscapeLabel(string value) {
        return value
            .Replace("&", "&amp;", StringComparison.Ordinal)
            .Replace("\"", "&quot;", StringComparison.Ordinal)
            .Replace("<", "&lt;", StringComparison.Ordinal)
            .Replace(">", "&gt;", StringComparison.Ordinal);
    }
}
