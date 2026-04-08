using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Identifiers;
using CanDoItAll.CodeAnalytics.Facts.Symbols;
using CanDoItAll.CodeAnalytics.Workspace.Loading;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CanDoItAll.CodeAnalytics.Facts.Dependencies;

public sealed partial class DependencyFactCollector {
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
        var relationshipWeights = new Dictionary<(TypeRelationshipKind Kind, string FromTypeId, string ToTypeId), TypeRelationshipAggregate>();

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

                foreach (var relationship in CollectTypeRelationships(typeSymbol, workspace.Request)) {
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

                    AddTypeRelationship(relationshipWeights, relationship.Kind, sourceType.TypeId, targetType.TypeId, relationship.Source);
                }
            }
        }

        return new DependencyCollectionResult(
            BuildModules(workspace, symbols),
            BuildTypeRelationships(relationshipWeights),
            BuildDependencyFacts(edges),
            diagnostics.OrderBy(diagnostic => diagnostic.Code, StringComparer.Ordinal).ToArray());
    }
}
