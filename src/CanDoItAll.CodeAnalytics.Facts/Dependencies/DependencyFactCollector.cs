using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Identifiers;
using CanDoItAll.CodeAnalytics.Facts.Symbols;
using CanDoItAll.CodeAnalytics.Workspace.Loading;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CanDoItAll.CodeAnalytics.Facts.Dependencies;

public sealed class DependencyFactCollector {
    private readonly ILogger<DependencyFactCollector> _logger;

    public DependencyFactCollector(ILogger<DependencyFactCollector>? logger = null) {
        _logger = logger ?? NullLogger<DependencyFactCollector>.Instance;
    }

    public async Task<DependencyCollectionResult> CollectAsync(
        WorkspaceLoadResult workspace,
        SymbolCollectionResult symbols,
        CancellationToken cancellationToken = default) {
        var diagnostics = new List<AnalysisDiagnostic>();
        var edges = new Dictionary<(DependencyKind Kind, string FromId, string ToId), int>();
        var relationshipWeights = new Dictionary<(TypeRelationshipKind Kind, string FromTypeId, string ToTypeId), int>();

        foreach (var project in workspace.Projects) {
            foreach (var referenceId in project.ProjectReferences) {
                AddEdge(edges, DependencyKind.ProjectReference, project.ProjectId, referenceId);
            }
        }

        if (workspace.RoslynSolution is null) {
            return new DependencyCollectionResult(BuildModules(workspace, symbols), [], BuildDependencyFacts(edges), diagnostics);
        }

        var typeGroupsByDisplayName = symbols.Types
            .GroupBy(type => type.DisplayName, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => {
                    if (group.Count() > 1) {
                        var diagnostic = new AnalysisDiagnostic(
                            "DEP0002",
                            AnalysisDiagnosticSeverity.Warning,
                            $"Duplicate type display name detected: {group.Key}.");
                        diagnostics.Add(diagnostic);
                        _logger.LogWarning("Duplicate type display name detected during dependency collection: {DisplayName}", group.Key);
                    }

                    return group
                        .OrderBy(type => type.ProjectId, StringComparer.Ordinal)
                        .ThenBy(type => type.TypeId, StringComparer.Ordinal)
                        .ToArray();
                },
                StringComparer.Ordinal);

        foreach (var projectContext in workspace.ProjectContexts.OrderBy(context => context.Fact.Name, StringComparer.OrdinalIgnoreCase)) {
            var compilation = await projectContext.Project.GetCompilationAsync(cancellationToken);
            if (compilation is null) {
                diagnostics.Add(
                    new AnalysisDiagnostic(
                        "DEP0001",
                        AnalysisDiagnosticSeverity.Warning,
                        $"Compilation was unavailable for project {projectContext.Fact.Name}."));
                continue;
            }

            foreach (var typeSymbol in EnumerateTypes(compilation.GlobalNamespace)) {
                var sourceDisplayName = typeSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
                if (!TryResolveTypeFact(
                        typeGroupsByDisplayName,
                        sourceDisplayName,
                        projectContext.Fact.ProjectId,
                        diagnostics,
                        out var sourceType)) {
                    continue;
                }

                foreach (var targetDisplayName in CollectDependencyTargetDisplayNames(typeSymbol)) {
                    if (!TryResolveTypeFact(
                            typeGroupsByDisplayName,
                            targetDisplayName,
                            sourceType.ProjectId,
                            diagnostics,
                            out var targetType)) {
                        continue;
                    }

                    if (string.Equals(sourceType.TypeId, targetType.TypeId, StringComparison.Ordinal)) {
                        continue;
                    }

                    AddEdge(edges, DependencyKind.TypeDependency, sourceType.TypeId, targetType.TypeId);

                    if (!string.Equals(sourceType.NamespaceId, targetType.NamespaceId, StringComparison.Ordinal)) {
                        AddEdge(edges, DependencyKind.NamespaceDependency, sourceType.NamespaceId, targetType.NamespaceId);
                    }

                    if (!string.Equals(sourceType.ModuleId, targetType.ModuleId, StringComparison.Ordinal)) {
                        AddEdge(edges, DependencyKind.ModuleDependency, sourceType.ModuleId, targetType.ModuleId);
                    }
                }

                foreach (var relationship in CollectTypeRelationships(typeSymbol)) {
                    if (!TryResolveTypeFact(
                            typeGroupsByDisplayName,
                            relationship.TargetDisplayName,
                            sourceType.ProjectId,
                            diagnostics,
                            out var targetType)) {
                        continue;
                    }

                    if (string.Equals(sourceType.TypeId, targetType.TypeId, StringComparison.Ordinal)) {
                        continue;
                    }

                    AddTypeRelationship(relationshipWeights, relationship.Kind, sourceType.TypeId, targetType.TypeId);
                }
            }
        }

        return new DependencyCollectionResult(
            BuildModules(workspace, symbols),
            BuildTypeRelationships(relationshipWeights),
            BuildDependencyFacts(edges),
            diagnostics.OrderBy(diagnostic => diagnostic.Code, StringComparer.Ordinal).ToArray());
    }

