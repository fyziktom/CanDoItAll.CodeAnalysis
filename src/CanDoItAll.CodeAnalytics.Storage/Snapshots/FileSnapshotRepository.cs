using System.Text;
using CanDoItAll.CodeAnalytics.Domain.Exports;
using CanDoItAll.CodeAnalytics.Domain.Identifiers;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Storage.Caching;
using CanDoItAll.CodeAnalytics.Storage.Paths;
using CanDoItAll.CodeAnalytics.Storage.Recent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CanDoItAll.CodeAnalytics.Storage.Snapshots;

public sealed class FileSnapshotRepository {
    private readonly SnapshotJsonSerializer _serializer;
    private readonly ILogger<FileSnapshotRepository> _logger;

    public FileSnapshotRepository(
        SnapshotJsonSerializer serializer,
        ILogger<FileSnapshotRepository>? logger = null) {
        _serializer = serializer;
        _logger = logger ?? NullLogger<FileSnapshotRepository>.Instance;
    }

    public string ComputeRequestHash(
        AnalysisRequest request,
        string generatorVersion,
        string schemaVersion) {
        var json = _serializer.Serialize(
            new {
                schemaVersion,
                generatorVersion,
                request,
            });
        return StableId.ToHash(json);
    }

    public async Task<CachedSnapshotLookup?> TryGetCachedSnapshotAsync(
        SnapshotPathResolver pathResolver,
        string requestHash,
        CancellationToken cancellationToken = default) {
        var index = await ReadCacheIndexAsync(pathResolver, cancellationToken);
        var entry = index.Entries.FirstOrDefault(item => string.Equals(item.RequestHash, requestHash, StringComparison.Ordinal));
        if (entry is null) {
            _logger.LogInformation("Snapshot cache miss for request hash {RequestHash}", requestHash);
            return null;
        }

        var snapshotPath = pathResolver.GetSnapshotJsonPath(entry.SnapshotId);
        if (!File.Exists(snapshotPath)) {
            _logger.LogWarning("Snapshot cache entry {SnapshotId} was missing on disk.", entry.SnapshotId);
            return null;
        }

        var json = await File.ReadAllTextAsync(snapshotPath, cancellationToken);
        var snapshot = _serializer.DeserializeSnapshot(json);
        _logger.LogInformation("Snapshot cache hit for request hash {RequestHash} resolved to {SnapshotId}", requestHash, entry.SnapshotId);
        return new CachedSnapshotLookup(requestHash, snapshot);
    }

    public async Task<ArchitectureSnapshot?> LoadSnapshotAsync(
        SnapshotPathResolver pathResolver,
        string snapshotId,
        CancellationToken cancellationToken = default) {
        var snapshotPath = pathResolver.GetSnapshotJsonPath(snapshotId);
        if (!File.Exists(snapshotPath)) {
            _logger.LogInformation("Snapshot {SnapshotId} was not found on disk.", snapshotId);
            return null;
        }

        var json = await File.ReadAllTextAsync(snapshotPath, cancellationToken);
        _logger.LogInformation("Loaded snapshot {SnapshotId} from {SnapshotPath}", snapshotId, snapshotPath);
        return _serializer.DeserializeSnapshot(json);
    }

    public async Task<IReadOnlyList<RecentSnapshotRecord>> ListRecentAsync(
        SnapshotPathResolver pathResolver,
        int take,
        CancellationToken cancellationToken = default) {
        var index = await ReadRecentIndexAsync(pathResolver, cancellationToken);
        return index.Entries
            .OrderByDescending(item => item.CreatedUtc)
            .ThenBy(item => item.SnapshotId, StringComparer.Ordinal)
            .Take(take)
            .ToArray();
    }

    public async Task StoreAsync(
        SnapshotPathResolver pathResolver,
        ArchitectureSnapshot snapshot,
        string requestHash,
        IReadOnlyList<PreparedExport> exports,
        CancellationToken cancellationToken = default) {
        _logger.LogInformation("Storing snapshot {SnapshotId} with {ExportCount} exports.", snapshot.SnapshotId, exports.Count);
        var snapshotDirectory = pathResolver.GetSnapshotDirectory(snapshot.SnapshotId);
        Directory.CreateDirectory(snapshotDirectory);

        foreach (var export in exports.OrderBy(item => item.RelativePath, StringComparer.Ordinal)) {
            var filePath = ResolveExportPath(snapshotDirectory, export.RelativePath);
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrWhiteSpace(directory)) {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(filePath, export.Content, Encoding.UTF8, cancellationToken);
        }

        var snapshotPath = pathResolver.GetSnapshotJsonPath(snapshot.SnapshotId);
        var snapshotJson = _serializer.Serialize(snapshot);
        await File.WriteAllTextAsync(snapshotPath, snapshotJson, Encoding.UTF8, cancellationToken);

        await WriteCacheIndexAsync(pathResolver, requestHash, snapshot.SnapshotId, cancellationToken);
        await WriteRecentIndexAsync(pathResolver, snapshot, cancellationToken);
        _logger.LogInformation("Stored snapshot {SnapshotId} in {SnapshotDirectory}", snapshot.SnapshotId, snapshotDirectory);
    }

