using CanDoItAll.CodeAnalytics.Abstractions;
using CanDoItAll.CodeAnalytics.Abstractions.Commands;
using CanDoItAll.CodeAnalytics.Abstractions.Options;
using CanDoItAll.CodeAnalytics.Analysis.Graphs;
using CanDoItAll.CodeAnalytics.Analysis.Rules;
using CanDoItAll.CodeAnalytics.Application.Services;
using CanDoItAll.CodeAnalytics.Facts.Dependencies;
using CanDoItAll.CodeAnalytics.Facts.Documentation;
using CanDoItAll.CodeAnalytics.Facts.Members;
using CanDoItAll.CodeAnalytics.Facts.Persistence;
using CanDoItAll.CodeAnalytics.Facts.Services;
using CanDoItAll.CodeAnalytics.Facts.Symbols;
using CanDoItAll.CodeAnalytics.Rendering.Exports;
using CanDoItAll.CodeAnalytics.Rendering.Markdown;
using CanDoItAll.CodeAnalytics.Rendering.Mermaid;
using CanDoItAll.CodeAnalytics.Storage.Snapshots;
using CanDoItAll.CodeAnalytics.Web.Components;
using CanDoItAll.CodeAnalytics.Web.Operations;
using CanDoItAll.CodeAnalytics.Web.State;
using CanDoItAll.CodeAnalytics.Web.WorkspacePicker;
using CanDoItAll.CodeAnalytics.Workspace.Inventory;
using CanDoItAll.CodeAnalytics.Workspace.Loading;
using CanDoItAll.CodeAnalytics.Workspace.Normalization;
using CanDoItAll.Components.BaseLib;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CanDoItAll.CodeAnalytics.Web;

public class Program {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);
        var useHttpsRedirection = HasConfiguredHttpsEndpoint(builder.Configuration);
        builder.Logging.AddSimpleConsole(options => {
            options.TimestampFormat = "[HH:mm:ss] ";
            options.SingleLine = true;
        });

        builder.Services
            .AddRazorComponents()
            .AddInteractiveServerComponents();
        builder.Services.AddCanDoItAllBaseLib();
        RegisterServices(builder.Services, builder.Environment.ContentRootPath);

        var app = builder.Build();

        app.UseExceptionHandler("/Error");
        if (!app.Environment.IsDevelopment()) {
            app.UseHsts();
        }

        app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
        if (useHttpsRedirection) {
            app.UseHttpsRedirection();
        }

        app.UseAntiforgery();

        app.MapPost("/analyze", HandleAnalyzeAsync);
        app.MapPost("/api/workspace-picker", HandleWorkspacePickerAsync);
        app.MapGet("/exports/{snapshotId}/{**relativePath}", HandleExportAsync);
        app.MapGet("/favicon.ico", static () => Results.NoContent());
        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

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
        services.AddSingleton<MemberRelationshipCollector>();
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
        services.AddSingleton<AnalysisOperationCoordinator>();
        services.AddSingleton<IWorkspacePicker, WindowsWorkspacePicker>();
    }

    private static async Task<RedirectHttpResult> HandleAnalyzeAsync(
        HttpContext context,
        AnalysisOperationCoordinator operationCoordinator,
        CodeAnalyticsWebSettings settings,
        ILogger<Program> logger) {
        var form = await context.Request.ReadFormAsync();
        var command = CreateBuildCommand(form, settings);
        var operationId = operationCoordinator.Start(command);

        logger.LogInformation("Queued analysis operation {OperationId} for {WorkspacePath}", operationId, command.SolutionPath);
        return TypedResults.Redirect($"/operations/{operationId}");
    }

    private static async Task<IResult> HandleWorkspacePickerAsync(
        HttpContext context,
        IWorkspacePicker workspacePicker,
        ILogger<Program> logger,
        CancellationToken cancellationToken) {
        var request = await context.Request.ReadFromJsonAsync<WorkspacePickerRequest>(cancellationToken);
        var result = await workspacePicker.PickAsync(request?.CurrentPath, cancellationToken);
        if (!result.IsSuccess && !result.IsCanceled && !string.IsNullOrWhiteSpace(result.ErrorMessage)) {
            logger.LogWarning("Workspace picker returned an error: {Error}", result.ErrorMessage);
        }

        return Results.Json(result);
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

    private static BuildArchitectureSnapshotCommand CreateBuildCommand(IFormCollection form, CodeAnalyticsWebSettings settings) {
        return new BuildArchitectureSnapshotCommand(
            GetValue(form, "solutionPath", settings.DefaultSolutionPath),
            ParseDelimitedList(form, "scopeProjectNames"),
            null,
            IsChecked(form, "includeDi"),
            IsChecked(form, "includePersistence"),
            IsChecked(form, "includeRisks"),
            IsChecked(form, "includeXmlDocs"),
            IsChecked(form, "includeMermaidExports"),
            IsChecked(form, "forceRefresh"));
    }

    private static IReadOnlyList<string> ParseDelimitedList(IFormCollection form, string key) {
        if (!form.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value)) {
            return [];
        }

        return value.ToString()
            .Split([',', ';', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
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

    private static bool HasConfiguredHttpsEndpoint(IConfiguration configuration) {
        var urls = configuration["urls"]
            ?? configuration["ASPNETCORE_URLS"]
            ?? configuration["DOTNET_URLS"];
        if (string.IsNullOrWhiteSpace(urls)) {
            return false;
        }

        foreach (var candidate in urls.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)) {
            if (Uri.TryCreate(candidate, UriKind.Absolute, out var uri)
                && string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)) {
                return true;
            }
        }

        return false;
    }

    private sealed record WorkspacePickerRequest(string? CurrentPath);
}
