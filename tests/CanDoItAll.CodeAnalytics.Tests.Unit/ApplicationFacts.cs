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

    [Fact]
    public async Task Application_builds_a_project_scoped_snapshot_from_a_csproj_path() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        var service = ApplicationServiceFactory.Create(output.Path);

        var response = await service.BuildSnapshotAsync(
            new BuildArchitectureSnapshotCommand(
                FixturePaths.GetFixtureProjectPath("Fixture.Shop.Infrastructure"),
                ForceRefresh: true));

        Assert.Equal("Fixture.Shop.Infrastructure", response.Snapshot.Facts.Solution.Name);
        Assert.Single(response.Snapshot.Facts.Projects);
        Assert.Equal("Fixture.Shop.Infrastructure", response.Snapshot.Facts.Projects[0].Name);
        Assert.All(
            response.Snapshot.Facts.Types,
            type => Assert.Equal(response.Snapshot.Facts.Projects[0].ProjectId, type.ProjectId));
    }

    [Fact]
    public async Task Application_filters_types_by_project_and_can_expand_methods() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        var service = ApplicationServiceFactory.Create(output.Path);

        var build = await service.BuildSnapshotAsync(new BuildArchitectureSnapshotCommand(FixturePaths.GetFixtureSolutionPath(), ForceRefresh: true));
        var response = await service.GetTypesAsync(
            new TypeSearchQuery(
                build.Snapshot.SnapshotId,
                ProjectName: "Fixture.Shop.Application",
                MemberSearchText: "PlaceOrderAsync",
                IncludeMembers: true,
                MethodsOnly: true));

        Assert.NotNull(response);
        Assert.NotEmpty(response!.Types);
        Assert.All(response.Types, item => Assert.Equal("Fixture.Shop.Application", item.ProjectName));
        Assert.Contains(
            response.Types.SelectMany(item => item.Members),
            member => member.Kind == CanDoItAll.CodeAnalytics.Domain.Facts.MemberKind.Method &&
                member.DisplayName.Contains("PlaceOrderAsync", StringComparison.Ordinal));
    }
}
