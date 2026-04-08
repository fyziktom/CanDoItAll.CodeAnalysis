using System.Diagnostics;
using System.Text;
using System.Text.Json;
using CanDoItAll.CodeAnalytics.Abstractions;
using CanDoItAll.CodeAnalytics.Abstractions.Commands;
using CanDoItAll.CodeAnalytics.Abstractions.Queries;
using CanDoItAll.CodeAnalytics.Abstractions.Responses;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Tests.Support;

var configuration = RunnerConfiguration.FromArgs(args);
Directory.CreateDirectory(configuration.BundleAnalysisDirectory);
Directory.CreateDirectory(configuration.SnapshotOutputDirectory);

var service = ApplicationServiceFactory.Create(configuration.SnapshotOutputDirectory);
var setupStopwatch = Stopwatch.StartNew();
var buildResponse = await service.BuildSnapshotAsync(
    new BuildArchitectureSnapshotCommand(
        configuration.SolutionPath,
        ForceRefresh: true));
setupStopwatch.Stop();

var setupReport = new SetupReport(
    buildResponse.Snapshot.SnapshotId,
    configuration.SolutionPath,
    setupStopwatch.ElapsedMilliseconds,
    1,
    buildResponse.Snapshot.Facts.Projects.Count,
    buildResponse.Snapshot.Facts.Types.Count,
    buildResponse.Snapshot.Facts.Members.Count);

switch (configuration.Mode) {
    case HarnessMode.SymbolTools:
        await RunSymbolToolsAsync(service, buildResponse.Snapshot.SnapshotId, configuration, setupReport);
        break;
    default:
        await RunFocusedContextAsync(service, buildResponse.Snapshot.SnapshotId, configuration, setupReport);
        break;
}

static async Task RunFocusedContextAsync(
    ICodeAnalyticsApplicationService service,
    string snapshotId,
    RunnerConfiguration configuration,
    SetupReport setupReport) {
    var scenarios = FocusedContextScenarioDefinition.CreateDefault();
    var scenarioReports = new List<FocusedContextScenarioReport>(scenarios.Length);

    foreach (var scenario in scenarios) {
        var scenarioStopwatch = Stopwatch.StartNew();
        var response = await service.GetFocusedContextAsync(
            new FocusedContextQuery(
                snapshotId,
                Depth: scenario.Depth,
                QueryText: scenario.QueryText,
                FocusTags: scenario.FocusTags,
                Intent: scenario.Intent,
                Precision: scenario.Precision));
        scenarioStopwatch.Stop();

        if (response is null) {
            throw new InvalidOperationException($"Focused-context returned null for scenario '{scenario.Key}'.");
        }

        var markdown = FocusedContextMarkdownRenderer.Render(scenario, response, scenarioStopwatch.Elapsed);
        var responsePath = Path.Combine(configuration.BundleAnalysisDirectory, $"focused-context-{scenario.Key}.md");
        var payloadPath = Path.Combine(configuration.BundleAnalysisDirectory, $"focused-context-{scenario.Key}.json");

        await File.WriteAllTextAsync(responsePath, markdown);
        await File.WriteAllTextAsync(payloadPath, JsonSerializer.Serialize(response, JsonOptions.Indented));

        scenarioReports.Add(
            new FocusedContextScenarioReport(
                scenario.Key,
                scenario.Name,
                scenario.QueryText,
                scenario.FocusTags,
                scenario.Depth,
                scenario.Intent,
                scenario.Precision,
                scenarioStopwatch.ElapsedMilliseconds,
                1,
                markdown.Length,
                EstimateTokens(markdown.Length),
                response.Stats.FileCount,
                response.Stats.BlockCount,
                response.Stats.SelectedLineCount,
                response.Stats.TotalLineCount,
                response.ResolvedIntent,
                response.ResolvedPrecision,
                response.SeedType?.DisplayName,
                response.SeedMember?.DisplayName,
                response.UsageSummary?.TotalCallerCount,
                response.UsageSummary?.TotalClusterCount,
                response.UsageSummary?.OmittedCallerCount,
                Path.GetFileName(responsePath),
                Path.GetFileName(payloadPath)));
    }

    var summary = new FocusedContextRunSummary(setupReport, scenarioReports);
    var summaryPath = Path.Combine(configuration.BundleAnalysisDirectory, "focused-context-summary.json");
    await File.WriteAllTextAsync(summaryPath, JsonSerializer.Serialize(summary, JsonOptions.Indented));

    Console.WriteLine(JsonSerializer.Serialize(summary, JsonOptions.Indented));
}

