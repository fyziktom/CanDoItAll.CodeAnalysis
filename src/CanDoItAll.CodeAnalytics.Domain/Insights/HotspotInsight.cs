namespace CanDoItAll.CodeAnalytics.Domain.Insights;

public sealed record HotspotInsight(
    string NodeId,
    string Kind,
    double Score,
    string Reason);
