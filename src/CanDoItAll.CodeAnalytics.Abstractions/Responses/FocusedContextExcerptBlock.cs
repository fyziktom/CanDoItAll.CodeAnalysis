namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record FocusedContextExcerptBlock(
    string Title,
    string Kind,
    int StartLine,
    int EndLine,
    string Code);
