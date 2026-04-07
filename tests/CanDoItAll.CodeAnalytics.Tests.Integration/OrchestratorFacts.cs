using CanDoItAll.CodeAnalytics.Abstractions.Commands;
using CanDoItAll.CodeAnalytics.Tests.Support;

namespace CanDoItAll.CodeAnalytics.Tests.Integration;

public sealed class OrchestratorFacts {
    [Fact]
    public async Task Orchestrator_reuses_cached_snapshots_and_updates_recent_history() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        var service = ApplicationServiceFactory.Create(output.Path);
        var command = new BuildArchitectureSnapshotCommand(FixturePaths.GetFixtureSolutionPath());

        var first = await service.BuildSnapshotAsync(command);
        var second = await service.BuildSnapshotAsync(command);
        var recent = await service.ListRecentSnapshotsAsync(10);

        Assert.False(first.FromCache);
        Assert.True(second.FromCache);
        Assert.Contains(recent, item => item.SnapshotId == first.Snapshot.SnapshotId);
    }
}
