using CanDoItAll.CodeAnalytics.Abstractions;
using CanDoItAll.CodeAnalytics.Abstractions.Commands;
using CanDoItAll.CodeAnalytics.Abstractions.Options;
using CanDoItAll.CodeAnalytics.Analysis.Graphs;
using CanDoItAll.CodeAnalytics.Analysis.Rules;
using CanDoItAll.CodeAnalytics.Application.Services;
using CanDoItAll.CodeAnalytics.Facts.Dependencies;
using CanDoItAll.CodeAnalytics.Facts.Documentation;
using CanDoItAll.CodeAnalytics.Facts.Persistence;
using CanDoItAll.CodeAnalytics.Facts.Services;
using CanDoItAll.CodeAnalytics.Facts.Symbols;
using CanDoItAll.CodeAnalytics.Rendering.Exports;
using CanDoItAll.CodeAnalytics.Rendering.Markdown;
using CanDoItAll.CodeAnalytics.Rendering.Mermaid;
using CanDoItAll.CodeAnalytics.Storage.Snapshots;
using CanDoItAll.CodeAnalytics.Web.Components;
using CanDoItAll.CodeAnalytics.Web.State;
using CanDoItAll.CodeAnalytics.Workspace.Inventory;
using CanDoItAll.CodeAnalytics.Workspace.Loading;
using CanDoItAll.CodeAnalytics.Workspace.Normalization;

namespace CanDoItAll.CodeAnalytics.Web;

public class Program {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddRazorComponents();
        RegisterServices(builder.Services, builder.Environment.ContentRootPath);

        var app = builder.Build();

        if (!app.Environment.IsDevelopment()) {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
        app.UseHttpsRedirection();
        app.UseAntiforgery();

        app.MapPost("/analyze", HandleAnalyzeAsync);
        app.MapGet("/exports/{snapshotId}/{**relativePath}", HandleExportAsync);
        app.MapStaticAssets();
        app.MapRazorComponents<App>();

        app.Run();
    }

    private static void RegisterServices(IServiceCollection services, string contentRootPath) {
        var repoRoot = Path.GetFullPath(Path.Combine(contentRootPath, "..", ".."));
        var defaultSolutionPath = ResolvePath(
            Environment.GetEnvironmentVariable("CODE_ANALYTICS_DEFAULT_SOLUTION_PATH"),
            Path.Combine(repoRoot, "CanDoItAll.CodeAnalsis.slnx"),
            repoRoot);
        var outputRootPath = ResolvePath(
            Environment.GetEnvironmentVariable("CODE_ANALYTICS_OUTPUT_ROOT"),
            Path.Combine(repoRoot, "output"),
            repoRoot);

        services.AddSingleton(new CodeAnalyticsWebSettings(defaultSolutionPath, outputRootPath));
        services.AddSingleton(new CodeAnalyticsApplicationOptions(outputRootPath, "0.1.0"));
        services.AddSingleton<AnalysisRequestNormalizer>();
        services.AddSingleton<ProjectFileInventoryReader>();
        services.AddSingleton<MsBuildWorkspaceLoader>();
        services.AddSingleton<XmlDocumentationNormalizer>();
        services.AddSingleton<SymbolFactsCollector>();
        services.AddSingleton<DependencyFactCollector>();
        services.AddSingleton<ServiceRegistrationCollector>();
        services.AddSingleton<PersistenceFactCollector>();
        services.AddSingleton<StronglyConnectedComponentFinder>();
        services.AddSingleton<ArchitectureInsightBuilder>();
        services.AddSingleton<MarkdownSummaryWriter>();
        services.AddSingleton<ProjectGraphMermaidRenderer>();
        services.AddSingleton<ClassDiagramMermaidRenderer>();
        services.AddSingleton<ErDiagramMermaidRenderer>();
        services.AddSingleton<ExportBundleBuilder>();
        services.AddSingleton<SnapshotJsonSerializer>();
        services.AddSingleton<FileSnapshotRepository>();
        services.AddSingleton<ICodeAnalyticsApplicationService, CodeAnalyticsApplicationService>();
    }

    private static async Task<IResult> HandleAnalyzeAsync(
        HttpContext context,
        ICodeAnalyticsApplicationService applicationService,
        CodeAnalyticsWebSettings settings) {
        var form = await context.Request.ReadFormAsync();
        var solutionPath = GetValue(form, "solutionPath", settings.DefaultSolutionPath);
        var response = await applicationService.BuildSnapshotAsync(
            new BuildArchitectureSnapshotCommand(
                solutionPath,
                IncludeDi: IsChecked(form, "includeDi"),
                IncludePersistence: IsChecked(form, "includePersistence"),
                IncludeRisks: IsChecked(form, "includeRisks"),
                IncludeXmlDocs: IsChecked(form, "includeXmlDocs"),
                IncludeMermaidExports: IsChecked(form, "includeMermaidExports"),
                ForceRefresh: IsChecked(form, "forceRefresh")));

        return Results.Redirect($"/snapshots/{response.Snapshot.SnapshotId}");
    }

    private static IResult HandleExportAsync(string snapshotId, string? relativePath, CodeAnalyticsWebSettings settings) {
        if (string.IsNullOrWhiteSpace(relativePath)) {
            return Results.NotFound();
        }

        var snapshotRoot = Path.Combine(settings.OutputRootPath, "snapshots", snapshotId);
        var candidatePath = Path.GetFullPath(Path.Combine(snapshotRoot, relativePath.Replace('/', Path.DirectorySeparatorChar)));
        if (!candidatePath.StartsWith(snapshotRoot, StringComparison.OrdinalIgnoreCase) || !File.Exists(candidatePath)) {
            return Results.NotFound();
        }

        var contentType = Path.GetExtension(candidatePath).ToLowerInvariant() switch {
            ".md" => "text/markdown; charset=utf-8",
            ".json" => "application/json; charset=utf-8",
            _ => "text/plain; charset=utf-8",
        };

        return Results.File(candidatePath, contentType, enableRangeProcessing: false);
    }

    private static string GetValue(IFormCollection form, string key, string fallback) {
        return form.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value.ToString()
            : fallback;
    }

    private static bool IsChecked(IFormCollection form, string key) {
        return form.ContainsKey(key);
    }

    private static string ResolvePath(string? candidate, string fallback, string repoRoot) {
        if (string.IsNullOrWhiteSpace(candidate)) {
            return fallback;
        }

        return Path.IsPathRooted(candidate)
            ? candidate
            : Path.GetFullPath(Path.Combine(repoRoot, candidate));
    }
}
