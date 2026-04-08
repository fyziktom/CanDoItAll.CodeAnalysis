using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Identifiers;
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
            return new PersistenceCollectionResult([], [], [], []);
        }

        var diagnostics = new List<AnalysisDiagnostic>();
        var dbContexts = new List<DbContextFact>();
        var entityFactsById = new Dictionary<string, EntityFact>(StringComparer.Ordinal);
        var entityRelationshipTargetsById = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
        var navigationCandidates = new List<EntityNavigationCandidate>();
        var configuredEntityRelationships = new Dictionary<string, EntityRelationshipFact>(StringComparer.Ordinal);
        var typesByDisplayName = symbols.Types
            .GroupBy(type => type.DisplayName, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.ToArray(), StringComparer.Ordinal);
        var knownTypeDisplayNames = typesByDisplayName.Keys.ToHashSet(StringComparer.Ordinal);
        var entityIdsByIdentity = new Dictionary<(string ProjectId, string DisplayName), string>(EqualityComparer<(string ProjectId, string DisplayName)>.Default);
        var projectAnalyses = await BuildProjectAnalysesAsync(workspace, diagnostics, cancellationToken);
        var modelSnapshotDiscoveries = await BuildModelSnapshotDiscoveriesAsync(workspace, diagnostics, cancellationToken);
        var projectAnalysesByProjectId = projectAnalyses.ToDictionary(item => item.Context.Fact.ProjectId, item => item, StringComparer.Ordinal);
        var configurationMappings = projectAnalyses
            .SelectMany(item => item.ConfigurationMappings)
            .OrderBy(item => item.ProjectId, StringComparer.Ordinal)
            .ThenBy(item => item.EntityDisplayName, StringComparer.Ordinal)
            .ToArray();
        var relationshipMappings = projectAnalyses
            .SelectMany(item => item.RelationshipMappings)
            .OrderBy(item => item.ProjectId, StringComparer.Ordinal)
            .ThenBy(item => item.FromEntityDisplayName, StringComparer.Ordinal)
            .ThenBy(item => item.ToEntityDisplayName, StringComparer.Ordinal)
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
                var applicableRelationshipMappings = ResolveApplicableRelationshipMappings(
                    relationshipMappings,
                    analysis.Context.Fact.ProjectId,
                    dbContextModel);
                var modelSnapshotDiscovery = ResolveModelSnapshotDiscovery(modelSnapshotDiscoveries, dbContextDisplayName);
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

                if (modelSnapshotDiscovery is not null) {
                    foreach (var entityDisplayName in modelSnapshotDiscovery.EntityDisplayNames) {
                        entityDisplayNames.Add(entityDisplayName);
                    }
                }

                var storeObjectMappings = BuildStoreObjectMappings(applicableConfigurationMappings, dbContextModel);
                if (modelSnapshotDiscovery is not null) {
                    foreach (var mapping in modelSnapshotDiscovery.StoreObjectMappings) {
                        MergeStoreObjectMapping(storeObjectMappings, mapping, overwriteExisting: false);
                    }

                    applicableRelationshipMappings = applicableRelationshipMappings
                        .Concat(CreateModelSnapshotRelationshipMappings(modelSnapshotDiscovery, analysis.Context.Fact.ProjectId))
                        .GroupBy(
                            item => (item.ProjectId, item.FromEntityDisplayName, item.ToEntityDisplayName, item.Kind),
                            EqualityComparer<(string ProjectId, string FromEntityDisplayName, string ToEntityDisplayName, EntityRelationshipKind Kind)>.Default)
                        .Select(group => group.First())
                        .OrderBy(item => item.FromEntityDisplayName, StringComparer.Ordinal)
                        .ThenBy(item => item.ToEntityDisplayName, StringComparer.Ordinal)
                        .ThenBy(item => item.Kind)
                        .ToArray();
                }

                var entityIds = new List<string>();
                var knownEntityDisplayNames = entityDisplayNames.ToHashSet(StringComparer.Ordinal);
                var resolvedEntities = new List<ResolvedEntityContext>();

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
                    }

                    var storeObjectMapping = ResolveStoreObjectMapping(
                        entityDisplayName,
                        storeObjectMappings,
                        PersistenceSyntaxExplorer.TryReadTableAttribute(entitySymbol, workspace.Request),
                        dbContextModel.DefaultSchema);
                    resolvedEntities.Add(new ResolvedEntityContext(entityId, entitySymbol, entityType, storeObjectMapping));
                    entityIds.Add(entityId);
                }

                foreach (var resolvedEntity in resolvedEntities.OrderBy(item => item.EntityType.DisplayName, StringComparer.Ordinal)) {
                    var candidates = CreateEntityRelationshipCandidates(
                        resolvedEntity.EntityId,
                        resolvedEntity.EntitySymbol,
                        resolvedEntity.EntityType,
                        workspace.Request,
                        knownTypeDisplayNames,
                        knownEntityDisplayNames,
                        entityIdsByIdentity);
                    navigationCandidates.AddRange(candidates);

                    if (!entityRelationshipTargetsById.TryGetValue(resolvedEntity.EntityId, out var relationshipTargets)) {
                        relationshipTargets = new HashSet<string>(StringComparer.Ordinal);
                        entityRelationshipTargetsById[resolvedEntity.EntityId] = relationshipTargets;
                    }

                    foreach (var candidate in candidates) {
                        relationshipTargets.Add(candidate.ToEntityId);
                    }

                    entityFactsById[resolvedEntity.EntityId] = CreateEntityFact(
                        resolvedEntity.EntityId,
                        resolvedEntity.EntitySymbol,
                        resolvedEntity.EntityType,
                        relationshipTargets.OrderBy(value => value, StringComparer.Ordinal).ToArray(),
                        resolvedEntity.StoreObjectMapping);
                }

                foreach (var relationshipMapping in applicableRelationshipMappings) {
                    var relationship = CreateConfiguredEntityRelationshipFact(
                        relationshipMapping,
                        entityIdsByIdentity,
                        entityFactsById);
                    if (relationship is null) {
                        continue;
                    }

                    if (!entityRelationshipTargetsById.TryGetValue(relationship.FromEntityId, out var fromTargets)) {
                        fromTargets = new HashSet<string>(StringComparer.Ordinal);
                        entityRelationshipTargetsById[relationship.FromEntityId] = fromTargets;
                    }

                    if (!entityRelationshipTargetsById.TryGetValue(relationship.ToEntityId, out var toTargets)) {
                        toTargets = new HashSet<string>(StringComparer.Ordinal);
                        entityRelationshipTargetsById[relationship.ToEntityId] = toTargets;
                    }

                    fromTargets.Add(relationship.ToEntityId);
                    toTargets.Add(relationship.FromEntityId);
                    entityFactsById[relationship.FromEntityId] = entityFactsById[relationship.FromEntityId] with {
                        RelationshipTargets = fromTargets.OrderBy(value => value, StringComparer.Ordinal).ToArray(),
                    };
                    entityFactsById[relationship.ToEntityId] = entityFactsById[relationship.ToEntityId] with {
                        RelationshipTargets = toTargets.OrderBy(value => value, StringComparer.Ordinal).ToArray(),
                    };
                    configuredEntityRelationships[relationship.RelationshipId] = relationship;
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

        var entities = entityFactsById
            .Values
            .OrderBy(item => item.DisplayName, StringComparer.Ordinal)
            .ToArray();
        var entityRelationships = MergeEntityRelationships(
            BuildEntityRelationships(navigationCandidates, entityFactsById),
            configuredEntityRelationships.Values.ToArray());
        return new PersistenceCollectionResult(
            dbContexts.OrderBy(item => item.DisplayName, StringComparer.Ordinal).ToArray(),
            entities,
            entityRelationships,
            diagnostics.OrderBy(item => item.Code, StringComparer.Ordinal).ThenBy(item => item.Message, StringComparer.Ordinal).ToArray());
    }
}
