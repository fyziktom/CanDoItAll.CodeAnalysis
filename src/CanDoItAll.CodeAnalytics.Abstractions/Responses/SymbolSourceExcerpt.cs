namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record SymbolSourceExcerpt(
    string Path,
    int StartLine,
    int EndLine,
    string Code,
    bool IsTruncated);
