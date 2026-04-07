namespace CanDoItAll.CodeAnalytics.Storage.Recent;

public sealed record RecentSnapshotIndex(IReadOnlyList<RecentSnapshotRecord> Entries) {
    public static RecentSnapshotIndex Empty { get; } = new([]);
}
