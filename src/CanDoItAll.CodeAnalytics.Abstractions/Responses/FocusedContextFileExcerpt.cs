namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record FocusedContextFileExcerpt(
    string Path,
    int TotalLineCount,
    int SelectedLineCount,
    IReadOnlyList<string> TypeDisplayNames,
    IReadOnlyList<FocusedContextExcerptBlock> Blocks);
