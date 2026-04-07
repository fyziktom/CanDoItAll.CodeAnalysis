using CanDoItAll.CodeAnalytics.Abstractions.Commands;
using CanDoItAll.CodeAnalytics.Tests.Support;

namespace CanDoItAll.CodeAnalytics.Tests.Integration;

public sealed class PersistenceFacts {
    [Fact]
    public async Task Persistence_snapshot_contains_er_ready_facts() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        var service = ApplicationServiceFactory.Create(output.Path);

        var response = await service.BuildSnapshotAsync(new BuildArchitectureSnapshotCommand(FixturePaths.GetFixtureSolutionPath(), ForceRefresh: true));

        Assert.Contains(response.Snapshot.Facts.Entities, entity => entity.DisplayName == "Order" && entity.TableName == "Orders");
        Assert.Contains(response.Snapshot.Facts.Entities, entity => entity.RelationshipTargets.Count > 0);
    }
}
