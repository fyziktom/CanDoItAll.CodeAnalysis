namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record FocusedContextUsageSummary(
    int TotalCallerCount,
    int TotalClusterCount,
    int OmittedCallerCount,
    IReadOnlyList<FocusedContextUsageCluster> Clusters);