static async Task RunSymbolToolsAsync(
    ICodeAnalyticsApplicationService service,
    string snapshotId,
    RunnerConfiguration configuration,
    SetupReport setupReport) {
    var scenarios = SymbolToolsScenarioDefinition.CreateDefault();
    var scenarioReports = new List<SymbolToolsScenarioReport>(scenarios.Length);

    foreach (var scenario in scenarios) {
        var scenarioStopwatch = Stopwatch.StartNew();
        var searchResponse = await service.SearchSymbolsAsync(
            new SymbolSearchQuery(
                snapshotId,
                SearchText: scenario.QueryText,
                SearchMode: SymbolSearchMode.Exact));
        if (searchResponse is null) {
            throw new InvalidOperationException($"Symbol search returned null for scenario '{scenario.Key}'.");
        }

        var selectedResult = ResolveSelectedSymbolResult(searchResponse.Results, scenario);
        if (selectedResult is null) {
            throw new InvalidOperationException($"No symbol result was resolved for scenario '{scenario.Key}'.");
        }

        var definitionResponse = await service.GetSymbolDefinitionAsync(
            new SymbolDefinitionQuery(
                snapshotId,
                selectedResult.TypeId,
                selectedResult.MemberId));
        var membersResponse = await service.GetSymbolMembersAsync(new SymbolMembersQuery(snapshotId, selectedResult.TypeId));
        var implementationsResponse = await service.GetSymbolImplementationsAsync(new SymbolImplementationsQuery(snapshotId, selectedResult.TypeId));
        var referencesResponse = await service.GetSymbolReferencesAsync(
            new SymbolReferencesQuery(
                snapshotId,
                selectedResult.TypeId,
                selectedResult.MemberId));
        scenarioStopwatch.Stop();

        if (definitionResponse is null || membersResponse is null || implementationsResponse is null || referencesResponse is null) {
            throw new InvalidOperationException($"One or more symbol-tool responses were null for scenario '{scenario.Key}'.");
        }

        var payload = new SymbolToolsScenarioPayload(
            searchResponse,
            selectedResult,
            definitionResponse,
            membersResponse,
            implementationsResponse,
            referencesResponse);
        var markdown = SymbolToolsMarkdownRenderer.Render(scenario, payload, scenarioStopwatch.Elapsed);
        var responsePath = Path.Combine(configuration.BundleAnalysisDirectory, $"symbol-tools-{scenario.Key}.md");
        var payloadPath = Path.Combine(configuration.BundleAnalysisDirectory, $"symbol-tools-{scenario.Key}.json");

        await File.WriteAllTextAsync(responsePath, markdown);
        await File.WriteAllTextAsync(payloadPath, JsonSerializer.Serialize(payload, JsonOptions.Indented));

        scenarioReports.Add(
            new SymbolToolsScenarioReport(
                scenario.Key,
                scenario.Name,
                scenario.QueryText,
                scenarioStopwatch.ElapsedMilliseconds,
                5,
                markdown.Length,
                EstimateTokens(markdown.Length),
                searchResponse.Results.Count,
                selectedResult.DisplayName,
                definitionResponse.TargetKind,
                membersResponse.Members.Count,
                implementationsResponse.Implementations.Count,
                referencesResponse.TotalCount,
                referencesResponse.References.Count,
                Path.GetFileName(responsePath),
                Path.GetFileName(payloadPath)));
    }

    var summary = new SymbolToolsRunSummary(setupReport, scenarioReports);
    var summaryPath = Path.Combine(configuration.BundleAnalysisDirectory, "symbol-tools-summary.json");
    await File.WriteAllTextAsync(summaryPath, JsonSerializer.Serialize(summary, JsonOptions.Indented));

    Console.WriteLine(JsonSerializer.Serialize(summary, JsonOptions.Indented));
}

