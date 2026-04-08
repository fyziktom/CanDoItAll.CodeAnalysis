using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Domain.Sources;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace CanDoItAll.CodeAnalytics.Facts.Dependencies;

public sealed partial class DependencyFactCollector {
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

    private static IEnumerable<string> CollectDependencyTargetDisplayNames(
        INamedTypeSymbol typeSymbol,
        StringComparer? comparer = null) {
        var results = new HashSet<string>(comparer ?? StringComparer.Ordinal);

        foreach (var candidate in ExpandType(typeSymbol.BaseType)) {
            results.Add(candidate);
        }

        foreach (var iface in typeSymbol.Interfaces) {
            foreach (var candidate in ExpandType(iface)) {
                results.Add(candidate);
            }
        }

        foreach (var relationship in CollectTypeRelationships(typeSymbol)) {
            results.Add(relationship.TargetDisplayName);
        }

        return results.OrderBy(item => item, StringComparer.Ordinal);
    }

    private static IEnumerable<TypeRelationshipCandidate> CollectTypeRelationships(INamedTypeSymbol typeSymbol, AnalysisRequest? request = null) {
        var results = new HashSet<string>(StringComparer.Ordinal);
        var relationships = new List<TypeRelationshipCandidate>();

        foreach (var member in typeSymbol.GetMembers().Where(member => !member.IsImplicitlyDeclared)) {
            switch (member) {
                case IFieldSymbol field:
                    AddExpandedRelationships(relationships, results, field.Type, TypeRelationshipKind.Field, CreateSourceReference(field, request));
                    break;
                case IPropertySymbol property:
                    AddExpandedRelationships(relationships, results, property.Type, TypeRelationshipKind.Property, CreateSourceReference(property, request));
                    break;
                case IEventSymbol eventSymbol:
                    AddExpandedRelationships(relationships, results, eventSymbol.Type, TypeRelationshipKind.Event, CreateSourceReference(eventSymbol, request));
                    break;
                case IMethodSymbol method:
                    if (method.MethodKind == MethodKind.Constructor) {
                        foreach (var parameter in method.Parameters) {
                            AddExpandedRelationships(relationships, results, parameter.Type, TypeRelationshipKind.ConstructorParameter, CreateSourceReference(parameter, request));
                        }

                        break;
                    }

                    AddExpandedRelationships(relationships, results, method.ReturnType, TypeRelationshipKind.MethodReturn, CreateSourceReference(method, request));
                    foreach (var parameter in method.Parameters) {
                        AddExpandedRelationships(relationships, results, parameter.Type, TypeRelationshipKind.MethodParameter, CreateSourceReference(parameter, request));
                    }

                    break;
            }
        }

        return relationships
            .OrderBy(item => item.Kind)
            .ThenBy(item => item.TargetDisplayName, StringComparer.Ordinal)
            .ToArray();
    }

    private static void AddExpandedRelationships(
        ICollection<TypeRelationshipCandidate> relationships,
        ISet<string> seenKeys,
        ITypeSymbol? type,
        TypeRelationshipKind kind,
        SourceReference? source) {
        foreach (var candidate in ExpandType(type)) {
            var key = $"{kind}:{candidate}";
            if (!seenKeys.Add(key)) {
                continue;
            }

            relationships.Add(new TypeRelationshipCandidate(candidate, kind, source));
        }
    }

    private static IEnumerable<string> ExpandType(ITypeSymbol? type) {
        if (type is null) {
            yield break;
        }

        switch (type) {
            case INamedTypeSymbol namedType:
                yield return namedType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
                foreach (var typeArgument in namedType.TypeArguments) {
                    foreach (var nested in ExpandType(typeArgument)) {
                        yield return nested;
                    }
                }

                break;
            case IArrayTypeSymbol arrayType:
                foreach (var nested in ExpandType(arrayType.ElementType)) {
                    yield return nested;
                }

                break;
            case IPointerTypeSymbol pointerType:
                foreach (var nested in ExpandType(pointerType.PointedAtType)) {
                    yield return nested;
                }

                break;
        }
    }

    private bool TryResolveTypeFact(
        IReadOnlyDictionary<string, TypeFact[]> typeGroupsByDisplayName,
        string displayName,
        string projectId,
        ICollection<AnalysisDiagnostic> diagnostics,
        out TypeFact typeFact) {
        if (!typeGroupsByDisplayName.TryGetValue(displayName, out var candidates) || candidates.Length == 0) {
            typeFact = null!;
            return false;
        }

        var sameProjectCandidates = candidates
            .Where(candidate => string.Equals(candidate.ProjectId, projectId, StringComparison.Ordinal))
            .ToArray();
        if (sameProjectCandidates.Length == 1) {
            typeFact = sameProjectCandidates[0];
            return true;
        }

        if (sameProjectCandidates.Length > 1 || candidates.Length > 1) {
            diagnostics.Add(
                new AnalysisDiagnostic(
                    "DEP0003",
                    AnalysisDiagnosticSeverity.Warning,
                    $"Multiple collected types share the display name {displayName}. Falling back to the first candidate."));
            _logger.LogWarning("Multiple collected types share the display name {DisplayName}. Falling back to the first candidate.", displayName);
        }

        typeFact = sameProjectCandidates.FirstOrDefault() ?? candidates[0];
        return true;
    }

    private static SourceReference? CreateSourceReference(ISymbol symbol, AnalysisRequest? request) {
        if (request is null) {
            return null;
        }

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

    private sealed record TypeRelationshipCandidate(string TargetDisplayName, TypeRelationshipKind Kind, SourceReference? Source);
}