    private async Task<SnapshotCacheIndex> ReadCacheIndexAsync(
        SnapshotPathResolver pathResolver,
        CancellationToken cancellationToken) {
        var path = pathResolver.GetCacheIndexPath();
        if (!File.Exists(path)) {
            return SnapshotCacheIndex.Empty;
        }

        var json = await File.ReadAllTextAsync(path, cancellationToken);
        return _serializer.Deserialize<SnapshotCacheIndex>(json);
    }

    private async Task<RecentSnapshotIndex> ReadRecentIndexAsync(
        SnapshotPathResolver pathResolver,
        CancellationToken cancellationToken) {
        var path = pathResolver.GetRecentIndexPath();
        if (!File.Exists(path)) {
            return RecentSnapshotIndex.Empty;
        }

        var json = await File.ReadAllTextAsync(path, cancellationToken);
        return _serializer.Deserialize<RecentSnapshotIndex>(json);
    }

    private async Task WriteCacheIndexAsync(
        SnapshotPathResolver pathResolver,
        string requestHash,
        string snapshotId,
        CancellationToken cancellationToken) {
        var current = await ReadCacheIndexAsync(pathResolver, cancellationToken);
        var entries = current.Entries
            .Where(entry => !string.Equals(entry.RequestHash, requestHash, StringComparison.Ordinal))
            .Append(new SnapshotCacheEntry(requestHash, snapshotId))
            .OrderBy(entry => entry.RequestHash, StringComparer.Ordinal)
            .ToArray();

        var index = new SnapshotCacheIndex(entries);
        var path = pathResolver.GetCacheIndexPath();
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await File.WriteAllTextAsync(path, _serializer.Serialize(index), Encoding.UTF8, cancellationToken);
    }

    private async Task WriteRecentIndexAsync(
        SnapshotPathResolver pathResolver,
        ArchitectureSnapshot snapshot,
        CancellationToken cancellationToken) {
        var current = await ReadRecentIndexAsync(pathResolver, cancellationToken);
        var entry = new RecentSnapshotRecord(
            snapshot.SnapshotId,
            snapshot.Facts.Solution.Name,
            snapshot.Request.SolutionPath,
            snapshot.CreatedUtc,
            snapshot.Insights.Findings.Count,
            snapshot.Diagnostics.Count);

        var entries = current.Entries
            .Where(item => !string.Equals(item.SnapshotId, snapshot.SnapshotId, StringComparison.Ordinal))
            .Append(entry)
            .OrderByDescending(item => item.CreatedUtc)
            .ThenBy(item => item.SnapshotId, StringComparer.Ordinal)
            .Take(50)
            .ToArray();

        var index = new RecentSnapshotIndex(entries);
        var path = pathResolver.GetRecentIndexPath();
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await File.WriteAllTextAsync(path, _serializer.Serialize(index), Encoding.UTF8, cancellationToken);
    }

    private static string ResolveExportPath(string snapshotDirectory, string relativePath) {
        if (string.IsNullOrWhiteSpace(relativePath)) {
            throw new InvalidOperationException("Export relative path cannot be empty.");
        }

        var snapshotRoot = Path.GetFullPath(snapshotDirectory);
        var exportPath = Path.GetFullPath(
            Path.Combine(snapshotRoot, relativePath.Replace('/', Path.DirectorySeparatorChar)));
        if (!IsPathWithinDirectory(exportPath, snapshotRoot)) {
            throw new InvalidOperationException($"Export path '{relativePath}' resolves outside the snapshot directory.");
        }

        return exportPath;
    }

    private static bool IsPathWithinDirectory(string candidatePath, string directoryPath) {
        var comparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;
        var directory = Path.TrimEndingDirectorySeparator(Path.GetFullPath(directoryPath));
        var candidate = Path.GetFullPath(candidatePath);

        return string.Equals(candidate, directory, comparison)
            || candidate.StartsWith(directory + Path.DirectorySeparatorChar, comparison);
    }
}
