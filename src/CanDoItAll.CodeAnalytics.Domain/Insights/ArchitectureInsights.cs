namespace CanDoItAll.CodeAnalytics.Domain.Insights;

public sealed record ArchitectureInsights(
    RiskSummaryInsight Summary,
    IReadOnlyList<CycleInsight> Cycles,
    IReadOnlyList<HotspotInsight> Hotspots,
    IReadOnlyList<FindingInsight> Findings,
    IReadOnlyList<OpenQuestionInsight> OpenQuestions) {
    public static ArchitectureInsights Empty { get; } = new(
        new RiskSummaryInsight(0, 0, 0, 0, 0, 0, 0, 0),
        [],
        [],
        [],
        []);
}
