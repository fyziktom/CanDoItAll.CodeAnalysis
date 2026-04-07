namespace CanDoItAll.CodeAnalytics.Abstractions.Queries;

public sealed record SnapshotQuery(
    string SnapshotId,
    string? SearchText = null);
