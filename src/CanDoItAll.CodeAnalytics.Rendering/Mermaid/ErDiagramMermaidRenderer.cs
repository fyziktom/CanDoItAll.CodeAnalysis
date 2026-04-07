using System.Text;
using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Rendering.Mermaid;

public sealed class ErDiagramMermaidRenderer {
    public string Render(IReadOnlyList<EntityFact> entities, int maxNodes) {
        var selectedEntities = entities.OrderBy(entity => entity.DisplayName, StringComparer.Ordinal).Take(maxNodes).ToArray();
        var entityIds = selectedEntities.Select(entity => entity.EntityId).ToHashSet(StringComparer.Ordinal);

        var builder = new StringBuilder();
        builder.AppendLine("erDiagram");

        foreach (var entity in selectedEntities) {
            builder.AppendLine($"    {entity.DisplayName} {{");
            foreach (var key in entity.KeyPropertyNames.DefaultIfEmpty("Id")) {
                builder.AppendLine($"        int {key} PK");
            }

            builder.AppendLine("    }");
        }

        foreach (var entity in selectedEntities) {
            foreach (var targetId in entity.RelationshipTargets.OrderBy(value => value, StringComparer.Ordinal)) {
                if (!entityIds.Contains(targetId)) {
                    continue;
                }

                var target = selectedEntities.First(candidate => string.Equals(candidate.EntityId, targetId, StringComparison.Ordinal));
                builder.AppendLine($"    {entity.DisplayName} ||--o{{ {target.DisplayName} : relates");
            }
        }

        return builder.ToString().TrimEnd() + Environment.NewLine;
    }
}
