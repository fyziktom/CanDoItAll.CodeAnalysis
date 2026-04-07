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
}
