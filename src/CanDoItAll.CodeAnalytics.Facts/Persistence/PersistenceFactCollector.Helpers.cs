using CanDoItAll.CodeAnalytics.Domain.Facts;
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
        ISet<string> knownTypeDisplayNames,
        ISet<string> knownEntityDisplayNames,
        IReadOnlyDictionary<(string ProjectId, string DisplayName), string> entityIdsByIdentity,
        EntityStoreObjectMapping? storeObjectMapping) {
        var relationshipTargets = entitySymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .SelectMany(property => ExpandEntityPropertyTypes(property.Type))
            .Select(candidate => candidate.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat))
            .Where(knownTypeDisplayNames.Contains)
            .Where(knownEntityDisplayNames.Contains)
            .Select(candidate => entityIdsByIdentity.TryGetValue((entityType.ProjectId, candidate), out var targetId) ? targetId : null)
            .Where(targetId => !string.IsNullOrWhiteSpace(targetId))
            .Cast<string>()
            .Distinct(StringComparer.Ordinal)
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();

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

    private static IEnumerable<INamedTypeSymbol> ExpandEntityPropertyTypes(ITypeSymbol? typeSymbol) {
        if (typeSymbol is null) {
            yield break;
        }

        switch (typeSymbol) {
            case INamedTypeSymbol namedType when namedType.SpecialType != SpecialType.None:
                yield break;
            case INamedTypeSymbol namedType when string.Equals(namedType.Name, "String", StringComparison.Ordinal):
                yield break;
            case INamedTypeSymbol namedType when namedType.IsGenericType:
                foreach (var typeArgument in namedType.TypeArguments.OfType<INamedTypeSymbol>()) {
                    yield return typeArgument;
                }

                break;
            case INamedTypeSymbol namedType:
                yield return namedType;
                break;
        }
    }
}
