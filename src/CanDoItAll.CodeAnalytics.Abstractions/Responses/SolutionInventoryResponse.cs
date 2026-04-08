using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record SolutionInventoryResponse(
    string SnapshotId,
    SolutionFact Solution,
    IReadOnlyList<ProjectInventoryItem> Projects);
