using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record PersistenceViewResponse(
    string SnapshotId,
    string? SearchText,
    IReadOnlyList<DbContextFact> DbContexts,
    IReadOnlyList<EntityFact> Entities,
    IReadOnlyList<AnalysisDiagnostic> Diagnostics);
