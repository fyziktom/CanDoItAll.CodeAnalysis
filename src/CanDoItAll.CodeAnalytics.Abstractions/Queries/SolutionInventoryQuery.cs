namespace CanDoItAll.CodeAnalytics.Abstractions.Queries;

public sealed record SolutionInventoryQuery(
    string SnapshotId,
    bool IncludeDocuments = false);
