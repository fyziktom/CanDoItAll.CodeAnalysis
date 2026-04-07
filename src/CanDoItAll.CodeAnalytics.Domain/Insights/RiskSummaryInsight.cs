namespace CanDoItAll.CodeAnalytics.Domain.Insights;

public sealed record RiskSummaryInsight(
    int ProjectCount,
    int TypeCount,
    int MemberCount,
    int ServiceRegistrationCount,
    int EntityCount,
    int FindingCount,
    int OpenQuestionCount,
    int DiagnosticCount);
