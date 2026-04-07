using System.Text.Json;
using CanDoItAll.CodeAnalytics.Tests.Support;

namespace CanDoItAll.CodeAnalytics.Tests.Architecture;

public sealed class FutureMcpFacts {
    [Fact]
    public void FutureMcp_tool_surface_and_settings_keep_the_driver_thin() {
        var repoRoot = RepositoryRootLocator.FindRepositoryRoot();
        var toolSurfacePath = Path.Combine(repoRoot, "reference", "tool-surface-proposal.json");
        var settingsPath = Path.Combine(repoRoot, "reference", "CanDoItAll.Mcp.CodeAnalytics.settings.example.json");

        using var toolSurface = JsonDocument.Parse(File.ReadAllText(toolSurfacePath));
        using var settings = JsonDocument.Parse(File.ReadAllText(settingsPath));

        Assert.Equal("CanDoItAll.Mcp.CodeAnalytics", toolSurface.RootElement.GetProperty("driverProject").GetString());
        Assert.Equal("CanDoItAll.Mcp.CodeAnalytics", settings.RootElement.GetProperty("Server").GetProperty("Name").GetString());
    }

    [Fact]
    public void FutureMcp_standalone_source_tree_does_not_clone_host_runtime_projects() {
        var repoRoot = RepositoryRootLocator.FindRepositoryRoot();
        var sourceTree = Directory.GetFiles(Path.Combine(repoRoot, "src"), "*.cs", SearchOption.AllDirectories)
            .Select(File.ReadAllText)
            .ToArray();

        Assert.DoesNotContain(sourceTree, content => content.Contains("McpToolEnvelope", StringComparison.Ordinal));
        Assert.DoesNotContain(sourceTree, content => content.Contains("CanDoItAll.Mcp.Core", StringComparison.Ordinal));
    }
}
