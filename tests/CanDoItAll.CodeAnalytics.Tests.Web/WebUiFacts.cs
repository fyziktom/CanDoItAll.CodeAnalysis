using System.Net;
using System.Text.RegularExpressions;
using CanDoItAll.CodeAnalytics.Tests.Support;
using CanDoItAll.CodeAnalytics.Web;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CanDoItAll.CodeAnalytics.Tests.Web;

public sealed class WebUiFacts {
    private static readonly Regex SnapshotPathPattern = new("href=\"(?<path>/snapshots/[^\"]+)\"", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex ContextPathPattern = new("href=\"(?<path>/snapshots/[^\"]+/context\\?[^\"]+)\"", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    [Fact]
    public async Task Home_route_renders_workspace_picker_controls() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        using var factory = new CodeAnalyticsWebFactory(output.Path, FixturePaths.GetFixtureSolutionPath());
        using var client = factory.CreateClient();

        var html = await client.GetStringAsync("/");

        Assert.Contains("Solution or project path", html, StringComparison.Ordinal);
        Assert.Contains("Browse", html, StringComparison.Ordinal);
        Assert.Contains("Project filter", html, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Dashboard_route_renders_after_analysis() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        using var factory = new CodeAnalyticsWebFactory(output.Path, FixturePaths.GetFixtureSolutionPath());
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var operationPath = await StartAnalysisAsync(client, FixturePaths.GetFixtureSolutionPath());
        var snapshotPath = await WaitForSnapshotPathAsync(client, operationPath);
        var dashboard = await client.GetStringAsync(snapshotPath);

        Assert.Contains("Snapshot dashboard", dashboard, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Fixture.Shop", dashboard, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Project_path_analysis_renders_project_dashboard() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        using var factory = new CodeAnalyticsWebFactory(output.Path, FixturePaths.GetFixtureProjectPath("Fixture.Shop.Infrastructure"));
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var operationPath = await StartAnalysisAsync(client, FixturePaths.GetFixtureProjectPath("Fixture.Shop.Infrastructure"));
        var snapshotPath = await WaitForSnapshotPathAsync(client, operationPath);
        var dashboard = await client.GetStringAsync(snapshotPath);

        Assert.Contains("Fixture.Shop.Infrastructure", dashboard, StringComparison.Ordinal);
        Assert.Contains(">1<", dashboard, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Drilldown_routes_render_after_analysis() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        using var factory = new CodeAnalyticsWebFactory(output.Path, FixturePaths.GetFixtureSolutionPath());
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var operationPath = await StartAnalysisAsync(client, FixturePaths.GetFixtureSolutionPath());
        var snapshotPath = await WaitForSnapshotPathAsync(client, operationPath);
        var snapshotId = snapshotPath.Trim('/').Split('/').Last();

        var dependencies = await client.GetStringAsync($"/snapshots/{snapshotId}/dependencies");
        var services = await client.GetStringAsync($"/snapshots/{snapshotId}/services");
        var persistence = await client.GetStringAsync($"/snapshots/{snapshotId}/persistence");
        var types = await client.GetStringAsync($"/snapshots/{snapshotId}/types?project=Fixture.Shop.Application&memberSearch=PlaceOrderAsync&includeMembers=true&methodsOnly=true");
        var symbols = await client.GetStringAsync($"/snapshots/{snapshotId}/symbols?search=IOrderService&mode=Exact");
        var findings = await client.GetStringAsync($"/snapshots/{snapshotId}/findings");

        Assert.Contains("Dependencies", dependencies, StringComparison.Ordinal);
        Assert.Contains("Services", services, StringComparison.Ordinal);
        Assert.Contains("Persistence", persistence, StringComparison.Ordinal);
        Assert.Contains("Type Explorer", types, StringComparison.Ordinal);
        Assert.Contains("PlaceOrderAsync", types, StringComparison.Ordinal);
        Assert.Contains("Definition Search", symbols, StringComparison.Ordinal);
        Assert.Contains("Findings", findings, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Export_route_serves_generated_summary() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        using var factory = new CodeAnalyticsWebFactory(output.Path, FixturePaths.GetFixtureSolutionPath());
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var operationPath = await StartAnalysisAsync(client, FixturePaths.GetFixtureSolutionPath());
        var snapshotPath = await WaitForSnapshotPathAsync(client, operationPath);
        var snapshotId = snapshotPath.Trim('/').Split('/').Last();

        using var response = await client.GetAsync($"/exports/{snapshotId}/exports/summary.md");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/markdown", response.Content.Headers.ContentType?.MediaType);
        Assert.Contains("Architecture Summary", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Focused_context_route_renders_after_analysis() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        using var factory = new CodeAnalyticsWebFactory(output.Path, FixturePaths.GetFixtureSolutionPath());
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var operationPath = await StartAnalysisAsync(client, FixturePaths.GetFixtureSolutionPath());
        var snapshotPath = await WaitForSnapshotPathAsync(client, operationPath);
        var snapshotId = snapshotPath.Trim('/').Split('/').Last();
        var services = await client.GetStringAsync($"/snapshots/{snapshotId}/services");
        var contextPath = ExtractContextPath(services);

        var context = await client.GetStringAsync(contextPath);

        Assert.Contains("Focused Context", context, StringComparison.Ordinal);
        Assert.Contains("Member relations", context, StringComparison.Ordinal);
        Assert.Contains("Selection reasons", context, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Focused_context_lab_route_renders_grouped_file_excerpts() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        using var factory = new CodeAnalyticsWebFactory(output.Path, FixturePaths.GetFixtureSolutionPath());
        using var client = factory.CreateClient();

        var route = $"/context-lab?workspacePath={Uri.EscapeDataString(FixturePaths.GetFixtureSolutionPath())}&projectFilter={Uri.EscapeDataString("Fixture.Shop.Application")}&queryText={Uri.EscapeDataString("PlaceOrderAsync")}&tags={Uri.EscapeDataString("EntityFramework")}&relationHints={Uri.EscapeDataString("OrderService")}&depth=2";
        var html = await client.GetStringAsync(route);

        Assert.Contains("Focused Context Lab", html, StringComparison.Ordinal);
        Assert.Contains("Relation hints", html, StringComparison.Ordinal);
        Assert.Contains("entityframework", html, StringComparison.Ordinal);
        Assert.Contains("orderservice", html, StringComparison.Ordinal);
        Assert.Contains("Selected Files", html, StringComparison.Ordinal);
        Assert.Contains("OrderService.cs", html, StringComparison.Ordinal);
        Assert.Contains("PlaceOrderAsync", html, StringComparison.Ordinal);
        Assert.Contains("selected /", html, StringComparison.Ordinal);
        Assert.Contains("Selection quality", html, StringComparison.Ordinal);
        Assert.Contains("Intent", html, StringComparison.Ordinal);
        Assert.Contains("Precision", html, StringComparison.Ordinal);
        Assert.Contains("Focused", html, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Focused_context_lab_route_renders_outline_mode_without_code_excerpts() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        using var factory = new CodeAnalyticsWebFactory(output.Path, FixturePaths.GetFixtureSolutionPath());
        using var client = factory.CreateClient();

        var route = $"/context-lab?workspacePath={Uri.EscapeDataString(FixturePaths.GetFixtureSolutionPath())}&projectFilter={Uri.EscapeDataString("Fixture.Shop.Application")}&queryText={Uri.EscapeDataString("PlaceOrderAsync")}&tags={Uri.EscapeDataString("Db")}&depth=2&precision=Outline";
        var html = await client.GetStringAsync(route);

        Assert.Contains("Outline", html, StringComparison.Ordinal);
        Assert.Contains("Selection Reasons", html, StringComparison.Ordinal);
        Assert.Contains("Outline precision intentionally suppresses code excerpts", html, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Symbol_explorer_route_renders_selected_symbol_details() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        using var factory = new CodeAnalyticsWebFactory(output.Path, FixturePaths.GetFixtureSolutionPath());
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var operationPath = await StartAnalysisAsync(client, FixturePaths.GetFixtureSolutionPath());
        var snapshotPath = await WaitForSnapshotPathAsync(client, operationPath);
        var snapshotId = snapshotPath.Trim('/').Split('/').Last();
        var service = ApplicationServiceFactory.Create(output.Path);
        var snapshot = await service.GetSnapshotAsync(snapshotId);
        var type = Assert.Single(snapshot!.Facts.Types, item => string.Equals(item.DisplayName, "Fixture.Shop.Contracts.Orders.IOrderService", StringComparison.Ordinal));
        var inspectPath = $"/snapshots/{snapshotId}/symbols?search=IOrderService&mode=Exact&typeId={Uri.EscapeDataString(type.TypeId)}";

        var details = await client.GetStringAsync(inspectPath);

        Assert.Contains("Definition", details, StringComparison.Ordinal);
        Assert.Contains("Implementations", details, StringComparison.Ordinal);
        Assert.Contains("References", details, StringComparison.Ordinal);
        Assert.Contains("IOrderService", details, StringComparison.Ordinal);
        Assert.Contains("OrderService", details, StringComparison.Ordinal);
    }

    private static async Task<string> StartAnalysisAsync(HttpClient client, string workspacePath) {
        using var request = new FormUrlEncodedContent(
            new Dictionary<string, string> {
                ["solutionPath"] = workspacePath,
                ["includeDi"] = "on",
                ["includePersistence"] = "on",
                ["includeRisks"] = "on",
                ["includeXmlDocs"] = "on",
                ["includeMermaidExports"] = "on",
            });

        using var response = await client.PostAsync("/analyze", request);
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.StartsWith("/operations/", response.Headers.Location!.OriginalString, StringComparison.Ordinal);
        return response.Headers.Location.OriginalString;
    }

    private static async Task<string> WaitForSnapshotPathAsync(HttpClient client, string operationPath) {
        string? lastPage = null;

        for (var attempt = 0; attempt < 120; attempt++) {
            lastPage = await client.GetStringAsync(operationPath);
            var match = SnapshotPathPattern.Match(lastPage);
            if (match.Success) {
                return match.Groups["path"].Value;
            }

            await Task.Delay(250);
        }

        throw new InvalidOperationException($"Operation did not complete in time. Last page:{Environment.NewLine}{lastPage}");
    }

    private static string ExtractContextPath(string html) {
        var match = ContextPathPattern.Match(html);
        if (!match.Success) {
            throw new InvalidOperationException($"No focused-context link was found.{Environment.NewLine}{html}");
        }

        return match.Groups["path"].Value;
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
