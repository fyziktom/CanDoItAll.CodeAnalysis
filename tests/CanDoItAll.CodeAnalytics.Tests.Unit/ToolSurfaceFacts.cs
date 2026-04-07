using System.Text.Json;
using CanDoItAll.CodeAnalytics.Tests.Support;

namespace CanDoItAll.CodeAnalytics.Tests.Unit;

public sealed class ToolSurfaceFacts {
    [Fact]
    public void ToolSurface_reference_artifact_matches_the_expected_driver_shape() {
        var toolSurfacePath = Path.Combine(RepositoryRootLocator.FindRepositoryRoot(), "reference", "tool-surface-proposal.json");
        using var document = JsonDocument.Parse(File.ReadAllText(toolSurfacePath));

        Assert.Equal("CanDoItAll.Mcp.CodeAnalytics", document.RootElement.GetProperty("driverProject").GetString());
        Assert.Equal("code_analytics_", document.RootElement.GetProperty("toolPrefix").GetString());
    }
}
