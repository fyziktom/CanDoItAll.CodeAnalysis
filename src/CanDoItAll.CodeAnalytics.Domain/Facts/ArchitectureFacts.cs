namespace CanDoItAll.CodeAnalytics.Domain.Facts;

public sealed record ArchitectureFacts(
    SolutionFact Solution,
    IReadOnlyList<ProjectFact> Projects,
    IReadOnlyList<DocumentFact> Documents,
    IReadOnlyList<ModuleFact> Modules,
    IReadOnlyList<NamespaceFact> Namespaces,
    IReadOnlyList<TypeFact> Types,
    IReadOnlyList<MemberFact> Members,
    IReadOnlyList<TypeRelationshipFact> TypeRelationships,
    IReadOnlyList<ServiceRegistrationFact> ServiceRegistrations,
    IReadOnlyList<DbContextFact> DbContexts,
    IReadOnlyList<EntityFact> Entities,
    IReadOnlyList<EntityRelationshipFact> EntityRelationships,
    IReadOnlyList<DependencyEdgeFact> Dependencies);
