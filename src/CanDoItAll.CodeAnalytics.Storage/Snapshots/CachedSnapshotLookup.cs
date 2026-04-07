using CanDoItAll.CodeAnalytics.Domain.Snapshot;

namespace CanDoItAll.CodeAnalytics.Storage.Snapshots;

public sealed record CachedSnapshotLookup(
    string RequestHash,
    ArchitectureSnapshot Snapshot);
