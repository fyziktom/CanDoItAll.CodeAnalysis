namespace CanDoItAll.CodeAnalytics.Domain.Facts;

public sealed record ProjectFact(
    string ProjectId,
    string Name,
    string Path,
    IReadOnlyList<string> TargetFrameworks,
    IReadOnlyList<string> ProjectReferences,
    IReadOnlyList<string> PackageReferences,
    int DocumentCount);
