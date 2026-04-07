namespace CanDoItAll.CodeAnalytics.Storage.Recent;

public sealed record RecentSnapshotRecord(
    string SnapshotId,
    string SolutionName,
    string SolutionPath,
    DateTimeOffset CreatedUtc,
    int FindingCount,
    int DiagnosticCount);
