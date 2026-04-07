namespace CanDoItAll.CodeAnalytics.Storage.Caching;

public sealed record SnapshotCacheEntry(
    string RequestHash,
    string SnapshotId);
