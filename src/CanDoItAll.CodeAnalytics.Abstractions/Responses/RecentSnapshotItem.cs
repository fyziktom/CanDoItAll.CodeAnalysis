namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record RecentSnapshotItem(
    string SnapshotId,
    string SolutionName,
    string SolutionPath,
    DateTimeOffset CreatedUtc,
    int FindingCount,
    int DiagnosticCount,
    bool FromCache);
