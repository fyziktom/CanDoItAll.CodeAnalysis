namespace CanDoItAll.CodeAnalytics.Storage.Paths;

public sealed class SnapshotPathResolver {
    private readonly string _outputRootPath;

    public SnapshotPathResolver(string outputRootPath) {
        _outputRootPath = Path.GetFullPath(outputRootPath);
    }

    public string GetCacheIndexPath() {
        return Path.Combine(_outputRootPath, "cache", "index.json");
    }

    public string GetRecentIndexPath() {
        return Path.Combine(_outputRootPath, "recent", "index.json");
    }

    public string GetSnapshotDirectory(string snapshotId) {
        return Path.Combine(_outputRootPath, "snapshots", snapshotId);
    }

    public string GetSnapshotJsonPath(string snapshotId) {
        return Path.Combine(GetSnapshotDirectory(snapshotId), "snapshot.json");
    }
}
