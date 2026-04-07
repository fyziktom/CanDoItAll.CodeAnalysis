using CanDoItAll.CodeAnalytics.Analysis.Graphs;

namespace CanDoItAll.CodeAnalytics.Tests.Unit;

public sealed class DependencyGraphFacts {
    [Fact]
    public void DependencyGraph_detects_a_cycle() {
        var finder = new StronglyConnectedComponentFinder();
        var adjacency = new Dictionary<string, IReadOnlyList<string>>(StringComparer.Ordinal) {
            ["mod-orders"] = ["mod-notifications"],
            ["mod-notifications"] = ["mod-orders"],
            ["mod-persistence"] = [],
        };

        var cycles = finder.FindCycles(adjacency);

        Assert.Single(cycles);
        Assert.Equal(["mod-notifications", "mod-orders"], cycles[0]);
    }
}
