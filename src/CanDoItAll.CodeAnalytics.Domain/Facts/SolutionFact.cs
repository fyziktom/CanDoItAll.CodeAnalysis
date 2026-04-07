namespace CanDoItAll.CodeAnalytics.Domain.Facts;

public sealed record SolutionFact(
    string Name,
    string Path,
    int ProjectCount,
    int DocumentCount);
