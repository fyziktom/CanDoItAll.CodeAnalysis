namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record FocusedContextUsageSample(
    string TypeId,
    string TypeDisplayName,
    string MemberId,
    string MemberDisplayName,
    string Path,
    int? Line,
    string Reason);
