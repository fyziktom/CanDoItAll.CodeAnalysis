namespace CanDoItAll.CodeAnalytics.Abstractions.Queries;

public sealed record ProjectInventoryQuery(
    string SnapshotId,
    string? ProjectId = null,
    string? ProjectName = null,
    bool IncludeDocuments = true);
