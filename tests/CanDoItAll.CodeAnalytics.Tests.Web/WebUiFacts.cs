using System.Net;
using System.Net.Http.Headers;
using CanDoItAll.CodeAnalytics.Tests.Support;
using CanDoItAll.CodeAnalytics.Web;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CanDoItAll.CodeAnalytics.Tests.Web;

public sealed class WebUiFacts {
    [Fact]
    public async Task Dashboard_route_renders_after_analysis() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        using var factory = new CodeAnalyticsWebFactory(output.Path, FixturePaths.GetFixtureSolutionPath());
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var redirect = await StartAnalysisAsync(client);
        var dashboard = await client.GetStringAsync(redirect);

        Assert.Contains("Snapshot dashboard", dashboard, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Fixture.Shop", dashboard, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Drilldown_routes_render_after_analysis() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        using var factory = new CodeAnalyticsWebFactory(output.Path, FixturePaths.GetFixtureSolutionPath());
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var redirect = await StartAnalysisAsync(client);
        var snapshotId = redirect.Trim('/').Split('/').Last();

        var dependencies = await client.GetStringAsync($"/snapshots/{snapshotId}/dependencies");
        var services = await client.GetStringAsync($"/snapshots/{snapshotId}/services");
        var persistence = await client.GetStringAsync($"/snapshots/{snapshotId}/persistence");
        var findings = await client.GetStringAsync($"/snapshots/{snapshotId}/findings");

        Assert.Contains("Dependencies", dependencies, StringComparison.Ordinal);
        Assert.Contains("Services", services, StringComparison.Ordinal);
        Assert.Contains("Persistence", persistence, StringComparison.Ordinal);
        Assert.Contains("Findings", findings, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Export_route_serves_generated_summary() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        using var factory = new CodeAnalyticsWebFactory(output.Path, FixturePaths.GetFixtureSolutionPath());
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var redirect = await StartAnalysisAsync(client);
        var snapshotId = redirect.Trim('/').Split('/').Last();

        using var response = await client.GetAsync($"/exports/{snapshotId}/exports/summary.md");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/markdown", response.Content.Headers.ContentType?.MediaType);
        Assert.Contains("Architecture Summary", content, StringComparison.Ordinal);
    }

    private static async Task<string> StartAnalysisAsync(HttpClient client) {
        using var request = new FormUrlEncodedContent(
            new Dictionary<string, string> {
                ["solutionPath"] = FixturePaths.GetFixtureSolutionPath(),
                ["includeDi"] = "on",
                ["includePersistence"] = "on",
                ["includeRisks"] = "on",
                ["includeXmlDocs"] = "on",
                ["includeMermaidExports"] = "on",
            });

        using var response = await client.PostAsync("/analyze", request);
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        return response.Headers.Location!.OriginalString;
    }

    private sealed class CodeAnalyticsWebFactory : WebApplicationFactory<Program> {
        private readonly string? _previousDefaultSolutionPath;
        private readonly string? _previousOutputRoot;

        public CodeAnalyticsWebFactory(string outputRoot, string defaultSolutionPath) {
            _previousDefaultSolutionPath = Environment.GetEnvironmentVariable("CODE_ANALYTICS_DEFAULT_SOLUTION_PATH");
            _previousOutputRoot = Environment.GetEnvironmentVariable("CODE_ANALYTICS_OUTPUT_ROOT");

            Environment.SetEnvironmentVariable("CODE_ANALYTICS_DEFAULT_SOLUTION_PATH", defaultSolutionPath);
            Environment.SetEnvironmentVariable("CODE_ANALYTICS_OUTPUT_ROOT", outputRoot);
        }

        protected override void Dispose(bool disposing) {
            Environment.SetEnvironmentVariable("CODE_ANALYTICS_DEFAULT_SOLUTION_PATH", _previousDefaultSolutionPath);
            Environment.SetEnvironmentVariable("CODE_ANALYTICS_OUTPUT_ROOT", _previousOutputRoot);
            base.Dispose(disposing);
        }
    }
}
