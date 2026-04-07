using CanDoItAll.CodeAnalytics.Domain.Sources;

namespace CanDoItAll.CodeAnalytics.Domain.Insights;

public sealed record FindingInsight(
    string FindingId,
    string RuleId,
    FindingSeverity Severity,
    FindingCategory Category,
    string Title,
    string Description,
    string Rationale,
    double Confidence,
    IReadOnlyList<string> RelatedIds,
    SourceReference? Source = null);
