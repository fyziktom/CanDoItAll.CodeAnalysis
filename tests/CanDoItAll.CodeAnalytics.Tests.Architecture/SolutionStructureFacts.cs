using System.Xml.Linq;
using CanDoItAll.CodeAnalytics.Tests.Support;

namespace CanDoItAll.CodeAnalytics.Tests.Architecture;

public sealed class SolutionStructureFacts {
    [Fact]
    public void Production_projects_follow_the_bootstrap_reference_map() {
        var repoRoot = RepositoryRootLocator.FindRepositoryRoot();
        var expectedGraph = new Dictionary<string, string[]>(StringComparer.Ordinal) {
            ["CanDoItAll.CodeAnalytics.Abstractions"] = ["CanDoItAll.CodeAnalytics.Domain"],
            ["CanDoItAll.CodeAnalytics.Analysis"] = ["CanDoItAll.CodeAnalytics.Domain", "CanDoItAll.CodeAnalytics.Facts"],
            ["CanDoItAll.CodeAnalytics.Application"] =
            [
                "CanDoItAll.CodeAnalytics.Abstractions",
                "CanDoItAll.CodeAnalytics.Analysis",
                "CanDoItAll.CodeAnalytics.Domain",
                "CanDoItAll.CodeAnalytics.Facts",
                "CanDoItAll.CodeAnalytics.Rendering",
                "CanDoItAll.CodeAnalytics.Storage",
                "CanDoItAll.CodeAnalytics.Workspace"
            ],
            ["CanDoItAll.CodeAnalytics.Domain"] = [],
            ["CanDoItAll.CodeAnalytics.Facts"] = ["CanDoItAll.CodeAnalytics.Domain", "CanDoItAll.CodeAnalytics.Workspace"],
            ["CanDoItAll.CodeAnalytics.Rendering"] = ["CanDoItAll.CodeAnalytics.Domain"],
            ["CanDoItAll.CodeAnalytics.Storage"] = ["CanDoItAll.CodeAnalytics.Domain"],
            ["CanDoItAll.CodeAnalytics.Web"] = ["CanDoItAll.CodeAnalytics.Application"],
            ["CanDoItAll.CodeAnalytics.Workspace"] = ["CanDoItAll.CodeAnalytics.Domain"]
        };

        foreach (var project in expectedGraph.OrderBy(static pair => pair.Key, StringComparer.Ordinal)) {
            var projectPath = Path.Combine(repoRoot, "src", project.Key, $"{project.Key}.csproj");
            var references = ReadProjectReferences(projectPath);

            Assert.Equal(project.Value.OrderBy(static value => value, StringComparer.Ordinal), references);
        }
    }

