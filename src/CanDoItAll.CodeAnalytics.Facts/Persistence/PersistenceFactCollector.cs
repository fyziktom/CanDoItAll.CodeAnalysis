using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Identifiers;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Facts.Symbols;
using CanDoItAll.CodeAnalytics.Workspace.Loading;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CanDoItAll.CodeAnalytics.Facts.Persistence;

public sealed partial class PersistenceFactCollector {
    private readonly ILogger<PersistenceFactCollector> _logger;

    public PersistenceFactCollector(ILogger<PersistenceFactCollector>? logger = null) {
        _logger = logger ?? NullLogger<PersistenceFactCollector>.Instance;
    }

    public async Task<PersistenceCollectionResult> CollectAsync(
        WorkspaceLoadResult workspace,
        SymbolCollectionResult symbols,
        CancellationToken cancellationToken = default) {
        if (!workspace.Request.IncludePersistence || workspace.RoslynSolution is null) {
            return new PersistenceCollectionResult([], [], []);
        }

        var diagnostics = new List<AnalysisDiagnostic>();
        var dbContexts = new List<DbContextFact>();
        var entities = new List<EntityFact>();
        var typesByDisplayName = symbols.Types
            .GroupBy(type => type.DisplayName, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.ToArray(), StringComparer.Ordinal);
        var knownTypeDisplayNames = typesByDisplayName.Keys.ToHashSet(StringComparer.Ordinal);
        var entityIdsByIdentity = new Dictionary<(string ProjectId, string DisplayName), string>(EqualityComparer<(string ProjectId, string DisplayName)>.Default);
        var projectAnalyses = await BuildProjectAnalysesAsync(workspace, diagnostics, cancellationToken);
        var projectAnalysesByProjectId = projectAnalyses.ToDictionary(item => item.Context.Fact.ProjectId, item => item, StringComparer.Ordinal);
        var configurationMappings = projectAnalyses
            .SelectMany(item => item.ConfigurationMappings)
            .OrderBy(item => item.ProjectId, StringComparer.Ordinal)
            .ThenBy(item => item.EntityDisplayName, StringComparer.Ordinal)
            .ToArray();

        foreach (var analysis in projectAnalyses) {
            foreach (var dbContextSymbol in EnumerateTypes(analysis.Compilation.GlobalNamespace)
                .Where(symbol => IsOwnedByProject(symbol, analysis.ProjectDocumentPaths))
                .Where(IsDbContext)) {
                var dbContextDisplayName = dbContextSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
                if (!TryResolveTypeFact(typesByDisplayName, dbContextDisplayName, analysis.Context.Fact.ProjectId, diagnostics, out var dbContextType)) {
                    continue;
                }

                var dbContextModel = PersistenceSyntaxExplorer.DiscoverDbContextModel(
                    dbContextSymbol,
                    analysis.Compilation,
                    workspace.Request,
                    cancellationToken);
                foreach (var diagnostic in dbContextModel.Diagnostics) {
                    diagnostics.Add(diagnostic);
                }

                var applicableConfigurationMappings = ResolveApplicableConfigurationMappings(
                    configurationMappings,
                    analysis.Context.Fact.ProjectId,
                    dbContextModel,
                    diagnostics,
                    dbContextDisplayName);
                var entityDisplayNames = new HashSet<string>(StringComparer.Ordinal);
                foreach (var entitySymbol in EnumerateDbSetEntityTypes(dbContextSymbol)) {
                    entityDisplayNames.Add(entitySymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat));
                }

                foreach (var entityDisplayName in dbContextModel.EntityDisplayNames) {
                    entityDisplayNames.Add(entityDisplayName);
                }

                foreach (var configurationMapping in applicableConfigurationMappings) {
                    entityDisplayNames.Add(configurationMapping.EntityDisplayName);
                }

                var storeObjectMappings = BuildStoreObjectMappings(applicableConfigurationMappings, dbContextModel);
                var entityIds = new List<string>();
                var knownEntityDisplayNames = entityDisplayNames.ToHashSet(StringComparer.Ordinal);

                foreach (var entityDisplayName in entityDisplayNames.OrderBy(value => value, StringComparer.Ordinal)) {
                    if (!TryResolveTypeFact(typesByDisplayName, entityDisplayName, analysis.Context.Fact.ProjectId, diagnostics, out var entityType)) {
                        diagnostics.Add(
                            new AnalysisDiagnostic(
                                "EF0002",
                                AnalysisDiagnosticSeverity.Info,
                                $"Entity type {entityDisplayName} was not part of the collected source symbol set."));
                        continue;
                    }

                    if (!TryResolveEntitySymbol(projectAnalysesByProjectId, entityType.ProjectId, entityDisplayName, out var entitySymbol)) {
                        diagnostics.Add(
                            new AnalysisDiagnostic(
                                "EF0006",
                                AnalysisDiagnosticSeverity.Warning,
                                $"Entity type {entityDisplayName} could not be resolved back to a Roslyn source symbol."));
                        continue;
                    }

                    var entityKey = (entityType.ProjectId, entityDisplayName);
                    if (!entityIdsByIdentity.TryGetValue(entityKey, out var entityId)) {
                        entityId = StableId.ForEntity($"{entityType.ProjectId}:{entityDisplayName}");
                        entityIdsByIdentity[entityKey] = entityId;

                        var storeObjectMapping = ResolveStoreObjectMapping(
                            entityDisplayName,
                            storeObjectMappings,
                            PersistenceSyntaxExplorer.TryReadTableAttribute(entitySymbol, workspace.Request),
                            dbContextModel.DefaultSchema);
                        entities.Add(
                            CreateEntityFact(
                                entityId,
                                entitySymbol,
                                entityType,
                                knownTypeDisplayNames,
                                knownEntityDisplayNames,
                                entityIdsByIdentity,
                                storeObjectMapping));
                    }

                    entityIds.Add(entityId);
                }

                dbContexts.Add(
                    new DbContextFact(
                        StableId.ForDbContext($"{dbContextType.ProjectId}:{dbContextDisplayName}"),
                        dbContextType.TypeId,
                        dbContextType.ProjectId,
                        dbContextType.ModuleId,
                        dbContextSymbol.Name,
                        entityIds.OrderBy(value => value, StringComparer.Ordinal).ToArray(),
                        dbContextType.Source));
            }
        }

