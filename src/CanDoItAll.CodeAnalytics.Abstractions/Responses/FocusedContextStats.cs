namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record FocusedContextStats(
    int FileCount,
    int BlockCount,
    int SelectedLineCount,
    int TotalLineCount);