static SymbolSearchResultItem? ResolveSelectedSymbolResult(
    IReadOnlyList<SymbolSearchResultItem> results,
    SymbolToolsScenarioDefinition scenario) {
    return results
        .Where(item => item.TargetKind == SymbolTargetKind.Type)
        .FirstOrDefault(
            item => string.Equals(item.DisplayName, scenario.ExpectedDisplayName, StringComparison.Ordinal)
                || string.Equals(GetTrailingIdentifier(item.DisplayName), scenario.QueryText, StringComparison.Ordinal))
        ?? results.FirstOrDefault(item => item.TargetKind == SymbolTargetKind.Type)
        ?? results.FirstOrDefault();
}

static int EstimateTokens(int characterCount) {
    return (characterCount + 3) / 4;
}

static string GetTrailingIdentifier(string displayName) {
    var trimmed = displayName.Trim();
    var genericStart = trimmed.IndexOf('<');
    if (genericStart >= 0) {
        trimmed = trimmed[..genericStart];
    }

    var methodStart = trimmed.IndexOf('(');
    if (methodStart >= 0) {
        trimmed = trimmed[..methodStart];
    }

    var lastDot = trimmed.LastIndexOf('.');
    return lastDot >= 0
        ? trimmed[(lastDot + 1)..]
        : trimmed;
}

internal static class JsonOptions {
    public static readonly JsonSerializerOptions Indented = new() {
        WriteIndented = true,
    };
}

internal enum HarnessMode {
    FocusedContext,
    SymbolTools,
}

internal sealed record RunnerConfiguration(
    HarnessMode Mode,
    string SolutionPath,
    string BundleAnalysisDirectory,
    string SnapshotOutputDirectory) {
    public static RunnerConfiguration FromArgs(string[] args) {
        if (args.Length == 3) {
            return new RunnerConfiguration(
                HarnessMode.FocusedContext,
                Path.GetFullPath(args[0]),
                Path.GetFullPath(args[1]),
                Path.GetFullPath(args[2]));
        }

        if (args.Length == 4 && TryParseMode(args[0], out var mode)) {
            return new RunnerConfiguration(
                mode,
                Path.GetFullPath(args[1]),
                Path.GetFullPath(args[2]),
                Path.GetFullPath(args[3]));
        }

        throw new InvalidOperationException(
            "Expected arguments: <solution-path> <bundle-analysis-directory> <snapshot-output-directory> or <mode> <solution-path> <bundle-analysis-directory> <snapshot-output-directory>.");
    }

    private static bool TryParseMode(string value, out HarnessMode mode) {
        if (string.Equals(value, "focused-context", StringComparison.OrdinalIgnoreCase)) {
            mode = HarnessMode.FocusedContext;
            return true;
        }

        if (string.Equals(value, "symbol-tools", StringComparison.OrdinalIgnoreCase)) {
            mode = HarnessMode.SymbolTools;
            return true;
        }

        return Enum.TryParse(value, ignoreCase: true, out mode);
    }
}

