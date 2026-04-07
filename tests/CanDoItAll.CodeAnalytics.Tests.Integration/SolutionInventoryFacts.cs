using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Tests.Support;
using CanDoItAll.CodeAnalytics.Workspace.Inventory;
using CanDoItAll.CodeAnalytics.Workspace.Loading;
using CanDoItAll.CodeAnalytics.Workspace.Normalization;

namespace CanDoItAll.CodeAnalytics.Tests.Integration;

public sealed class SolutionInventoryFacts {
    [Fact]
    public async Task SolutionInventory_is_deterministic_and_reports_invalid_paths() {
        FixtureSolutionHost.EnsurePrepared();
        var loader = new MsBuildWorkspaceLoader(new AnalysisRequestNormalizer(), new ProjectFileInventoryReader());

        using var first = await loader.LoadAsync(new AnalysisRequest(FixturePaths.GetFixtureSolutionPath(), [], [], true, true, true, true, true));
        using var second = await loader.LoadAsync(new AnalysisRequest(FixturePaths.GetFixtureSolutionPath(), [], [], true, true, true, true, true));
        using var invalid = await loader.LoadAsync(new AnalysisRequest("missing.slnx", [], [], true, true, true, true, true));

        Assert.Equal(first.Projects.Select(project => project.Name), second.Projects.Select(project => project.Name));
        Assert.Contains(invalid.Diagnostics, diagnostic => diagnostic.Code == "WS0001");
    }
}
