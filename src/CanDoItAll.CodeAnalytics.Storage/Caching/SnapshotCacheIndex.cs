namespace CanDoItAll.CodeAnalytics.Storage.Caching;

public sealed record SnapshotCacheIndex(IReadOnlyList<SnapshotCacheEntry> Entries) {
    public static SnapshotCacheIndex Empty { get; } = new([]);
}