internal sealed record FocusedContextScenarioDefinition(
    string Key,
    string Name,
    string QueryText,
    IReadOnlyList<string> FocusTags,
    int Depth,
    FocusedContextIntent Intent,
    FocusedContextPrecision Precision) {
    public static FocusedContextScenarioDefinition[] CreateDefault() {
        return [
            new FocusedContextScenarioDefinition(
                "app-db-context",
                "Database scenario",
                "AppDbContext",
                ["Db"],
                2,
                FocusedContextIntent.Auto,
                FocusedContextPrecision.Auto),
            new FocusedContextScenarioDefinition(
                "i-clock",
                "Common helper scenario",
                "IClock",
                [],
                2,
                FocusedContextIntent.Auto,
                FocusedContextPrecision.Auto),
            new FocusedContextScenarioDefinition(
                "canvas-scene-host",
                "UI scenario",
                "CanvasSceneHost",
                ["Ui"],
                2,
                FocusedContextIntent.Auto,
                FocusedContextPrecision.Auto),
        ];
    }
}

internal sealed record SymbolToolsScenarioDefinition(
    string Key,
    string Name,
    string QueryText,
    string ExpectedDisplayName) {
    public static SymbolToolsScenarioDefinition[] CreateDefault() {
        return [
            new SymbolToolsScenarioDefinition(
                "app-db-context",
                "Database scenario",
                "AppDbContext",
                "CanDoItAll.Infrastructure.Persistence.AppDbContext"),
            new SymbolToolsScenarioDefinition(
                "i-clock",
                "Common helper scenario",
                "IClock",
                "CanDoItAll.SharedKernel.IClock"),
            new SymbolToolsScenarioDefinition(
                "canvas-scene-host",
                "UI scenario",
                "CanvasSceneHost",
                "CanDoItAll.Components.CanvasLib.CanvasSceneHost"),
            new SymbolToolsScenarioDefinition(
                "storage-driver-registry",
                "Storage registry scenario",
                "IStorageDriverRegistry",
                "CanDoItAll.Infrastructure.Storage.IStorageDriverRegistry"),
            new SymbolToolsScenarioDefinition(
                "database-runtime-state",
                "Database runtime switching scenario",
                "IDatabaseRuntimeState",
                "CanDoItAll.Infrastructure.Persistence.IDatabaseRuntimeState"),
        ];
    }
}

internal sealed record SetupReport(
    string SnapshotId,
    string SolutionPath,
    long ElapsedMilliseconds,
    int CallCount,
    int ProjectCount,
    int TypeCount,
    int MemberCount);

internal sealed record FocusedContextScenarioReport(
    string Key,
    string Name,
    string QueryText,
    IReadOnlyList<string> FocusTags,
    int Depth,
    FocusedContextIntent RequestedIntent,
    FocusedContextPrecision RequestedPrecision,
    long ElapsedMilliseconds,
    int CallCount,
    int CharacterCount,
    int EstimatedTokenCount,
    int FileCount,
    int BlockCount,
    int SelectedLineCount,
    int TotalLineCount,
    FocusedContextIntent ResolvedIntent,
    FocusedContextPrecision ResolvedPrecision,
    string? SeedType,
    string? SeedMember,
    int? TotalCallerCount,
    int? TotalClusterCount,
    int? OmittedCallerCount,
    string MarkdownArtifact,
    string JsonArtifact);

internal sealed record FocusedContextRunSummary(
    SetupReport Setup,
    IReadOnlyList<FocusedContextScenarioReport> Scenarios);

internal sealed record SymbolToolsScenarioReport(
    string Key,
    string Name,
    string QueryText,
    long ElapsedMilliseconds,
    int CallCount,
    int CharacterCount,
    int EstimatedTokenCount,
    int SearchResultCount,
    string SelectedSymbol,
    SymbolTargetKind TargetKind,
    int MemberCount,
    int ImplementationCount,
    int TotalReferenceCount,
    int ReturnedReferenceCount,
    string MarkdownArtifact,
    string JsonArtifact);

internal sealed record SymbolToolsRunSummary(
    SetupReport Setup,
    IReadOnlyList<SymbolToolsScenarioReport> Scenarios);

