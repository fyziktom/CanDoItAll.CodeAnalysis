using CanDoItAll.CodeAnalytics.Abstractions.Commands;
using CanDoItAll.CodeAnalytics.Tests.Support;

namespace CanDoItAll.CodeAnalytics.Tests.Integration;

public sealed class SnapshotAssemblyFacts {
    [Fact]
    public async Task SnapshotAssembly_writes_snapshot_and_export_files() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        var service = ApplicationServiceFactory.Create(output.Path);

        var response = await service.BuildSnapshotAsync(new BuildArchitectureSnapshotCommand(FixturePaths.GetFixtureSolutionPath(), ForceRefresh: true));
        var snapshotDirectory = Path.Combine(output.Path, "snapshots", response.Snapshot.SnapshotId);

        Assert.True(File.Exists(Path.Combine(snapshotDirectory, "snapshot.json")));
        Assert.True(File.Exists(Path.Combine(snapshotDirectory, "exports", "summary.md")));
        Assert.NotEmpty(Directory.GetFiles(Path.Combine(snapshotDirectory, "exports", "class-diagrams"), "*.mmd"));
        Assert.NotEmpty(Directory.GetFiles(Path.Combine(snapshotDirectory, "exports", "er-diagrams"), "*.mmd"));
    }
}
