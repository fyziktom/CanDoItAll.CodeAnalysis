using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Identifiers;
using CanDoItAll.CodeAnalytics.Domain.Sources;
using Microsoft.CodeAnalysis;

namespace CanDoItAll.CodeAnalytics.Facts.Persistence;

public sealed partial class PersistenceFactCollector {
    private static IReadOnlyList<EntityRelationshipFact> BuildEntityRelationships(
        IReadOnlyList<EntityNavigationCandidate> navigationCandidates,
        IReadOnlyDictionary<string, EntityFact> entitiesById) {
        return navigationCandidates
            .GroupBy(
                candidate => CreateEntityPairKey(candidate.FromEntityId, candidate.ToEntityId),
                StringComparer.Ordinal)
            .Select(group => CreateEntityRelationshipFact(group, entitiesById))
            .OrderBy(item => item.Kind)
            .ThenBy(item => item.FromEntityId, StringComparer.Ordinal)
            .ThenBy(item => item.ToEntityId, StringComparer.Ordinal)
            .ToArray();
    }

    private static IReadOnlyList<EntityRelationshipFact> MergeEntityRelationships(
        IReadOnlyList<EntityRelationshipFact> navigationRelationships,
        IReadOnlyList<EntityRelationshipFact> configuredRelationships) {
        return navigationRelationships
            .Concat(configuredRelationships)
            .GroupBy(item => CreateEntityPairKey(item.FromEntityId, item.ToEntityId), StringComparer.Ordinal)
            .Select(
                group => {
                    var preferred = group
                        .OrderByDescending(item => GetRelationshipPriority(item.Kind))
                        .ThenBy(item => item.FromEntityId, StringComparer.Ordinal)
                        .ThenBy(item => item.ToEntityId, StringComparer.Ordinal)
                        .First();
                    var navigationNames = group
                        .SelectMany(item => item.NavigationPropertyNames)
                        .Distinct(StringComparer.Ordinal)
                        .OrderBy(value => value, StringComparer.Ordinal)
                        .ToArray();

                    return preferred with {
                        NavigationPropertyNames = navigationNames,
                    };
                })
            .OrderBy(item => item.Kind)
            .ThenBy(item => item.FromEntityId, StringComparer.Ordinal)
            .ThenBy(item => item.ToEntityId, StringComparer.Ordinal)
            .ToArray();
    }

    private static EntityRelationshipFact? CreateConfiguredEntityRelationshipFact(
        ConfiguredEntityRelationshipMapping mapping,
        IReadOnlyDictionary<(string ProjectId, string DisplayName), string> entityIdsByIdentity,
        IReadOnlyDictionary<string, EntityFact> entitiesById) {
        if (!entityIdsByIdentity.TryGetValue((mapping.ProjectId, mapping.FromEntityDisplayName), out var fromEntityId) ||
            !entityIdsByIdentity.TryGetValue((mapping.ProjectId, mapping.ToEntityDisplayName), out var toEntityId) ||
            !entitiesById.ContainsKey(fromEntityId) ||
            !entitiesById.ContainsKey(toEntityId)) {
            return null;
        }

        var (orderedFromEntityId, orderedToEntityId) = mapping.Kind == EntityRelationshipKind.OneToMany || mapping.Kind == EntityRelationshipKind.Reference
            ? (fromEntityId, toEntityId)
            : OrderEntityPair(fromEntityId, toEntityId, entitiesById);
        return new EntityRelationshipFact(
            StableId.ForEntityRelationship($"{mapping.Kind}:{orderedFromEntityId}:{orderedToEntityId}"),
            orderedFromEntityId,
            orderedToEntityId,
            mapping.Kind,
            mapping.NavigationPropertyNames,
            mapping.Source);
    }

