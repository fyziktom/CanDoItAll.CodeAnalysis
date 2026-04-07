using CanDoItAll.CodeAnalytics.Abstractions.Commands;
using CanDoItAll.CodeAnalytics.Abstractions.Queries;
using CanDoItAll.CodeAnalytics.Tests.Support;

namespace CanDoItAll.CodeAnalytics.Tests.Unit;

public sealed class ApplicationFacts {
    [Fact]
    public async Task Application_builds_and_queries_a_fixture_snapshot() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        var service = ApplicationServiceFactory.Create(output.Path);

        var build = await service.BuildSnapshotAsync(new BuildArchitectureSnapshotCommand(FixturePaths.GetFixtureSolutionPath(), ForceRefresh: true));
        var dashboard = await service.GetDashboardAsync(build.Snapshot.SnapshotId);
        var findings = await service.GetFindingsAsync(new SnapshotQuery(build.Snapshot.SnapshotId));

        Assert.NotNull(dashboard);
        Assert.NotNull(findings);
        Assert.Equal(build.Snapshot.SnapshotId, dashboard!.Snapshot.SnapshotId);
        Assert.True(findings!.Findings.Count > 0 || findings.OpenQuestions.Count > 0);
    }
}
