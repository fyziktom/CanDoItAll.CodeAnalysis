namespace CanDoItAll.CodeAnalytics.Domain.Sources;

public sealed record SourceReference(
    string Path,
    int? Line = null,
    int? Column = null);
