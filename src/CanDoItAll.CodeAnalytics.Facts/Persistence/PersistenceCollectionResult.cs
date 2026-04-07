using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Facts.Persistence;

public sealed record PersistenceCollectionResult(
    IReadOnlyList<DbContextFact> DbContexts,
    IReadOnlyList<EntityFact> Entities,
    IReadOnlyList<EntityRelationshipFact> EntityRelationships,
    IReadOnlyList<AnalysisDiagnostic> Diagnostics);
