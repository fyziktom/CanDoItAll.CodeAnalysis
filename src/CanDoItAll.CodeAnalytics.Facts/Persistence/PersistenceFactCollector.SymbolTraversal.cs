using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Domain.Sources;
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
        AnalysisRequest request,
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
                            Source = CreateSourceReference(property, request),
                        }))
            .Where(candidate => knownTypeDisplayNames.Contains(candidate.DisplayName))
            .Where(candidate => knownEntityDisplayNames.Contains(candidate.DisplayName))
            .Select(
                candidate => entityIdsByIdentity.TryGetValue((entityType.ProjectId, candidate.DisplayName), out var targetId)
                    ? new EntityNavigationCandidate(entityId, targetId, candidate.IsCollection, candidate.Name, candidate.Source)
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

    private static SourceReference? CreateSourceReference(ISymbol symbol, AnalysisRequest request) {
        var location = symbol.Locations.FirstOrDefault(candidate => candidate.IsInSource && candidate.SourceTree?.FilePath is not null);
        if (location is null) {
            return null;
        }

        var lineSpan = location.GetLineSpan();
        var solutionDirectory = Path.GetDirectoryName(request.SolutionPath)!;
        return new SourceReference(
            Path.GetRelativePath(solutionDirectory, lineSpan.Path).Replace('\\', '/'),
            lineSpan.StartLinePosition.Line + 1,
            lineSpan.StartLinePosition.Character + 1);
    }
}