    [Fact]
    public void Canonical_solution_contains_the_expected_project_set() {
        var repoRoot = RepositoryRootLocator.FindRepositoryRoot();
        var solutionPath = Path.Combine(repoRoot, "CanDoItAll.CodeAnalsis.slnx");
        var expectedProjects = new[]
        {
            "src/CanDoItAll.CodeAnalytics.Abstractions/CanDoItAll.CodeAnalytics.Abstractions.csproj",
            "src/CanDoItAll.CodeAnalytics.Analysis/CanDoItAll.CodeAnalytics.Analysis.csproj",
            "src/CanDoItAll.CodeAnalytics.Application/CanDoItAll.CodeAnalytics.Application.csproj",
            "src/CanDoItAll.CodeAnalytics.Domain/CanDoItAll.CodeAnalytics.Domain.csproj",
            "src/CanDoItAll.CodeAnalytics.Facts/CanDoItAll.CodeAnalytics.Facts.csproj",
            "src/CanDoItAll.CodeAnalytics.Rendering/CanDoItAll.CodeAnalytics.Rendering.csproj",
            "src/CanDoItAll.CodeAnalytics.Storage/CanDoItAll.CodeAnalytics.Storage.csproj",
            "src/CanDoItAll.CodeAnalytics.Web/CanDoItAll.CodeAnalytics.Web.csproj",
            "src/CanDoItAll.CodeAnalytics.Workspace/CanDoItAll.CodeAnalytics.Workspace.csproj",
            "tests/CanDoItAll.CodeAnalytics.Tests.Architecture/CanDoItAll.CodeAnalytics.Tests.Architecture.csproj",
            "tests/CanDoItAll.CodeAnalytics.Tests.Integration/CanDoItAll.CodeAnalytics.Tests.Integration.csproj",
            "tests/CanDoItAll.CodeAnalytics.Tests.Support/CanDoItAll.CodeAnalytics.Tests.Support.csproj",
            "tests/CanDoItAll.CodeAnalytics.Tests.Unit/CanDoItAll.CodeAnalytics.Tests.Unit.csproj",
            "tests/CanDoItAll.CodeAnalytics.Tests.Web/CanDoItAll.CodeAnalytics.Tests.Web.csproj",
            "tools/ScenarioEvaluationHarness/ScenarioEvaluationHarness.csproj"
        };

        var document = XDocument.Load(solutionPath);
        var actualProjects = document
            .Descendants("Project")
            .Select(static element => (string?)element.Attribute("Path"))
            .Where(static path => !string.IsNullOrWhiteSpace(path))
            .Select(static path => path!.Replace("\\", "/"))
            .OrderBy(static path => path, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(expectedProjects.OrderBy(static path => path, StringComparer.Ordinal), actualProjects);
    }

    [Fact]
    public void Source_projects_do_not_reference_host_mcp_core() {
        var repoRoot = RepositoryRootLocator.FindRepositoryRoot();
        var projectFiles = Directory.GetFiles(Path.Combine(repoRoot, "src"), "*.csproj", SearchOption.AllDirectories)
            .OrderBy(static path => path, StringComparer.Ordinal)
            .ToArray();

        foreach (var projectFile in projectFiles) {
            var content = File.ReadAllText(projectFile);
            Assert.DoesNotContain("CanDoItAll.Mcp.Core", content);
        }
    }

    [Fact]
    public void Production_project_reference_graph_is_acyclic() {
        var repoRoot = RepositoryRootLocator.FindRepositoryRoot();
        var graph = ReadProductionProjectGraph(repoRoot);
        var visiting = new HashSet<string>(StringComparer.Ordinal);
        var visited = new HashSet<string>(StringComparer.Ordinal);
        var cycles = new List<string>();

        foreach (var project in graph.Keys.OrderBy(static key => key, StringComparer.Ordinal)) {
            Visit(project, []);
        }

        Assert.True(cycles.Count == 0, $"Project reference cycles were found: {string.Join("; ", cycles)}");

        void Visit(string project, string[] path) {
            if (visited.Contains(project)) {
                return;
            }

            if (!visiting.Add(project)) {
                cycles.Add(string.Join(" -> ", path.Append(project)));
                return;
            }

            foreach (var reference in graph[project]) {
                if (graph.ContainsKey(reference)) {
                    Visit(reference, [.. path, project]);
                }
            }

            visiting.Remove(project);
            visited.Add(project);
        }
    }

    [Fact]
    public void Reusable_source_projects_do_not_reference_sandbox_or_nonshipping_boundaries() {
        var repoRoot = RepositoryRootLocator.FindRepositoryRoot();
        var graph = ReadProductionProjectGraph(repoRoot);
        var forbiddenReferences = graph
            .Where(static pair => !string.Equals(pair.Key, "CanDoItAll.CodeAnalytics.Web", StringComparison.Ordinal))
            .SelectMany(static pair => pair.Value.Select(reference => $"{pair.Key} -> {reference}"))
            .Where(static edge =>
                edge.Contains("CanDoItAll.CodeAnalytics.Web", StringComparison.Ordinal) ||
                edge.Contains(".Tests.", StringComparison.Ordinal) ||
                edge.Contains("ScenarioEvaluationHarness", StringComparison.Ordinal) ||
                edge.Contains("ComparisonHarness", StringComparison.Ordinal))
            .OrderBy(static edge => edge, StringComparer.Ordinal)
            .ToArray();

        Assert.Empty(forbiddenReferences);
    }

    [Fact]
    public void Desktop_sandbox_is_the_only_web_sdk_source_project() {
        var repoRoot = RepositoryRootLocator.FindRepositoryRoot();
        var projectFiles = Directory.GetFiles(Path.Combine(repoRoot, "src"), "*.csproj", SearchOption.AllDirectories)
            .OrderBy(static path => path, StringComparer.Ordinal)
            .ToArray();

        foreach (var projectFile in projectFiles) {
            var document = XDocument.Load(projectFile);
            var projectName = Path.GetFileNameWithoutExtension(projectFile);
            var sdk = (string?)document.Root?.Attribute("Sdk") ?? string.Empty;

            if (string.Equals(projectName, "CanDoItAll.CodeAnalytics.Web", StringComparison.Ordinal)) {
                Assert.Equal("Microsoft.NET.Sdk.Web", sdk);
                continue;
            }

            Assert.DoesNotContain("Web", sdk, StringComparison.OrdinalIgnoreCase);
        }
    }

    private static string[] ReadProjectReferences(string projectPath) {
        var document = XDocument.Load(projectPath);

        return document
            .Descendants("ProjectReference")
            .Select(static reference => (string?)reference.Attribute("Include"))
            .Where(static include => !string.IsNullOrWhiteSpace(include))
            .Select(static include => Path.GetFileNameWithoutExtension(include)!)
            .OrderBy(static include => include, StringComparer.Ordinal)
            .ToArray();
    }

    private static Dictionary<string, string[]> ReadProductionProjectGraph(string repoRoot) {
        return Directory.GetFiles(Path.Combine(repoRoot, "src"), "*.csproj", SearchOption.AllDirectories)
            .ToDictionary(
                static projectPath => Path.GetFileNameWithoutExtension(projectPath),
                ReadProjectReferences,
                StringComparer.Ordinal);
    }
}
