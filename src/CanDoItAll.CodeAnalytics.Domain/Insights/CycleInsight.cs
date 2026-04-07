namespace CanDoItAll.CodeAnalytics.Domain.Insights;

public sealed record CycleInsight(
    string Level,
    IReadOnlyList<string> NodeIds);
