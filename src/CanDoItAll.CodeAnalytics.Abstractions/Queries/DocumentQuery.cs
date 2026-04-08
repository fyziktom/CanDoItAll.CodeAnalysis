namespace CanDoItAll.CodeAnalytics.Abstractions.Queries;

public sealed record DocumentQuery(
    string SnapshotId,
    string? DocumentId = null,
    string? DocumentPath = null);
