using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Workspace.Loading;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace CanDoItAll.CodeAnalytics.Facts.Persistence;

public sealed partial class PersistenceFactCollector {
    private async Task<IReadOnlyList<ProjectAnalysisContext>> BuildProjectAnalysesAsync(
        WorkspaceLoadResult workspace,
        ICollection<AnalysisDiagnostic> diagnostics,
        CancellationToken cancellationToken) {
        var analyses = new List<ProjectAnalysisContext>();

        foreach (var projectContext in workspace.ProjectContexts.OrderBy(context => context.Fact.Name, StringComparer.OrdinalIgnoreCase)) {
            if (!ShouldIncludeProject(workspace.Request, projectContext.Fact)) {
                continue;
            }

            var compilation = await projectContext.Project.GetCompilationAsync(cancellationToken);
            if (compilation is null) {
                diagnostics.Add(
                    new AnalysisDiagnostic(
                        "EF0001",
                        AnalysisDiagnosticSeverity.Warning,
                        $"Compilation was unavailable for project {projectContext.Fact.Name}."));
                continue;
            }

            var projectDocumentPaths = projectContext.Project.Documents
                .Where(document => !string.IsNullOrWhiteSpace(document.FilePath))
                .Select(document => Path.GetFullPath(document.FilePath!))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            var sourceTypesByDisplayName = EnumerateTypes(compilation.GlobalNamespace)
                .Where(symbol => IsOwnedByProject(symbol, projectDocumentPaths))
                .GroupBy(symbol => symbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat), StringComparer.Ordinal)
                .ToDictionary(group => group.Key, group => group.ToArray(), StringComparer.Ordinal);
            var configurationMappings = PersistenceSyntaxExplorer.DiscoverEntityConfigurations(
                projectContext,
                compilation,
                workspace.Request,
                projectDocumentPaths,
                cancellationToken);
            var relationshipMappings = PersistenceSyntaxExplorer.DiscoverEntityRelationships(
                projectContext,
                compilation,
                workspace.Request,
                projectDocumentPaths,
                cancellationToken);

            analyses.Add(
                new ProjectAnalysisContext(
                    projectContext,
                    compilation,
                    projectDocumentPaths,
                    sourceTypesByDisplayName,
                    configurationMappings,
                    relationshipMappings));
        }

