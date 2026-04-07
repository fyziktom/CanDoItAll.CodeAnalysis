using System.Text.Json;
using CanDoItAll.CodeAnalytics.Tests.Support;

namespace CanDoItAll.CodeAnalytics.Tests.Architecture;

public sealed class CompatibilityContractFacts {
    [Fact]
    public void Compatibility_naming_map_stays_frozen() {
        var repoRoot = RepositoryRootLocator.FindRepositoryRoot();
        var sourceProjects = Directory.GetFiles(Path.Combine(repoRoot, "src"), "*.csproj", SearchOption.AllDirectories)
            .Select(Path.GetFileNameWithoutExtension)
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.True(File.Exists(Path.Combine(repoRoot, "CanDoItAll.CodeAnalsis.slnx")));
        Assert.All(sourceProjects, static name => {
            Assert.NotNull(name);
            Assert.StartsWith("CanDoItAll.CodeAnalytics.", name);
            Assert.DoesNotContain("CodeAnalsis", name);
        });
    }

    [Fact]
    public void Compatibility_reference_artifacts_exist() {
        var repoRoot = RepositoryRootLocator.FindRepositoryRoot();
        var requiredFiles = new[]
        {
            Path.Combine(repoRoot, "reference", "compatibility-matrix.md"),
            Path.Combine(repoRoot, "reference", "reuse-later-vs-do-not-duplicate-now.md"),
            Path.Combine(repoRoot, "reference", "current-candoitall-mcp-context.md"),
            Path.Combine(repoRoot, "reference", "current-candoitall-mcp-context.json"),
            Path.Combine(repoRoot, "reference", "tool-surface-proposal.json"),
            Path.Combine(repoRoot, "reference", "CanDoItAll.Mcp.CodeAnalytics.settings.example.json"),
            Path.Combine(repoRoot, "reference", "vscode-mcp-snippet.code-analytics.json")
        };

        Assert.All(requiredFiles, static path => Assert.True(File.Exists(path), $"Missing compatibility artifact: {path}"));
    }

    [Fact]
    public void Compatibility_tool_surface_uses_the_code_analytics_prefix() {
        var repoRoot = RepositoryRootLocator.FindRepositoryRoot();
        var toolSurfacePath = Path.Combine(repoRoot, "reference", "tool-surface-proposal.json");
        var settingsPath = Path.Combine(repoRoot, "reference", "CanDoItAll.Mcp.CodeAnalytics.settings.example.json");

        using var toolSurface = JsonDocument.Parse(File.ReadAllText(toolSurfacePath));
        using var settings = JsonDocument.Parse(File.ReadAllText(settingsPath));

        Assert.Equal("CanDoItAll.Mcp.CodeAnalytics", toolSurface.RootElement.GetProperty("driverProject").GetString());
        Assert.Equal("code_analytics_", toolSurface.RootElement.GetProperty("toolPrefix").GetString());
        Assert.Equal("CanDoItAll.Mcp.CodeAnalytics", settings.RootElement.GetProperty("Server").GetProperty("Name").GetString());

        var toolNames = toolSurface.RootElement.GetProperty("tools")
            .EnumerateArray()
            .Select(static tool => tool.GetProperty("name").GetString())
            .ToArray();

        Assert.All(toolNames, static toolName => {
            Assert.NotNull(toolName);
            Assert.StartsWith("code_analytics_", toolName);
        });
    }
}
