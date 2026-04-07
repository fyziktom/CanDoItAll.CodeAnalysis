namespace CanDoItAll.CodeAnalytics.Domain.Facts;

public sealed record DocumentFact(
    string DocumentId,
    string ProjectId,
    string Path,
    string Name,
    int LineCount);
