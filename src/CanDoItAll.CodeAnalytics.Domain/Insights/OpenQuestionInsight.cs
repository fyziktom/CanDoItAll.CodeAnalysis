using CanDoItAll.CodeAnalytics.Domain.Sources;

namespace CanDoItAll.CodeAnalytics.Domain.Insights;

public sealed record OpenQuestionInsight(
    string QuestionId,
    string Title,
    string Description,
    double Confidence,
    IReadOnlyList<string> RelatedIds,
    SourceReference? Source = null);
