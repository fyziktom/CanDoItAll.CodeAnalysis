using System.Text;
using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Rendering.Mermaid;

public sealed class ErDiagramMermaidRenderer {
    public string Render(
        IReadOnlyList<EntityFact> entities,
        IReadOnlyList<EntityRelationshipFact> relationships,
        int maxNodes) {
        var selectedEntities = SelectEntities(entities, relationships, maxNodes);
        var entityIds = selectedEntities.Select(entity => entity.EntityId).ToHashSet(StringComparer.Ordinal);
        var identifiersByEntityId = CreateIdentifiers(selectedEntities);

        var builder = new StringBuilder();
        builder.AppendLine("erDiagram");

        foreach (var entity in selectedEntities) {
            builder.AppendLine($"    {identifiersByEntityId[entity.EntityId]} {{");
            foreach (var key in entity.KeyPropertyNames.DefaultIfEmpty("Id")) {
                builder.AppendLine($"        string {key} PK");
            }

            builder.AppendLine("    }");
        }

        foreach (var relationship in relationships
            .Where(item => entityIds.Contains(item.FromEntityId) && entityIds.Contains(item.ToEntityId))
            .OrderBy(item => item.Kind)
            .ThenBy(item => item.FromEntityId, StringComparer.Ordinal)
            .ThenBy(item => item.ToEntityId, StringComparer.Ordinal)) {
            builder.AppendLine(
                $"    {identifiersByEntityId[relationship.FromEntityId]} {GetRelationshipSyntax(relationship.Kind)} {identifiersByEntityId[relationship.ToEntityId]} : {BuildRelationshipLabel(relationship)}");
        }

        return builder.ToString().TrimEnd() + Environment.NewLine;
    }

    private static IReadOnlyList<EntityFact> SelectEntities(
        IReadOnlyList<EntityFact> entities,
        IReadOnlyList<EntityRelationshipFact> relationships,
        int maxNodes) {
        var relationshipScores = relationships
            .SelectMany(item => new[] { item.FromEntityId, item.ToEntityId })
            .GroupBy(item => item, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.Ordinal);

        return entities
            .OrderByDescending(entity => relationshipScores.TryGetValue(entity.EntityId, out var score) ? score : 0)
            .ThenBy(entity => entity.DisplayName, StringComparer.Ordinal)
            .Take(maxNodes)
            .ToArray();
    }

    private static IReadOnlyDictionary<string, string> CreateIdentifiers(IReadOnlyList<EntityFact> entities) {
        var counts = new Dictionary<string, int>(StringComparer.Ordinal);
        var identifiers = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var entity in entities.OrderBy(item => item.DisplayName, StringComparer.Ordinal)) {
            var baseIdentifier = NormalizeIdentifier(entity.DisplayName);
            var currentCount = counts.TryGetValue(baseIdentifier, out var existingCount)
                ? existingCount + 1
                : 1;
            counts[baseIdentifier] = currentCount;
            identifiers[entity.EntityId] = currentCount == 1
                ? baseIdentifier
                : $"{baseIdentifier}_{currentCount}";
        }

        return identifiers;
    }

    private static string NormalizeIdentifier(string value) {
        var builder = new StringBuilder(value.Length);
        foreach (var character in value) {
            builder.Append(char.IsLetterOrDigit(character) ? character : '_');
        }

        var identifier = builder.ToString().Trim('_');
        if (string.IsNullOrWhiteSpace(identifier)) {
            return "Entity";
        }

        return char.IsDigit(identifier[0])
            ? $"Entity_{identifier}"
            : identifier;
    }

    private static string GetRelationshipSyntax(EntityRelationshipKind kind) {
        return kind switch {
            EntityRelationshipKind.OneToOne => "||--||",
            EntityRelationshipKind.OneToMany => "||--o{",
            EntityRelationshipKind.ManyToMany => "}o--o{",
            EntityRelationshipKind.Reference => "||--o|",
            _ => "||--o|",
        };
    }

    private static string BuildRelationshipLabel(EntityRelationshipFact relationship) {
        if (relationship.NavigationPropertyNames.Count > 0) {
            return string.Join("/", relationship.NavigationPropertyNames.Take(2));
        }

        return relationship.Kind switch {
            EntityRelationshipKind.OneToOne => "one_to_one",
            EntityRelationshipKind.OneToMany => "one_to_many",
            EntityRelationshipKind.ManyToMany => "many_to_many",
            _ => "references",
        };
    }
}
