using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Insights;

namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record DependencyViewResponse(
    string SnapshotId,
    string? SearchText,
    IReadOnlyList<ModuleFact> Modules,
    IReadOnlyList<DependencyEdgeFact> Dependencies,
    IReadOnlyList<CycleInsight> Cycles);