internal sealed record SymbolToolsScenarioPayload(
    SymbolSearchResponse Search,
    SymbolSearchResultItem SelectedResult,
    SymbolDefinitionResponse Definition,
    SymbolMembersResponse Members,
    SymbolImplementationsResponse Implementations,
    SymbolReferencesResponse References);

internal static class FocusedContextMarkdownRenderer {
    public static string Render(
        FocusedContextScenarioDefinition scenario,
        FocusedContextResponse response,
        TimeSpan elapsed) {
        var builder = new StringBuilder();
        var typeNamesById = BuildTypeNames(response);
        builder.AppendLine($"# {scenario.Name}");
        builder.AppendLine();
        builder.AppendLine("## Query");
        builder.AppendLine();
        builder.AppendLine($"- Query text: `{scenario.QueryText}`");
        builder.AppendLine($"- Focus tags: {FormatList(scenario.FocusTags)}");
        builder.AppendLine($"- Depth: {scenario.Depth}");
        builder.AppendLine($"- Requested intent: `{scenario.Intent}`");
        builder.AppendLine($"- Requested precision: `{scenario.Precision}`");
        builder.AppendLine($"- Elapsed milliseconds: {elapsed.TotalMilliseconds:F0}");
        builder.AppendLine();
        builder.AppendLine("## Resolution");
        builder.AppendLine();
        builder.AppendLine($"- Seed type: {FormatNullable(response.SeedType?.DisplayName)}");
        builder.AppendLine($"- Seed member: {FormatNullable(response.SeedMember?.DisplayName)}");
        builder.AppendLine($"- Seed explanation: {FormatNullable(response.SeedExplanation)}");
        builder.AppendLine($"- Strategy explanation: {FormatNullable(response.StrategyExplanation)}");
        builder.AppendLine($"- Resolved intent: `{response.ResolvedIntent}`");
        builder.AppendLine($"- Resolved precision: `{response.ResolvedPrecision}`");
        builder.AppendLine();
        builder.AppendLine("## Stats");
        builder.AppendLine();
        builder.AppendLine($"- Files: {response.Stats.FileCount}");
        builder.AppendLine($"- Blocks: {response.Stats.BlockCount}");
        builder.AppendLine($"- Selected lines: {response.Stats.SelectedLineCount}");
        builder.AppendLine($"- Total lines in selected files: {response.Stats.TotalLineCount}");
        builder.AppendLine();
        builder.AppendLine("## Selection Reasons");
        builder.AppendLine();
        AppendSelectionReasons(builder, response);
        builder.AppendLine();
        builder.AppendLine("## Implementation Types");
        builder.AppendLine();
        AppendTypes(builder, response.ImplementationTypes);
        builder.AppendLine();
        builder.AppendLine("## Selected Types");
        builder.AppendLine();
        AppendTypes(builder, response.Types);
        builder.AppendLine();
        builder.AppendLine("## Selected Members");
        builder.AppendLine();
        AppendMembers(builder, response.Members, typeNamesById);
        builder.AppendLine();
        builder.AppendLine("## Usage Summary");
        builder.AppendLine();
        AppendUsageSummary(builder, response.UsageSummary);
        builder.AppendLine();
        builder.AppendLine("## File Excerpts");
        builder.AppendLine();
        AppendFiles(builder, response.Files);
        return builder.ToString();
    }