        return new PersistenceCollectionResult(
            dbContexts.OrderBy(item => item.DisplayName, StringComparer.Ordinal).ToArray(),
            entities.OrderBy(item => item.DisplayName, StringComparer.Ordinal).ToArray(),
            diagnostics.OrderBy(item => item.Code, StringComparer.Ordinal).ThenBy(item => item.Message, StringComparer.Ordinal).ToArray());
    }

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

            analyses.Add(
                new ProjectAnalysisContext(
                    projectContext,
                    compilation,
                    projectDocumentPaths,
                    sourceTypesByDisplayName,
                    configurationMappings));
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

    private static bool TryResolveEntitySymbol(
        IReadOnlyDictionary<string, ProjectAnalysisContext> projectAnalysesByProjectId,
        string projectId,
        string displayName,
        out INamedTypeSymbol entitySymbol) {
        if (!projectAnalysesByProjectId.TryGetValue(projectId, out var analysis) ||
            !analysis.SourceTypesByDisplayName.TryGetValue(displayName, out var candidates) ||
            candidates.Length == 0) {
            entitySymbol = null!;
            return false;
        }

        entitySymbol = candidates
            .OrderBy(candidate => candidate.Name, StringComparer.Ordinal)
            .First();
        return true;
    }

    private static IEnumerable<INamedTypeSymbol> EnumerateDbSetEntityTypes(INamedTypeSymbol dbContextSymbol) {
        var current = dbContextSymbol;
        while (current is not null) {
            foreach (var entitySymbol in current.GetMembers()
                .OfType<IPropertySymbol>()
                .Select(property => ResolveDbSetEntityType(property.Type))
                .Where(symbol => symbol is not null)
                .Cast<INamedTypeSymbol>()) {
                yield return entitySymbol;
            }

            current = current.BaseType;
        }
    }

    private static bool IsDbContext(INamedTypeSymbol symbol) {
        var current = symbol;
        while (current is not null) {
            if (string.Equals(current.ToDisplayString(), "Microsoft.EntityFrameworkCore.DbContext", StringComparison.Ordinal)) {
                return true;
            }

            current = current.BaseType;
        }

        return false;
    }

    private static bool ShouldIncludeProject(AnalysisRequest request, ProjectFact project) {
        if (request.ScopeProjectNames.Count == 0) {
            return true;
        }

        return request.ScopeProjectNames.Contains(project.Name, StringComparer.OrdinalIgnoreCase);
    }

    private static bool IsOwnedByProject(ISymbol symbol, ISet<string> projectDocumentPaths) {
        return symbol.Locations
            .Where(location => location.IsInSource && location.SourceTree?.FilePath is not null)
            .Select(location => Path.GetFullPath(location.SourceTree!.FilePath))
            .Any(projectDocumentPaths.Contains);
    }

    private bool TryResolveTypeFact(
        IReadOnlyDictionary<string, TypeFact[]> typesByDisplayName,
        string displayName,
        string projectId,
        ICollection<AnalysisDiagnostic> diagnostics,
        out TypeFact typeFact) {
        if (!typesByDisplayName.TryGetValue(displayName, out var candidates) || candidates.Length == 0) {
            typeFact = null!;
            return false;
        }

        var projectMatch = candidates.FirstOrDefault(candidate => string.Equals(candidate.ProjectId, projectId, StringComparison.Ordinal));
        if (projectMatch is not null) {
            typeFact = projectMatch;
            return true;
        }

        if (candidates.Length > 1) {
            var diagnostic = new AnalysisDiagnostic(
                "EF0004",
                AnalysisDiagnosticSeverity.Warning,
                $"Multiple collected types share the display name {displayName}. Falling back to the first candidate.");
            diagnostics.Add(diagnostic);
            _logger.LogWarning("Multiple collected types share the display name {DisplayName}. Falling back to the first candidate.", displayName);
        }

        typeFact = candidates
            .OrderBy(candidate => candidate.ProjectId, StringComparer.Ordinal)
            .ThenBy(candidate => candidate.TypeId, StringComparer.Ordinal)
            .First();
        return true;
    }

    private sealed record ProjectAnalysisContext(
        WorkspaceProjectContext Context,
        Compilation Compilation,
        ISet<string> ProjectDocumentPaths,
        IReadOnlyDictionary<string, INamedTypeSymbol[]> SourceTypesByDisplayName,
        IReadOnlyList<EntityConfigurationMapping> ConfigurationMappings);
}
