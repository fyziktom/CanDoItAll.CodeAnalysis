using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Identifiers;
using Microsoft.CodeAnalysis;

namespace CanDoItAll.CodeAnalytics.Facts.Persistence;

public sealed partial class PersistenceFactCollector {
    private static IEnumerable<INamedTypeSymbol> EnumerateTypes(INamespaceSymbol namespaceSymbol) {
        foreach (var type in namespaceSymbol.GetTypeMembers()) {
            if (type.Locations.Any(location => location.IsInSource)) {
                yield return type;
            }

            foreach (var nested in EnumerateNestedTypes(type)) {
                yield return nested;
            }
        }

        foreach (var child in namespaceSymbol.GetNamespaceMembers()) {
            foreach (var type in EnumerateTypes(child)) {
                yield return type;
            }
        }
    }

    private static IEnumerable<INamedTypeSymbol> EnumerateNestedTypes(INamedTypeSymbol typeSymbol) {
        foreach (var nestedType in typeSymbol.GetTypeMembers()) {
            if (nestedType.Locations.Any(location => location.IsInSource)) {
                yield return nestedType;
            }

            foreach (var child in EnumerateNestedTypes(nestedType)) {
                yield return child;
            }
        }
    }

    private static INamedTypeSymbol? ResolveDbSetEntityType(ITypeSymbol typeSymbol) {
        if (typeSymbol is not INamedTypeSymbol namedType) {
            return null;
        }

        if (!string.Equals(namedType.Name, "DbSet", StringComparison.Ordinal) ||
            !string.Equals(namedType.ContainingNamespace.ToDisplayString(), "Microsoft.EntityFrameworkCore", StringComparison.Ordinal)) {
            return null;
        }

        return namedType.TypeArguments[0] as INamedTypeSymbol;
    }

    private static EntityFact CreateEntityFact(
        string entityId,
        INamedTypeSymbol entitySymbol,
        TypeFact entityType,
        IReadOnlyList<string> relationshipTargets,
        EntityStoreObjectMapping? storeObjectMapping) {
        var keyProperties = entitySymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(property => string.Equals(property.Name, "Id", StringComparison.Ordinal) ||
                string.Equals(property.Name, $"{entitySymbol.Name}Id", StringComparison.Ordinal))
            .Select(property => property.Name)
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();

        return new EntityFact(
            entityId,
            entityType.TypeId,
            entityType.ProjectId,
            entityType.ModuleId,
            entitySymbol.Name,
            storeObjectMapping?.TableName,
            storeObjectMapping?.Schema,
            keyProperties,
            relationshipTargets,
            entityType.Source);
    }

    private static IReadOnlyList<EntityNavigationCandidate> CreateEntityRelationshipCandidates(
        string entityId,
        INamedTypeSymbol entitySymbol,
        TypeFact entityType,
        ISet<string> knownTypeDisplayNames,
        ISet<string> knownEntityDisplayNames,
        IReadOnlyDictionary<(string ProjectId, string DisplayName), string> entityIdsByIdentity) {
        return entitySymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .SelectMany(
                property => ExpandEntityNavigationTargets(property.Type)
                    .Select(
                        candidate => new {
                            DisplayName = candidate.Symbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
                            candidate.IsCollection,
                            property.Name,
                        }))
            .Where(candidate => knownTypeDisplayNames.Contains(candidate.DisplayName))
            .Where(candidate => knownEntityDisplayNames.Contains(candidate.DisplayName))
            .Select(
                candidate => entityIdsByIdentity.TryGetValue((entityType.ProjectId, candidate.DisplayName), out var targetId)
                    ? new EntityNavigationCandidate(entityId, targetId, candidate.IsCollection, candidate.Name)
                    : null)
            .Where(candidate => candidate is not null)
            .Cast<EntityNavigationCandidate>()
            .Where(candidate => !string.Equals(candidate.FromEntityId, candidate.ToEntityId, StringComparison.Ordinal))
            .GroupBy(candidate => new { candidate.FromEntityId, candidate.ToEntityId, candidate.IsCollection, candidate.PropertyName })
            .Select(group => group.First())
            .OrderBy(candidate => candidate.FromEntityId, StringComparer.Ordinal)
            .ThenBy(candidate => candidate.ToEntityId, StringComparer.Ordinal)
            .ThenBy(candidate => candidate.PropertyName, StringComparer.Ordinal)
            .ToArray();
    }

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
            mapping.NavigationPropertyNames);
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
            navigationNames);
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
        string PropertyName);
}