    private static void AppendSelectionReasons(StringBuilder builder, FocusedContextResponse response) {
        if (response.SelectionReasons.Count == 0) {
            builder.AppendLine("- None");
            return;
        }

        foreach (var reason in response.SelectionReasons
                     .OrderBy(item => item.TargetKind)
                     .ThenBy(item => item.TargetId, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(item => item.ReasonKind)
                     .ThenBy(item => item.RoleKind)) {
            builder.AppendLine($"- `{ResolveSelectionTargetLabel(reason, response)}`");
            builder.AppendLine($"  Target kind: `{reason.TargetKind}`");
            builder.AppendLine($"  Reason: `{reason.ReasonKind}`");
            builder.AppendLine($"  Role: `{reason.RoleKind}`");
        }
    }

    private static void AppendTypes(StringBuilder builder, IReadOnlyList<TypeFact> types) {
        if (types.Count == 0) {
            builder.AppendLine("- None");
            return;
        }

        foreach (var type in types) {
            builder.AppendLine($"- `{type.DisplayName}`");
            builder.AppendLine($"  Path: {type.Source.Path}");
            builder.AppendLine($"  Kind: `{type.Kind}`");
            builder.AppendLine($"  Project: `{type.ProjectId}`");
        }
    }

    private static void AppendMembers(
        StringBuilder builder,
        IReadOnlyList<MemberFact> members,
        IReadOnlyDictionary<string, string> typeNamesById) {
        if (members.Count == 0) {
            builder.AppendLine("- None");
            return;
        }

        foreach (var member in members) {
            builder.AppendLine($"- `{member.DisplayName}`");
            builder.AppendLine($"  Type: `{ResolveTypeName(member.TypeId, typeNamesById)}`");
            builder.AppendLine($"  Kind: `{member.Kind}`");
            builder.AppendLine($"  Path: {member.Source.Path}");
            builder.AppendLine($"  Line: {FormatNullable(member.Source.Line?.ToString())}");
        }
    }

    private static void AppendUsageSummary(StringBuilder builder, FocusedContextUsageSummary? usageSummary) {
        if (usageSummary is null) {
            builder.AppendLine("- None");
            return;
        }

        builder.AppendLine($"- Total callers: {usageSummary.TotalCallerCount}");
        builder.AppendLine($"- Total clusters: {usageSummary.TotalClusterCount}");
        builder.AppendLine($"- Omitted callers: {usageSummary.OmittedCallerCount}");

        foreach (var cluster in usageSummary.Clusters) {
            builder.AppendLine($"- Cluster: `{cluster.ProjectName}` / `{FormatNullable(cluster.ModuleName)}`");
            builder.AppendLine($"  Caller count: {cluster.CallerCount}");

            foreach (var sample in cluster.Samples) {
                builder.AppendLine($"  Sample: `{sample.TypeDisplayName}` -> `{sample.MemberDisplayName}`");
                builder.AppendLine($"  Path: {sample.Path}");
                builder.AppendLine($"  Line: {FormatNullable(sample.Line?.ToString())}");
                builder.AppendLine($"  Reason: {sample.Reason}");
            }
        }
    }

    private static void AppendFiles(StringBuilder builder, IReadOnlyList<FocusedContextFileExcerpt> files) {
        if (files.Count == 0) {
            builder.AppendLine("- None");
            return;
        }

        foreach (var file in files) {
            builder.AppendLine($"### {file.Path}");
            builder.AppendLine();
            builder.AppendLine($"- Total lines: {file.TotalLineCount}");
            builder.AppendLine($"- Selected lines: {file.SelectedLineCount}");
            builder.AppendLine($"- Types: {FormatList(file.TypeDisplayNames)}");
            builder.AppendLine();

            foreach (var block in file.Blocks) {
                builder.AppendLine($"#### {block.Title}");
                builder.AppendLine();
                builder.AppendLine($"- Kind: `{block.Kind}`");
                builder.AppendLine($"- Lines: {block.StartLine}-{block.EndLine}");
                builder.AppendLine();
                builder.AppendLine("```csharp");
                builder.AppendLine(block.Code);
                builder.AppendLine("```");
                builder.AppendLine();
            }
        }
    }

    private static string ResolveSelectionTargetLabel(FocusedContextSelectionReason reason, FocusedContextResponse response) {
        if (reason.TargetKind == FocusedContextSelectionTargetKind.File) {
            return reason.TargetId;
        }

        return response.Members.FirstOrDefault(item => string.Equals(item.MemberId, reason.TargetId, StringComparison.Ordinal))?.DisplayName
            ?? reason.TargetId;
    }

    private static string FormatList(IReadOnlyList<string> values) {
        return values.Count == 0
            ? "None"
            : string.Join(", ", values.Select(item => $"`{item}`"));
    }

    private static string FormatNullable(string? value) {
        return string.IsNullOrWhiteSpace(value)
            ? "None"
            : value;
    }

    private static IReadOnlyDictionary<string, string> BuildTypeNames(FocusedContextResponse response) {
        var map = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var type in response.Types) {
            map[type.TypeId] = type.DisplayName;
        }

        foreach (var type in response.ImplementationTypes) {
            map[type.TypeId] = type.DisplayName;
        }

        if (response.SeedType is not null) {
            map[response.SeedType.TypeId] = response.SeedType.DisplayName;
        }

        return map;
    }