    private static IReadOnlyList<ModuleFact> BuildModules(WorkspaceLoadResult workspace, SymbolCollectionResult symbols) {
        var projectById = workspace.Projects.ToDictionary(project => project.ProjectId, StringComparer.Ordinal);
        return symbols.Namespaces
            .GroupBy(item => new { item.ModuleId, item.ProjectId })
            .Select(
                group => {
                    var project = projectById[group.Key.ProjectId];
                    var namespaceNames = group.Select(item => item.Name)
                        .OrderBy(value => value, StringComparer.Ordinal)
                        .ToArray();
                    var moduleName = ModuleNameClassifier.GetModuleName(project.Name, namespaceNames[0]);

                    return new ModuleFact(
                        group.Key.ModuleId,
                        group.Key.ProjectId,
                        moduleName,
                        moduleName,
                        group.Select(item => item.NamespaceId).OrderBy(value => value, StringComparer.Ordinal).ToArray(),
                        group.SelectMany(item => item.TypeIds).Distinct(StringComparer.Ordinal).OrderBy(value => value, StringComparer.Ordinal).ToArray());
                })
            .OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static IReadOnlyList<DependencyEdgeFact> BuildDependencyFacts(
        IReadOnlyDictionary<(DependencyKind Kind, string FromId, string ToId), int> edges) {
        return edges
            .OrderBy(item => item.Key.Kind)
            .ThenBy(item => item.Key.FromId, StringComparer.Ordinal)
            .ThenBy(item => item.Key.ToId, StringComparer.Ordinal)
            .Select(
                item => new DependencyEdgeFact(
                    StableId.ForDependency($"{item.Key.Kind}:{item.Key.FromId}:{item.Key.ToId}"),
                    item.Key.Kind,
                    item.Key.FromId,
                    item.Key.ToId,
                    item.Value))
            .ToArray();
    }

    private static IReadOnlyList<TypeRelationshipFact> BuildTypeRelationships(
        IReadOnlyDictionary<(TypeRelationshipKind Kind, string FromTypeId, string ToTypeId), int> relationshipWeights) {
        return relationshipWeights
            .OrderBy(item => item.Key.Kind)
            .ThenBy(item => item.Key.FromTypeId, StringComparer.Ordinal)
            .ThenBy(item => item.Key.ToTypeId, StringComparer.Ordinal)
            .Select(
                item => new TypeRelationshipFact(
                    StableId.ForTypeRelationship($"{item.Key.Kind}:{item.Key.FromTypeId}:{item.Key.ToTypeId}"),
                    item.Key.FromTypeId,
                    item.Key.ToTypeId,
                    item.Key.Kind,
                    item.Value))
            .ToArray();
    }

    private static void AddEdge(
        IDictionary<(DependencyKind Kind, string FromId, string ToId), int> edges,
        DependencyKind kind,
        string fromId,
        string toId) {
        var key = (kind, fromId, toId);
        if (edges.TryGetValue(key, out var existing)) {
            edges[key] = existing + 1;
            return;
        }

        edges[key] = 1;
    }

    private static void AddTypeRelationship(
        IDictionary<(TypeRelationshipKind Kind, string FromTypeId, string ToTypeId), int> relationshipWeights,
        TypeRelationshipKind kind,
        string fromTypeId,
        string toTypeId) {
        var key = (kind, fromTypeId, toTypeId);
        if (relationshipWeights.TryGetValue(key, out var existing)) {
            relationshipWeights[key] = existing + 1;
            return;
        }

        relationshipWeights[key] = 1;
    }

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

    private static IEnumerable<TypeRelationshipCandidate> CollectTypeRelationships(
        INamedTypeSymbol typeSymbol) {
        var results = new HashSet<string>(StringComparer.Ordinal);
        var relationships = new List<TypeRelationshipCandidate>();

        foreach (var member in typeSymbol.GetMembers().Where(member => !member.IsImplicitlyDeclared)) {
            switch (member) {
                case IFieldSymbol field:
                    AddExpandedRelationships(relationships, results, field.Type, TypeRelationshipKind.Field);
                    break;
                case IPropertySymbol property:
                    AddExpandedRelationships(relationships, results, property.Type, TypeRelationshipKind.Property);
                    break;
                case IEventSymbol eventSymbol:
                    AddExpandedRelationships(relationships, results, eventSymbol.Type, TypeRelationshipKind.Event);
                    break;
                case IMethodSymbol method:
                    if (method.MethodKind == MethodKind.Constructor) {
                        foreach (var parameter in method.Parameters) {
                            AddExpandedRelationships(relationships, results, parameter.Type, TypeRelationshipKind.ConstructorParameter);
                        }

                        break;
                    }

                    AddExpandedRelationships(relationships, results, method.ReturnType, TypeRelationshipKind.MethodReturn);
                    foreach (var parameter in method.Parameters) {
                        AddExpandedRelationships(relationships, results, parameter.Type, TypeRelationshipKind.MethodParameter);
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
        TypeRelationshipKind kind) {
        foreach (var candidate in ExpandType(type)) {
            var key = $"{kind}:{candidate}";
            if (!seenKeys.Add(key)) {
                continue;
            }

            relationships.Add(new TypeRelationshipCandidate(candidate, kind));
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

    private sealed record TypeRelationshipCandidate(string TargetDisplayName, TypeRelationshipKind Kind);
}
