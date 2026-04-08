using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using Microsoft.CodeAnalysis;

namespace CanDoItAll.CodeAnalytics.Facts.Symbols;

public sealed partial class SymbolFactsCollector {
    private static IEnumerable<INamedTypeSymbol> EnumerateTypes(INamespaceSymbol namespaceSymbol) {
        foreach (var type in namespaceSymbol.GetTypeMembers()) {
            if (type.Locations.Any(location => location.IsInSource)) {
                yield return type;
            }

            foreach (var nested in EnumerateNestedTypes(type)) {
                yield return nested;
            }
        }

        foreach (var childNamespace in namespaceSymbol.GetNamespaceMembers()) {
            foreach (var type in EnumerateTypes(childNamespace)) {
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

    private static bool ShouldIncludeProject(AnalysisRequest request, ProjectFact project) {
        if (request.ScopeProjectNames.Count == 0) {
            return true;
        }

        return request.ScopeProjectNames.Contains(project.Name, StringComparer.OrdinalIgnoreCase);
    }

    private static bool ShouldIncludeType(AnalysisRequest request, ProjectFact project, INamedTypeSymbol symbol) {
        if (!ShouldIncludeProject(request, project)) {
            return false;
        }

        if (request.ScopeNamespacePrefixes.Count == 0) {
            return true;
        }

        var namespaceName = symbol.ContainingNamespace.IsGlobalNamespace
            ? project.Name
            : symbol.ContainingNamespace.ToDisplayString();
        return request.ScopeNamespacePrefixes.Any(
            prefix => namespaceName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsOwnedByProject(ISymbol symbol, ISet<string> projectDocumentPaths) {
        return symbol.Locations
            .Where(location => location.IsInSource && location.SourceTree?.FilePath is not null)
            .Select(location => Path.GetFullPath(location.SourceTree!.FilePath))
            .Any(projectDocumentPaths.Contains);
    }
}