    private static string ResolveTypeName(string typeId, IReadOnlyDictionary<string, string> typeNamesById) {
        return typeNamesById.TryGetValue(typeId, out var displayName)
            ? displayName
            : typeId;
    }
}

internal static class SymbolToolsMarkdownRenderer {
    public static string Render(
        SymbolToolsScenarioDefinition scenario,
        SymbolToolsScenarioPayload payload,
        TimeSpan elapsed) {
        var builder = new StringBuilder();
        builder.AppendLine($"# {scenario.Name}");
        builder.AppendLine();
        builder.AppendLine("## Query");
        builder.AppendLine();
        builder.AppendLine($"- Query text: `{scenario.QueryText}`");
        builder.AppendLine($"- Elapsed milliseconds: {elapsed.TotalMilliseconds:F0}");
        builder.AppendLine($"- Search results: {payload.Search.Results.Count}");
        builder.AppendLine($"- Selected symbol: `{payload.SelectedResult.DisplayName}`");
        builder.AppendLine($"- Target kind: `{payload.Definition.TargetKind}`");
        builder.AppendLine();
        builder.AppendLine("## Definition");
        builder.AppendLine();
        builder.AppendLine($"- Declaration: `{payload.Definition.Declaration}`");
        builder.AppendLine($"- Path: {payload.Definition.Definition.Path}:{payload.Definition.Definition.StartLine}");
        builder.AppendLine($"- Truncated: {payload.Definition.Definition.IsTruncated}");
        builder.AppendLine();
        builder.AppendLine("```csharp");
        builder.AppendLine(payload.Definition.Definition.Code);
        builder.AppendLine("```");
        builder.AppendLine();
        builder.AppendLine("## Members");
        builder.AppendLine();
        builder.AppendLine($"- Member count: {payload.Members.Members.Count}");
        foreach (var member in payload.Members.Members.Take(12)) {
            builder.AppendLine($"- `{member.DisplayName}` ({member.Kind})");
        }

        builder.AppendLine();
        builder.AppendLine("## Implementations");
        builder.AppendLine();
        builder.AppendLine($"- Count: {payload.Implementations.Implementations.Count}");
        foreach (var implementation in payload.Implementations.Implementations.Take(12)) {
            builder.AppendLine($"- `{implementation.Type.DisplayName}` ({implementation.Kind})");
        }

        builder.AppendLine();
        builder.AppendLine("## References");
        builder.AppendLine();
        builder.AppendLine($"- Total references: {payload.References.TotalCount}");
        builder.AppendLine($"- Returned references: {payload.References.References.Count}");
        foreach (var reference in payload.References.References.Take(12)) {
            builder.AppendLine($"- `{reference.SourceType.DisplayName}` :: `{reference.SourceMember?.DisplayName ?? "Type-level reference"}` ({reference.Kind})");
            builder.AppendLine($"  Path: {reference.Path}:{reference.Line}");
        }

        return builder.ToString();
    }
}