        return analyses;
    }

    private IReadOnlyList<EntityConfigurationMapping> ResolveApplicableConfigurationMappings(
        IReadOnlyList<EntityConfigurationMapping> configurationMappings,
        string dbContextProjectId,
        DbContextModelDiscovery dbContextModel,
        ICollection<AnalysisDiagnostic> diagnostics,
        string dbContextDisplayName) {
        var mappings = new List<EntityConfigurationMapping>();
        if (dbContextModel.IncludesSameProjectConfigurations) {
            mappings.AddRange(configurationMappings.Where(mapping => string.Equals(mapping.ProjectId, dbContextProjectId, StringComparison.Ordinal)));
        }

        if (dbContextModel.IncludesExternalConfigurations) {
            mappings.AddRange(configurationMappings);
            diagnostics.Add(
                new AnalysisDiagnostic(
                    "EF0005",
                    AnalysisDiagnosticSeverity.Info,
                    $"DbContext {dbContextDisplayName} applies configurations from external assemblies. The collector interpreted that call broadly across discovered configuration types."));
            _logger.LogInformation(
                "DbContext {DbContextDisplayName} applies configurations from external assemblies. The collector interpreted that call broadly.",
                dbContextDisplayName);
        }

        return mappings
            .GroupBy(mapping => (mapping.ProjectId, mapping.EntityDisplayName), EqualityComparer<(string ProjectId, string EntityDisplayName)>.Default)
            .Select(group => group.First())
            .OrderBy(mapping => mapping.ProjectId, StringComparer.Ordinal)
            .ThenBy(mapping => mapping.EntityDisplayName, StringComparer.Ordinal)
            .ToArray();
    }

    private static IReadOnlyList<ConfiguredEntityRelationshipMapping> ResolveApplicableRelationshipMappings(
        IReadOnlyList<ConfiguredEntityRelationshipMapping> relationshipMappings,
        string dbContextProjectId,
        DbContextModelDiscovery dbContextModel) {
        var mappings = new List<ConfiguredEntityRelationshipMapping>();
        if (dbContextModel.IncludesSameProjectConfigurations) {
            mappings.AddRange(relationshipMappings.Where(mapping => string.Equals(mapping.ProjectId, dbContextProjectId, StringComparison.Ordinal)));
        }

        if (dbContextModel.IncludesExternalConfigurations) {
            mappings.AddRange(relationshipMappings);
        }

        return mappings
            .GroupBy(
                item => (item.ProjectId, item.FromEntityDisplayName, item.ToEntityDisplayName, item.Kind),
                EqualityComparer<(string ProjectId, string FromEntityDisplayName, string ToEntityDisplayName, EntityRelationshipKind Kind)>.Default)
            .Select(group => group.First())
            .OrderBy(item => item.FromEntityDisplayName, StringComparer.Ordinal)
            .ThenBy(item => item.ToEntityDisplayName, StringComparer.Ordinal)
            .ThenBy(item => item.Kind)
            .ToArray();
    }

    private static Dictionary<string, EntityStoreObjectMapping> BuildStoreObjectMappings(
        IReadOnlyList<EntityConfigurationMapping> configurationMappings,
        DbContextModelDiscovery dbContextModel) {
        var storeObjectMappings = new Dictionary<string, EntityStoreObjectMapping>(StringComparer.Ordinal);

        foreach (var configurationMapping in configurationMappings) {
            MergeStoreObjectMapping(
                storeObjectMappings,
                new EntityStoreObjectMapping(
                    configurationMapping.EntityDisplayName,
                    configurationMapping.TableName,
                    configurationMapping.Schema,
                    configurationMapping.Source),
                overwriteExisting: false);
        }

        foreach (var mapping in dbContextModel.StoreObjectMappings) {
            MergeStoreObjectMapping(storeObjectMappings, mapping, overwriteExisting: true);
        }

        return storeObjectMappings;
    }

    private static EntityStoreObjectMapping? ResolveStoreObjectMapping(
        string entityDisplayName,
        IReadOnlyDictionary<string, EntityStoreObjectMapping> storeObjectMappings,
        EntityStoreObjectMapping? attributeMapping,
        string? defaultSchema) {
        storeObjectMappings.TryGetValue(entityDisplayName, out var mapping);
        if (mapping is null && attributeMapping is null && string.IsNullOrWhiteSpace(defaultSchema)) {
            return null;
        }

        var tableName = mapping?.TableName ?? attributeMapping?.TableName;
        var schema = mapping?.Schema ?? attributeMapping?.Schema ?? defaultSchema;
        return new EntityStoreObjectMapping(
            entityDisplayName,
            tableName,
            schema,
            mapping?.Source ?? attributeMapping?.Source);
    }

    private static void MergeStoreObjectMapping(
        IDictionary<string, EntityStoreObjectMapping> storeObjectMappings,
        EntityStoreObjectMapping candidate,
        bool overwriteExisting) {
        if (!storeObjectMappings.TryGetValue(candidate.EntityDisplayName, out var existing)) {
            storeObjectMappings[candidate.EntityDisplayName] = candidate;
            return;
        }

        storeObjectMappings[candidate.EntityDisplayName] = new EntityStoreObjectMapping(
            candidate.EntityDisplayName,
            overwriteExisting
                ? candidate.TableName ?? existing.TableName
                : existing.TableName ?? candidate.TableName,
            overwriteExisting
                ? candidate.Schema ?? existing.Schema
                : existing.Schema ?? candidate.Schema,
            existing.Source ?? candidate.Source);
    }

    private sealed record ProjectAnalysisContext(
        WorkspaceProjectContext Context,
        Compilation Compilation,
        ISet<string> ProjectDocumentPaths,
        IReadOnlyDictionary<string, INamedTypeSymbol[]> SourceTypesByDisplayName,
        IReadOnlyList<EntityConfigurationMapping> ConfigurationMappings,
        IReadOnlyList<ConfiguredEntityRelationshipMapping> RelationshipMappings);

    private sealed record ResolvedEntityContext(
        string EntityId,
        INamedTypeSymbol EntitySymbol,
        TypeFact EntityType,
        EntityStoreObjectMapping? StoreObjectMapping);
}
