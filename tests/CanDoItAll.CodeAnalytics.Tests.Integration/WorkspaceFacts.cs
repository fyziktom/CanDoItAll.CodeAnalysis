using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Tests.Support;
using CanDoItAll.CodeAnalytics.Workspace.Inventory;
using CanDoItAll.CodeAnalytics.Workspace.Loading;
using CanDoItAll.CodeAnalytics.Workspace.Normalization;

namespace CanDoItAll.CodeAnalytics.Tests.Integration;

public sealed class WorkspaceFacts {
    [Fact]
    public async Task Workspace_loads_the_fixture_solution() {
        FixtureSolutionHost.EnsurePrepared();
        var loader = new MsBuildWorkspaceLoader(new AnalysisRequestNormalizer(), new ProjectFileInventoryReader());

        using var result = await loader.LoadAsync(
            new AnalysisRequest(FixturePaths.GetFixtureSolutionPath(), [], [], true, true, true, true, true));

        Assert.NotNull(result.Solution);
        Assert.False(result.HasBlockingErrors);
        Assert.True(result.Projects.Count >= 4);
    }

    [Fact]
    public async Task Workspace_loads_a_fixture_project_and_applies_project_scope() {
        FixtureSolutionHost.EnsurePrepared();
        var loader = new MsBuildWorkspaceLoader(new AnalysisRequestNormalizer(), new ProjectFileInventoryReader());

        using var result = await loader.LoadAsync(
            new AnalysisRequest(FixturePaths.GetFixtureProjectPath("Fixture.Shop.Infrastructure"), [], [], true, true, true, true, true));

        Assert.NotNull(result.Solution);
        Assert.False(result.HasBlockingErrors);
        Assert.Single(result.Projects);
        Assert.Single(result.Request.ScopeProjectNames);
        Assert.Equal("Fixture.Shop.Infrastructure", result.Request.ScopeProjectNames[0]);
        Assert.Equal("Fixture.Shop.Infrastructure", result.Projects[0].Name);
    }
}
