using CanDoItAll.CodeAnalytics.Domain.Exports;
using CanDoItAll.CodeAnalytics.Storage.Paths;
using CanDoItAll.CodeAnalytics.Storage.Snapshots;
using CanDoItAll.CodeAnalytics.Tests.Support;

namespace CanDoItAll.CodeAnalytics.Tests.Unit;

public sealed class SnapshotRepositoryFacts {
    [Fact]
    public async Task SnapshotRepository_stores_and_reloads_a_snapshot() {
        var serializer = new SnapshotJsonSerializer();
        var repository = new FileSnapshotRepository(serializer);
        var snapshot = SampleSnapshotFactory.Create();
        using var output = new TemporaryDirectoryScope();
        var pathResolver = new SnapshotPathResolver(output.Path);
        var requestHash = repository.ComputeRequestHash(snapshot.Request, snapshot.GeneratorVersion, snapshot.SchemaVersion);

        await repository.StoreAsync(pathResolver, snapshot, requestHash, [], CancellationToken.None);

        var loaded = await repository.LoadSnapshotAsync(pathResolver, snapshot.SnapshotId, CancellationToken.None);
        Assert.NotNull(loaded);
        SnapshotAssert.Equal(snapshot, loaded);
    }

    [Fact]
    public async Task SnapshotRepository_rejects_export_paths_outside_snapshot_directory() {
        var serializer = new SnapshotJsonSerializer();
        var repository = new FileSnapshotRepository(serializer);
        var snapshot = SampleSnapshotFactory.Create();
        using var output = new TemporaryDirectoryScope();
        var pathResolver = new SnapshotPathResolver(output.Path);
        var requestHash = repository.ComputeRequestHash(snapshot.Request, snapshot.GeneratorVersion, snapshot.SchemaVersion);
        var export = new PreparedExport(
            ExportArtifactKind.MarkdownSummary,
            "../escape.md",
            "Escaping export",
            "Should not be written outside the snapshot directory.",
            "escape");

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => repository.StoreAsync(pathResolver, snapshot, requestHash, [export], CancellationToken.None));

        Assert.Contains("outside the snapshot directory", exception.Message, StringComparison.Ordinal);
        Assert.False(File.Exists(Path.Combine(output.Path, "snapshots", "escape.md")));
    }
}
