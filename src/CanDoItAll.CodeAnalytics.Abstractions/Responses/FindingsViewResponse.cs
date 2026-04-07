using CanDoItAll.CodeAnalytics.Domain.Insights;

namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record FindingsViewResponse(
    string SnapshotId,
    string? SearchText,
    IReadOnlyList<FindingInsight> Findings,
    IReadOnlyList<OpenQuestionInsight> OpenQuestions,
    IReadOnlyList<HotspotInsight> Hotspots);
