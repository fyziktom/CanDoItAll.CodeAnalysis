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
        var aliases = selectedTypes.ToDictionary(type => type.TypeId, type => type.TypeId.Replace('-', '_'), StringComparer.Ordinal);

        var builder = new StringBuilder();
        builder.AppendLine("classDiagram");

        foreach (var type in selectedTypes) {
            builder.AppendLine($"    class \"{type.DisplayName}\" as {aliases[type.TypeId]}");
        }

        foreach (var type in selectedTypes) {
            foreach (var iface in type.InterfaceDisplayNames.OrderBy(value => value, StringComparer.Ordinal)) {
                var target = selectedTypes.FirstOrDefault(candidate => string.Equals(candidate.DisplayName, iface, StringComparison.Ordinal));
                if (target is not null) {
                    builder.AppendLine($"    {aliases[type.TypeId]} ..|> {aliases[target.TypeId]}");
                }
            }

            if (type.BaseTypeDisplayName is null) {
                continue;
            }

            var baseType = selectedTypes.FirstOrDefault(candidate => string.Equals(candidate.DisplayName, type.BaseTypeDisplayName, StringComparison.Ordinal));
            if (baseType is not null) {
                builder.AppendLine($"    {aliases[type.TypeId]} --|> {aliases[baseType.TypeId]}");
            }
        }

        return builder.ToString().TrimEnd() + Environment.NewLine;
    }
}
