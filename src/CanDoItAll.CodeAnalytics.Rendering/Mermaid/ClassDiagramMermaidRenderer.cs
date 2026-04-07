using System.Text;
using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Rendering.Mermaid;

public sealed class ClassDiagramMermaidRenderer {
    public string Render(IReadOnlyList<TypeFact> types, int maxNodes) {
        var selectedTypes = types
            .GroupBy(type => type.TypeId, StringComparer.Ordinal)
            .Select(group => group.First())
            .OrderBy(type => type.DisplayName, StringComparer.Ordinal)
            .Take(maxNodes)
            .ToArray();
        var aliasesByTypeId = selectedTypes
            .Select((type, index) => new { type.TypeId, Alias = $"T{index + 1:D4}" })
            .ToDictionary(item => item.TypeId, item => item.Alias, StringComparer.Ordinal);
        var typesByDisplayName = selectedTypes
            .GroupBy(type => type.DisplayName, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.ToArray(), StringComparer.Ordinal);

        var builder = new StringBuilder();
        builder.AppendLine("classDiagram");

        foreach (var type in selectedTypes) {
            builder.AppendLine($"    class {aliasesByTypeId[type.TypeId]}[\"{EscapeLabel(type.DisplayName)}\"]");
        }

        foreach (var type in selectedTypes) {
            foreach (var interfaceDisplayName in type.InterfaceDisplayNames.OrderBy(value => value, StringComparer.Ordinal)) {
                var target = ResolveRelationshipTarget(type, interfaceDisplayName, typesByDisplayName);
                if (target is not null) {
                    builder.AppendLine($"    {aliasesByTypeId[target.TypeId]} <|.. {aliasesByTypeId[type.TypeId]}");
                }
            }

            if (type.BaseTypeDisplayName is null) {
                continue;
            }

            var baseType = ResolveRelationshipTarget(type, type.BaseTypeDisplayName, typesByDisplayName);
            if (baseType is not null) {
                builder.AppendLine($"    {aliasesByTypeId[baseType.TypeId]} <|-- {aliasesByTypeId[type.TypeId]}");
            }
        }

        return builder.ToString().TrimEnd() + Environment.NewLine;
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

    private static string EscapeLabel(string value) {
        return value
            .Replace("&", "&amp;", StringComparison.Ordinal)
            .Replace("\"", "&quot;", StringComparison.Ordinal)
            .Replace("<", "&lt;", StringComparison.Ordinal)
            .Replace(">", "&gt;", StringComparison.Ordinal);
    }
}