    private static EntityRelationshipFact CreateEntityRelationshipFact(
        IGrouping<string, EntityNavigationCandidate> group,
        IReadOnlyDictionary<string, EntityFact> entitiesById) {
        var pair = group.Key.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var leftEntityId = pair[0];
        var rightEntityId = pair[1];

        var leftToRightCollection = group
            .Where(candidate => string.Equals(candidate.FromEntityId, leftEntityId, StringComparison.Ordinal) &&
                string.Equals(candidate.ToEntityId, rightEntityId, StringComparison.Ordinal) &&
                candidate.IsCollection)
            .ToArray();
        var leftToRightReference = group
            .Where(candidate => string.Equals(candidate.FromEntityId, leftEntityId, StringComparison.Ordinal) &&
                string.Equals(candidate.ToEntityId, rightEntityId, StringComparison.Ordinal) &&
                !candidate.IsCollection)
            .ToArray();
        var rightToLeftCollection = group
            .Where(candidate => string.Equals(candidate.FromEntityId, rightEntityId, StringComparison.Ordinal) &&
                string.Equals(candidate.ToEntityId, leftEntityId, StringComparison.Ordinal) &&
                candidate.IsCollection)
            .ToArray();
        var rightToLeftReference = group
            .Where(candidate => string.Equals(candidate.FromEntityId, rightEntityId, StringComparison.Ordinal) &&
                string.Equals(candidate.ToEntityId, leftEntityId, StringComparison.Ordinal) &&
                !candidate.IsCollection)
            .ToArray();

        if (leftToRightCollection.Length > 0 && rightToLeftCollection.Length > 0) {
            return CreateEntityRelationshipFact(
                leftEntityId,
                rightEntityId,
                EntityRelationshipKind.ManyToMany,
                leftToRightCollection.Concat(rightToLeftCollection),
                entitiesById);
        }

        if (leftToRightCollection.Length > 0) {
            return CreateEntityRelationshipFact(
                leftEntityId,
                rightEntityId,
                EntityRelationshipKind.OneToMany,
                leftToRightCollection.Concat(rightToLeftReference),
                entitiesById);
        }

        if (rightToLeftCollection.Length > 0) {
            return CreateEntityRelationshipFact(
                rightEntityId,
                leftEntityId,
                EntityRelationshipKind.OneToMany,
                rightToLeftCollection.Concat(leftToRightReference),
                entitiesById);
        }

        if (leftToRightReference.Length > 0 && rightToLeftReference.Length > 0) {
            var (fromEntityId, toEntityId) = OrderEntityPair(leftEntityId, rightEntityId, entitiesById);
            return CreateEntityRelationshipFact(
                fromEntityId,
                toEntityId,
                EntityRelationshipKind.OneToOne,
                leftToRightReference.Concat(rightToLeftReference),
                entitiesById);
        }

        if (leftToRightReference.Length > 0) {
            return CreateEntityRelationshipFact(
                leftEntityId,
                rightEntityId,
                EntityRelationshipKind.Reference,
                leftToRightReference,
                entitiesById);
        }

        return CreateEntityRelationshipFact(
            rightEntityId,
            leftEntityId,
            EntityRelationshipKind.Reference,
            rightToLeftReference,
            entitiesById);
    }

    private static EntityRelationshipFact CreateEntityRelationshipFact(
        string fromEntityId,
        string toEntityId,
        EntityRelationshipKind kind,
        IEnumerable<EntityNavigationCandidate> candidates,
        IReadOnlyDictionary<string, EntityFact> entitiesById) {
        var navigationNames = candidates
            .Select(candidate => candidate.PropertyName)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();
        var (orderedFromEntityId, orderedToEntityId) = kind == EntityRelationshipKind.OneToMany || kind == EntityRelationshipKind.Reference
            ? (fromEntityId, toEntityId)
            : OrderEntityPair(fromEntityId, toEntityId, entitiesById);

        return new EntityRelationshipFact(
            StableId.ForEntityRelationship($"{kind}:{orderedFromEntityId}:{orderedToEntityId}"),
            orderedFromEntityId,
            orderedToEntityId,
            kind,
            navigationNames,
            candidates.Select(candidate => candidate.Source).FirstOrDefault(source => source is not null));
    }

    private static (string FromEntityId, string ToEntityId) OrderEntityPair(
        string leftEntityId,
        string rightEntityId,
        IReadOnlyDictionary<string, EntityFact> entitiesById) {
        var leftName = entitiesById[leftEntityId].DisplayName;
        var rightName = entitiesById[rightEntityId].DisplayName;
        return string.Compare(leftName, rightName, StringComparison.Ordinal) <= 0
            ? (leftEntityId, rightEntityId)
            : (rightEntityId, leftEntityId);
    }

    private static string CreateEntityPairKey(string leftEntityId, string rightEntityId) {
        return string.Compare(leftEntityId, rightEntityId, StringComparison.Ordinal) <= 0
            ? $"{leftEntityId}|{rightEntityId}"
            : $"{rightEntityId}|{leftEntityId}";
    }

    private static IEnumerable<(INamedTypeSymbol Symbol, bool IsCollection)> ExpandEntityNavigationTargets(ITypeSymbol? typeSymbol) {
        if (typeSymbol is null) {
            yield break;
        }

        switch (typeSymbol) {
            case IArrayTypeSymbol arrayType when arrayType.ElementType is INamedTypeSymbol arrayElement:
                yield return (arrayElement, true);
                yield break;
            case INamedTypeSymbol namedType when namedType.SpecialType != SpecialType.None:
                yield break;
            case INamedTypeSymbol namedType when string.Equals(namedType.Name, "String", StringComparison.Ordinal):
                yield break;
            case INamedTypeSymbol namedType when IsCollectionType(namedType):
                foreach (var typeArgument in namedType.TypeArguments.OfType<INamedTypeSymbol>()) {
                    yield return (typeArgument, true);
                }

                break;
            case INamedTypeSymbol namedType:
                yield return (namedType, false);
                break;
        }
    }

    private static bool IsCollectionType(INamedTypeSymbol namedType) {
        return namedType.AllInterfaces.Any(
            iface => iface.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T);
    }

    private static int GetRelationshipPriority(EntityRelationshipKind kind) {
        return kind switch {
            EntityRelationshipKind.ManyToMany => 4,
            EntityRelationshipKind.OneToMany => 3,
            EntityRelationshipKind.OneToOne => 2,
            EntityRelationshipKind.Reference => 1,
            _ => 0,
        };
    }

    private sealed record EntityNavigationCandidate(
        string FromEntityId,
        string ToEntityId,
        bool IsCollection,
        string PropertyName,
        SourceReference? Source);
}
