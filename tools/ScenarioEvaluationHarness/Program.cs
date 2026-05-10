using System.Diagnostics;
using System.Text;
using System.Text.Json;
using CanDoItAll.CodeAnalytics.Abstractions;
using CanDoItAll.CodeAnalytics.Abstractions.Commands;
using CanDoItAll.CodeAnalytics.Abstractions.Queries;
using CanDoItAll.CodeAnalytics.Abstractions.Responses;
using CanDoItAll.CodeAnalytics.Tests.Support;

var configuration = EvaluationConfiguration.FromArgs(args);

if (configuration.Mode == EvaluationMode.Compare) {
    await CompareRunsAsync(configuration);
    return;
}

await RunScenariosAsync(configuration);

static async Task RunScenariosAsync(EvaluationConfiguration configuration) {
    Directory.CreateDirectory(configuration.OutputDirectory);
    Directory.CreateDirectory(configuration.SnapshotOutputDirectory);

    var service = ApplicationServiceFactory.Create(configuration.SnapshotOutputDirectory);
    var scenarios = EvaluationScenario.CreateDefault();
    var snapshots = new Dictionary<string, SnapshotSetupReport>(StringComparer.Ordinal);
    var scenarioReports = new List<EvaluationScenarioReport>(scenarios.Length);

    foreach (var snapshotRequest in scenarios.Select(item => item.Snapshot).DistinctBy(item => item.Key)) {
        var stopwatch = Stopwatch.StartNew();
        var response = await service.BuildSnapshotAsync(
            new BuildArchitectureSnapshotCommand(
                snapshotRequest.SolutionPath,
                ScopeProjectNames: snapshotRequest.ScopeProjectNames,
                ScopeNamespacePrefixes: snapshotRequest.ScopeNamespacePrefixes,
                IncludeMermaidExports: false,
                ForceRefresh: true));
        stopwatch.Stop();

        var setup = new SnapshotSetupReport(
            snapshotRequest.Key,
            response.Snapshot.SnapshotId,
            snapshotRequest.SolutionPath,
            snapshotRequest.ScopeProjectNames,
            snapshotRequest.ScopeNamespacePrefixes,
            stopwatch.ElapsedMilliseconds,
            response.Snapshot.Facts.Projects.Count,
            response.Snapshot.Facts.Types.Count,
            response.Snapshot.Facts.Members.Count);
        snapshots.Add(snapshotRequest.Key, setup);
    }

    foreach (var scenario in scenarios) {
        var snapshot = snapshots[scenario.Snapshot.Key];
        var stopwatch = Stopwatch.StartNew();
        var searchResponse = await service.SearchSymbolsAsync(
            new SymbolSearchQuery(
                snapshot.SnapshotId,
                SearchText: scenario.QueryText,
                SearchMode: SymbolSearchMode.Contains));
        var response = await service.GetFocusedContextAsync(
            new FocusedContextQuery(
                snapshot.SnapshotId,
                Depth: scenario.Depth,
                QueryText: scenario.QueryText,
                FocusTags: scenario.FocusTags,
                Intent: scenario.Intent,
                Precision: scenario.Precision,
                RelationHints: scenario.RelationHints));
        stopwatch.Stop();

        if (response is null || searchResponse is null) {
            var failedReport = EvaluationScenarioReport.CreateFailed(scenario, snapshot, stopwatch.ElapsedMilliseconds);
            scenarioReports.Add(failedReport);
            await WriteScenarioArtifactsAsync(configuration.OutputDirectory, scenario, failedReport, null, searchResponse);
            continue;
        }

        var score = ScenarioScorer.Score(scenario, response);
        var report = new EvaluationScenarioReport(
            scenario.Key,
            scenario.Category,
            scenario.RepositoryName,
            scenario.Prompt,
            scenario.AgentApproach,
            scenario.QueryText,
            scenario.FocusTags,
            scenario.RelationHints,
            scenario.Depth,
            scenario.Intent,
            scenario.Precision,
            snapshot.SnapshotId,
            stopwatch.ElapsedMilliseconds,
            CallCount: 2,
            searchResponse.Results.Count,
            response.SeedType?.DisplayName,
            response.SeedMember?.DisplayName,
            response.ResolvedIntent,
            response.ResolvedPrecision,
            response.Stats.FileCount,
            response.Stats.BlockCount,
            response.Stats.SelectedLineCount,
            response.Stats.TotalLineCount,
            response.UsageSummary?.TotalCallerCount,
            response.UsageSummary?.TotalClusterCount,
            response.UsageSummary?.OmittedCallerCount,
            TokenEstimator.Estimate(ScenarioScorer.GetResponseText(response).Length),
            ScenarioScorer.GetResponseText(response).Length,
            score,
            $"{scenario.Key}.md",
            $"{scenario.Key}.focused-context.json");
        scenarioReports.Add(report);
        await WriteScenarioArtifactsAsync(configuration.OutputDirectory, scenario, report, response, searchResponse);
    }

    var summary = new EvaluationRunSummary(
        configuration.RunLabel,
        DateTimeOffset.UtcNow,
        snapshots.Values.OrderBy(item => item.Key, StringComparer.Ordinal).ToArray(),
        scenarioReports);
    await File.WriteAllTextAsync(
        Path.Combine(configuration.OutputDirectory, "scenario-evaluation-summary.json"),
        JsonSerializer.Serialize(summary, JsonOptions.Indented));
    await File.WriteAllTextAsync(
        Path.Combine(configuration.OutputDirectory, "scenario-evaluation-summary.md"),
        MarkdownRenderer.RenderRunSummary(summary));

    Console.WriteLine(JsonSerializer.Serialize(summary.Aggregate, JsonOptions.Indented));
}

static async Task CompareRunsAsync(EvaluationConfiguration configuration) {
    Directory.CreateDirectory(configuration.OutputDirectory);

    var baseline = JsonSerializer.Deserialize<EvaluationRunSummary>(
        await File.ReadAllTextAsync(configuration.BaselineSummaryPath),
        JsonOptions.Indented)
        ?? throw new InvalidOperationException("Baseline summary could not be read.");
    var after = JsonSerializer.Deserialize<EvaluationRunSummary>(
        await File.ReadAllTextAsync(configuration.AfterSummaryPath),
        JsonOptions.Indented)
        ?? throw new InvalidOperationException("After summary could not be read.");

    var comparison = EvaluationComparison.Create(baseline, after);
    await File.WriteAllTextAsync(
        Path.Combine(configuration.OutputDirectory, "before-after-comparison.json"),
        JsonSerializer.Serialize(comparison, JsonOptions.Indented));
    await File.WriteAllTextAsync(
        Path.Combine(configuration.OutputDirectory, "before-after-comparison.md"),
        MarkdownRenderer.RenderComparison(comparison));

    Console.WriteLine(JsonSerializer.Serialize(comparison.Aggregate, JsonOptions.Indented));
}

static async Task WriteScenarioArtifactsAsync(
    string outputDirectory,
    EvaluationScenario scenario,
    EvaluationScenarioReport report,
    FocusedContextResponse? response,
    SymbolSearchResponse? searchResponse) {
    await File.WriteAllTextAsync(
        Path.Combine(outputDirectory, $"{scenario.Key}.md"),
        MarkdownRenderer.RenderScenario(scenario, report, response, searchResponse));
    await File.WriteAllTextAsync(
        Path.Combine(outputDirectory, $"{scenario.Key}.focused-context.json"),
        JsonSerializer.Serialize(response, JsonOptions.Indented));
    await File.WriteAllTextAsync(
        Path.Combine(outputDirectory, $"{scenario.Key}.symbol-search.json"),
        JsonSerializer.Serialize(searchResponse, JsonOptions.Indented));
}

internal enum EvaluationMode {
    Run,
    Compare,
}

internal sealed record EvaluationConfiguration(
    EvaluationMode Mode,
    string RunLabel,
    string OutputDirectory,
    string SnapshotOutputDirectory,
    string BaselineSummaryPath,
    string AfterSummaryPath) {
    public static EvaluationConfiguration FromArgs(string[] args) {
        if (args.Length == 3 && string.Equals(args[0], "run", StringComparison.OrdinalIgnoreCase)) {
            return new EvaluationConfiguration(
                EvaluationMode.Run,
                Path.GetFileName(Path.GetFullPath(args[1])),
                Path.GetFullPath(args[1]),
                Path.GetFullPath(args[2]),
                string.Empty,
                string.Empty);
        }

        if (args.Length == 4 && string.Equals(args[0], "compare", StringComparison.OrdinalIgnoreCase)) {
            return new EvaluationConfiguration(
                EvaluationMode.Compare,
                "comparison",
                Path.GetFullPath(args[3]),
                string.Empty,
                Path.GetFullPath(args[1]),
                Path.GetFullPath(args[2]));
        }

        throw new InvalidOperationException(
            "Expected arguments: run <output-directory> <snapshot-output-directory> or compare <baseline-summary> <after-summary> <output-directory>.");
    }
}

internal sealed record EvaluationSnapshotRequest(
    string Key,
    string SolutionPath,
    IReadOnlyList<string>? ScopeProjectNames = null,
    IReadOnlyList<string>? ScopeNamespacePrefixes = null);

internal sealed record EvaluationScenario(
    string Key,
    string Category,
    string RepositoryName,
    EvaluationSnapshotRequest Snapshot,
    string Prompt,
    string AgentApproach,
    string QueryText,
    IReadOnlyList<string> FocusTags,
    IReadOnlyList<string> RelationHints,
    int Depth,
    FocusedContextIntent Intent,
    FocusedContextPrecision Precision,
    IReadOnlyList<string> ExpectedTerms,
    IReadOnlyList<string> ExpectedFileFragments,
    IReadOnlyList<string> NoiseTerms,
    int TokenBudget) {
    public static EvaluationScenario[] CreateDefault() {
        var mbus = new EvaluationSnapshotRequest("mbus-full", @"C:\repositories\MBusParser\MbusParser.sln");
        var influx = new EvaluationSnapshotRequest("influx-full", @"C:\repositories\influxdb-client-csharp\influxdb-client-csharp.sln");
        var cando = new EvaluationSnapshotRequest("cando-full", @"C:\repositories\CanDoItAll\CanDoItAll.slnx");

        return [
            Intro("mbus-intro-parser", "MBusParser", mbus, "I need to add support for a new M-Bus frame variant. Before editing, show me the main parsing path and where telegram records are created.", "Build a snapshot, search for the public parser entry point, then ask focused context for a bounded protocol/parser overview.", "MBusParser", ["Protocol", "Parser"], ["MBusTelegram", "VariableDataRecord"], ["MBusParser", "MBusTelegram", "VariableDataRecord"], ["MbusParser", "MBusTelegram.cs", "VariableDataRecord.cs"], ["obj"], 2800),
            Intro("mbus-intro-decryption", "MBusParser", mbus, "I need to understand encryption/decryption support before fixing a meter that uses AES CTR.", "Search for the crypto provider, then ask focused context with crypto tags and AES-related relation hints.", "MbusCryptoProvider", ["Crypto"], ["AesCtrCrypto", "AesCbcCrypto", "DesCbcCrypto"], ["MbusCryptoProvider", "IMbusCrypto", "AesCtrCrypto"], ["Decryption"], ["obj"], 2200),
            Intro("mbus-intro-record-model", "MBusParser", mbus, "I need orientation on how data records, DIF, VIF, unit, and value description fit together.", "Start at the variable record model, then use relation hints for the header field types.", "VariableDataRecord", ["Protocol", "Model"], ["DataInformationField", "ValueInformationField", "ValueDescription"], ["VariableDataRecord", "DataInformationField", "ValueInformationField", "ValueDescription"], ["DataRecord"], ["obj"], 2800),
            Intro("influx-intro-write-flow", "influxdb-client-csharp", influx, "I need to add safer write batching behavior. First show me the client write flow from InfluxDBClient to write APIs and options.", "Search the client facade, then ask focused context with write tags and WriteApi relation hints.", "InfluxDBClient", ["Client", "Write"], ["WriteApi", "WriteApiAsync", "WriteOptions"], ["InfluxDBClient", "WriteApi", "WriteOptions"], ["Client", "WriteApi.cs", "WriteOptions.cs"], ["Domain"], 3500),
            Intro("influx-intro-query-flow", "influxdb-client-csharp", influx, "I need to understand Flux query execution and result parsing before changing query behavior.", "Start with QueryApi, then request query/parser context around Flux table and parser types.", "QueryApi", ["Query"], ["FluxCsvParser", "FluxTable", "FluxRecord"], ["QueryApi", "FluxCsvParser", "FluxTable", "FluxRecord"], ["QueryApi.cs", "Flux"], ["Domain"], 3500),
            Intro("cando-intro-canvas", "CanDoItAll", cando, "I need to change CanvasLib interaction behavior. Give me a first-pass map of the scene host and nearby canvas services.", "Use focused context on CanvasSceneHost with UI relation hints instead of opening the whole component library.", "CanvasSceneHost", ["Ui", "Razor"], ["CommandHistoryStore", "InvalidationScheduler", "LayerStack"], ["CanvasSceneHost", "CommandHistoryStore", "InvalidationScheduler"], ["CanvasLib", "CanvasSceneHost.cs"], ["Migrations"], 3200),

            Specific("mbus-fix-bcd-date", "MBusParser", mbus, "Fix a date decoding bug near MbusParser/Drivers/BcdDateTimeParser.cs. I need its registry usage and related parser examples.", "Search the named parser and ask for direct parser/registry context.", "BcdDateTimeParser", ["Parser"], ["DateTimeParserRegistry", "IDateTimeParser"], ["BcdDateTimeParser", "DateTimeParserRegistry", "IDateTimeParser"], ["Drivers"], ["obj"], 1800),
            Specific("mbus-enum-utils-dif", "MBusParser", mbus, "EnumUtils seems to map raw bytes to enum values. Show me how it is used by DIF/VIF fields before I tighten parsing.", "Start from EnumUtils and narrow by data information field relation hints.", "EnumUtils", ["Protocol"], ["DataInformationField", "PrimaryValueInformationField"], ["EnumUtils", "DataInformationField", "PrimaryValueInformationField"], ["Helpers", "DataInformationBlock", "ValueInformationBlock"], ["obj"], 1800),
            Specific("mbus-aes-ctr", "MBusParser", mbus, "A telegram decrypted with AES CTR is wrong. Show me AesCtrCrypto and how the provider chooses it.", "Search the AES CTR crypto type and ask for crypto context related to provider selection.", "AesCtrCrypto", ["Crypto"], ["MbusCryptoProvider", "IMbusCrypto"], ["AesCtrCrypto", "MbusCryptoProvider", "IMbusCrypto"], ["Decryption"], ["obj"], 1800),
            Specific("mbus-control-info", "MBusParser", mbus, "Control information code lookup looks suspicious. Show ControlInformationLookup with the enum/type it maps to.", "Use exact-ish focused context on the lookup helper with relation hints for the enum.", "ControlInformationLookup", ["Protocol"], ["ControlInformation"], ["ControlInformationLookup", "ControlInformation"], ["Header"], ["obj"], 1600),
            Specific("mbus-vif-extension", "MBusParser", mbus, "I need to add a new VIF extension value. Show me ValueInformationExtensionField and existing FB/FD extension handling.", "Search the extension field and ask for related extension classes.", "ValueInformationExtensionField", ["Protocol"], ["FBValueInformationExtensionField", "FDValueInformationExtensionField"], ["ValueInformationExtensionField", "FBValueInformationExtensionField", "FDValueInformationExtensionField"], ["ValueInformationBlock", "Extension"], ["obj"], 2200),

            Specific("influx-write-retry", "influxdb-client-csharp", influx, "WriteApi retries are behaving oddly. Show retry scheduling and write options around Client/WriteApi.cs.", "Start at WriteApi and ask for write/retry context with RetryAttempt and WriteOptions as relation hints.", "WriteApi", ["Write"], ["RetryAttempt", "WriteOptions"], ["WriteApi", "RetryAttempt", "WriteOptions"], ["WriteApi.cs", "RetryAttempt.cs", "WriteOptions.cs"], ["Domain"], 2800),
            Specific("influx-write-async", "influxdb-client-csharp", influx, "I need to change async write error handling in WriteApiAsync without touching sync write behavior by accident.", "Ask for WriteApiAsync with relation hints to sync WriteApi and retry behavior.", "WriteApiAsync", ["Write"], ["WriteApi", "RetryAttempt"], ["WriteApiAsync", "IWriteApiAsync", "RetryAttempt"], ["WriteApiAsync.cs", "WriteApi.cs"], ["Domain"], 2600),
            Specific("influx-point-escaping", "influxdb-client-csharp", influx, "PointData line protocol escaping is wrong for tags. Show PointData and its builder before patching tests.", "Start at PointData, relation-hint the builder partial and tests.", "PointData", ["Write", "Model"], ["PointData.Builder", "PointDataTest"], ["PointData", "PointData.Builder", "PointDataTest"], ["PointData.cs", "PointData.Builder.cs"], ["Domain"], 2400),
            Specific("influx-query-cancel", "influxdb-client-csharp", influx, "QueryApi cancellation handling needs review. Show QueryApi with parsing and test context.", "Use QueryApi with query/parser/test relation hints.", "QueryApi", ["Query"], ["FluxCsvParser", "QueryApiTest"], ["QueryApi", "FluxCsvParser", "QueryApiTest"], ["QueryApi.cs", "FluxCsvParser.cs", "QueryApiTest.cs"], ["Domain"], 3000),
            Specific("influx-delete-predicate", "influxdb-client-csharp", influx, "DeleteApi predicate formatting is failing in a specific test. Show DeleteApi and its test surface.", "Search DeleteApi and request direct test-related context.", "DeleteApi", ["Client", "Test"], ["DeleteApiTest", "DeletePredicateRequest"], ["DeleteApi", "DeletePredicateRequest"], ["DeleteApi.cs"], ["Domain"], 2200),
            Specific("influx-client-options", "influxdb-client-csharp", influx, "A new option should flow through InfluxDBClientFactory into InfluxDBClientOptions. Show that construction path.", "Start with the factory and relation-hint options and client facade.", "InfluxDBClientFactory", ["Client"], ["InfluxDBClientOptions", "InfluxDBClient"], ["InfluxDBClientFactory", "InfluxDBClientOptions", "InfluxDBClient"], ["InfluxDBClientFactory.cs", "InfluxDBClientOptions.cs"], ["Domain"], 2200),
            Specific("influx-linq-provider", "influxdb-client-csharp", influx, "LINQ query generation needs a fix. Show InfluxDBQueryable and the provider/expression path around it.", "Search the LINQ queryable and use relation hints for provider and expression terms.", "InfluxDBQueryable", ["Linq", "Query"], ["Expression", "Provider"], ["InfluxDBQueryable", "Expression", "Provider"], ["Client.Linq", "InfluxDBQueryable.cs"], ["Domain"], 2600),

            Specific("cando-db-save", "CanDoItAll", cando, "SaveChanges coordination in AppDbContext is risky. Show the save path and runtime state collaborators.", "Use DB tags on AppDbContext with relation hints for runtime state and coordination.", "AppDbContext", ["EntityFramework"], ["DatabaseRuntimeState", "SaveChanges"], ["AppDbContext", "DatabaseRuntimeState", "SaveChanges"], ["Persistence", "AppDbContext.cs"], ["CanvasLib"], 3200),
            Specific("cando-clock-workbench", "CanDoItAll", cando, "IClock is used everywhere, but I only care about Workbench behavior. Show the focused helper usage around Workbench.", "Use helper seed IClock with a concrete Workbench relation hint to avoid broad helper sampling.", "IClock", [], ["Workbench"], ["IClock", "Workbench"], ["SharedKernel"], ["Migrations"], 1600),
            Specific("cando-storage-registry", "CanDoItAll", cando, "Storage driver registry behavior is unclear. Show registry definition and the catalog/service consumer path.", "Search storage registry and use relation hints around storage catalog service.", "IStorageDriverRegistry", ["Infra", "Service"], ["StorageCatalogService"], ["IStorageDriverRegistry", "StorageCatalogService"], ["Storage"], ["CanvasLib"], 2400),
            Specific("cando-canvas-mark-applied", "CanDoItAll", cando, "A canvas dirty-state bug points at CanvasSceneHost.MarkApplied. Show that member and directly related invalidation state.", "Ask focused context for the named host with relation hints around MarkApplied and invalidation.", "MarkApplied", ["Ui"], ["CanvasSceneHost", "InvalidationScheduler"], ["MarkApplied", "CanvasSceneHost", "InvalidationScheduler"], ["CanvasLib", "CanvasSceneHost.cs"], ["Migrations"], 2200),
        ];
    }

    private static EvaluationScenario Intro(
        string key,
        string repositoryName,
        EvaluationSnapshotRequest snapshot,
        string prompt,
        string agentApproach,
        string queryText,
        IReadOnlyList<string> focusTags,
        IReadOnlyList<string> relationHints,
        IReadOnlyList<string> expectedTerms,
        IReadOnlyList<string> expectedFileFragments,
        IReadOnlyList<string> noiseTerms,
        int tokenBudget) {
        return new EvaluationScenario(
            key,
            "Introduction",
            repositoryName,
            snapshot,
            prompt,
            agentApproach,
            queryText,
            focusTags,
            relationHints,
            2,
            FocusedContextIntent.TroublePath,
            FocusedContextPrecision.Outline,
            expectedTerms,
            expectedFileFragments,
            noiseTerms,
            tokenBudget);
    }

    private static EvaluationScenario Specific(
        string key,
        string repositoryName,
        EvaluationSnapshotRequest snapshot,
        string prompt,
        string agentApproach,
        string queryText,
        IReadOnlyList<string> focusTags,
        IReadOnlyList<string> relationHints,
        IReadOnlyList<string> expectedTerms,
        IReadOnlyList<string> expectedFileFragments,
        IReadOnlyList<string> noiseTerms,
        int tokenBudget) {
        return new EvaluationScenario(
            key,
            "Specific",
            repositoryName,
            snapshot,
            prompt,
            agentApproach,
            queryText,
            focusTags,
            relationHints,
            2,
            FocusedContextIntent.Auto,
            FocusedContextPrecision.Auto,
            expectedTerms,
            expectedFileFragments,
            noiseTerms,
            tokenBudget);
    }
}

internal sealed record SnapshotSetupReport(
    string Key,
    string SnapshotId,
    string SolutionPath,
    IReadOnlyList<string>? ScopeProjectNames,
    IReadOnlyList<string>? ScopeNamespacePrefixes,
    long ElapsedMilliseconds,
    int ProjectCount,
    int TypeCount,
    int MemberCount);

internal sealed record EvaluationScenarioReport(
    string Key,
    string Category,
    string RepositoryName,
    string Prompt,
    string AgentApproach,
    string QueryText,
    IReadOnlyList<string> FocusTags,
    IReadOnlyList<string> RelationHints,
    int Depth,
    FocusedContextIntent RequestedIntent,
    FocusedContextPrecision RequestedPrecision,
    string SnapshotId,
    long ElapsedMilliseconds,
    int CallCount,
    int SearchResultCount,
    string? SeedType,
    string? SeedMember,
    FocusedContextIntent ResolvedIntent,
    FocusedContextPrecision ResolvedPrecision,
    int FileCount,
    int BlockCount,
    int SelectedLineCount,
    int TotalLineCount,
    int? TotalCallerCount,
    int? TotalClusterCount,
    int? OmittedCallerCount,
    int EstimatedTokenCount,
    int CharacterCount,
    ScenarioScore Score,
    string MarkdownArtifact,
    string JsonArtifact) {
    public static EvaluationScenarioReport CreateFailed(
        EvaluationScenario scenario,
        SnapshotSetupReport snapshot,
        long elapsedMilliseconds) {
        return new EvaluationScenarioReport(
            scenario.Key,
            scenario.Category,
            scenario.RepositoryName,
            scenario.Prompt,
            scenario.AgentApproach,
            scenario.QueryText,
            scenario.FocusTags,
            scenario.RelationHints,
            scenario.Depth,
            scenario.Intent,
            scenario.Precision,
            snapshot.SnapshotId,
            elapsedMilliseconds,
            2,
            0,
            null,
            null,
            FocusedContextIntent.Auto,
            FocusedContextPrecision.Auto,
            0,
            0,
            0,
            0,
            null,
            null,
            null,
            0,
            0,
            ScenarioScore.Failed,
            $"{scenario.Key}.md",
            $"{scenario.Key}.focused-context.json");
    }
}

internal sealed record ScenarioScore(
    double ExpectedTermCoverage,
    int ExpectedTermHits,
    int ExpectedTermCount,
    double ExpectedFileCoverage,
    int ExpectedFileHits,
    int ExpectedFileCount,
    int UsefulFileCount,
    int NonUsefulFileCount,
    double NonUsefulFileRatio,
    int NoiseTermHits,
    double TokenBudgetRatio,
    int MissingExpectedContextCount,
    double HelpfulnessScore,
    string Rating) {
    public static ScenarioScore Failed { get; } = new(0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, "Failed");
}

internal sealed record EvaluationRunSummary(
    string RunLabel,
    DateTimeOffset RunUtc,
    IReadOnlyList<SnapshotSetupReport> Snapshots,
    IReadOnlyList<EvaluationScenarioReport> Scenarios) {
    public EvaluationAggregate Aggregate => EvaluationAggregate.From(Scenarios);
}

internal sealed record EvaluationAggregate(
    int ScenarioCount,
    int IntroductionScenarioCount,
    double AverageHelpfulnessScore,
    double AverageExpectedTermCoverage,
    double AverageExpectedFileCoverage,
    double AverageNonUsefulFileRatio,
    double AverageTokenBudgetRatio,
    int GoodCount,
    int MixedCount,
    int PoorCount,
    int FailedCount,
    int TotalEstimatedTokens,
    int TotalSelectedLines,
    int TotalNonUsefulFiles,
    int TotalUsefulFiles) {
    public static EvaluationAggregate From(IReadOnlyList<EvaluationScenarioReport> scenarios) {
        return new EvaluationAggregate(
            scenarios.Count,
            scenarios.Count(item => string.Equals(item.Category, "Introduction", StringComparison.Ordinal)),
            Average(scenarios, item => item.Score.HelpfulnessScore),
            Average(scenarios, item => item.Score.ExpectedTermCoverage),
            Average(scenarios, item => item.Score.ExpectedFileCoverage),
            Average(scenarios, item => item.Score.NonUsefulFileRatio),
            Average(scenarios, item => item.Score.TokenBudgetRatio),
            scenarios.Count(item => string.Equals(item.Score.Rating, "Good", StringComparison.Ordinal)),
            scenarios.Count(item => string.Equals(item.Score.Rating, "Mixed", StringComparison.Ordinal)),
            scenarios.Count(item => string.Equals(item.Score.Rating, "Poor", StringComparison.Ordinal)),
            scenarios.Count(item => string.Equals(item.Score.Rating, "Failed", StringComparison.Ordinal)),
            scenarios.Sum(item => item.EstimatedTokenCount),
            scenarios.Sum(item => item.SelectedLineCount),
            scenarios.Sum(item => item.Score.NonUsefulFileCount),
            scenarios.Sum(item => item.Score.UsefulFileCount));
    }

    private static double Average(IReadOnlyList<EvaluationScenarioReport> scenarios, Func<EvaluationScenarioReport, double> selector) {
        return scenarios.Count == 0
            ? 0
            : Math.Round(scenarios.Average(selector), 3);
    }
}

internal sealed record ScenarioComparison(
    string Key,
    string Category,
    string RepositoryName,
    double BaselineHelpfulness,
    double AfterHelpfulness,
    double HelpfulnessDelta,
    int BaselineEstimatedTokens,
    int AfterEstimatedTokens,
    int EstimatedTokenDelta,
    int BaselineSelectedLines,
    int AfterSelectedLines,
    int SelectedLineDelta,
    int BaselineNonUsefulFiles,
    int AfterNonUsefulFiles,
    int NonUsefulFileDelta,
    string BaselineRating,
    string AfterRating);

internal sealed record EvaluationComparison(
    EvaluationAggregate BaselineAggregate,
    EvaluationAggregate AfterAggregate,
    EvaluationComparisonAggregate Aggregate,
    IReadOnlyList<ScenarioComparison> Scenarios) {
    public static EvaluationComparison Create(EvaluationRunSummary baseline, EvaluationRunSummary after) {
        var afterByKey = after.Scenarios.ToDictionary(item => item.Key, StringComparer.Ordinal);
        var comparisons = baseline.Scenarios
            .Where(item => afterByKey.ContainsKey(item.Key))
            .Select(
                item => {
                    var afterItem = afterByKey[item.Key];
                    return new ScenarioComparison(
                        item.Key,
                        item.Category,
                        item.RepositoryName,
                        item.Score.HelpfulnessScore,
                        afterItem.Score.HelpfulnessScore,
                        Math.Round(afterItem.Score.HelpfulnessScore - item.Score.HelpfulnessScore, 3),
                        item.EstimatedTokenCount,
                        afterItem.EstimatedTokenCount,
                        afterItem.EstimatedTokenCount - item.EstimatedTokenCount,
                        item.SelectedLineCount,
                        afterItem.SelectedLineCount,
                        afterItem.SelectedLineCount - item.SelectedLineCount,
                        item.Score.NonUsefulFileCount,
                        afterItem.Score.NonUsefulFileCount,
                        afterItem.Score.NonUsefulFileCount - item.Score.NonUsefulFileCount,
                        item.Score.Rating,
                        afterItem.Score.Rating);
                })
            .ToArray();

        return new EvaluationComparison(
            baseline.Aggregate,
            after.Aggregate,
            EvaluationComparisonAggregate.From(comparisons),
            comparisons);
    }
}

internal sealed record EvaluationComparisonAggregate(
    int ScenarioCount,
    int ImprovedCount,
    int RegressedCount,
    int UnchangedCount,
    double AverageHelpfulnessDelta,
    int EstimatedTokenDelta,
    int SelectedLineDelta,
    int NonUsefulFileDelta) {
    public static EvaluationComparisonAggregate From(IReadOnlyList<ScenarioComparison> comparisons) {
        return new EvaluationComparisonAggregate(
            comparisons.Count,
            comparisons.Count(item => item.HelpfulnessDelta > 0.02),
            comparisons.Count(item => item.HelpfulnessDelta < -0.02),
            comparisons.Count(item => item.HelpfulnessDelta is >= -0.02 and <= 0.02),
            comparisons.Count == 0 ? 0 : Math.Round(comparisons.Average(item => item.HelpfulnessDelta), 3),
            comparisons.Sum(item => item.EstimatedTokenDelta),
            comparisons.Sum(item => item.SelectedLineDelta),
            comparisons.Sum(item => item.NonUsefulFileDelta));
    }
}

internal static class ScenarioScorer {
    public static ScenarioScore Score(EvaluationScenario scenario, FocusedContextResponse response) {
        var text = GetResponseText(response);
        var normalizedText = text.ToLowerInvariant();
        var expectedTermHits = CountHits(scenario.ExpectedTerms, normalizedText);
        var expectedFileHits = CountHits(scenario.ExpectedFileFragments, normalizedText);
        var noiseTermHits = CountHits(scenario.NoiseTerms, normalizedText);
        var usefulFileCount = response.Files.Count(file => IsUsefulFile(scenario, file));
        var nonUsefulFileCount = Math.Max(0, response.Files.Count - usefulFileCount);
        var tokenCount = TokenEstimator.Estimate(text.Length);
        var expectedTermCoverage = Coverage(expectedTermHits, scenario.ExpectedTerms.Count);
        var expectedFileCoverage = Coverage(expectedFileHits, scenario.ExpectedFileFragments.Count);
        var nonUsefulFileRatio = response.Files.Count == 0
            ? 1
            : (double)nonUsefulFileCount / response.Files.Count;
        var tokenBudgetRatio = scenario.TokenBudget <= 0
            ? 0
            : Math.Min(2, (double)tokenCount / scenario.TokenBudget);
        var boundedness = Math.Max(0, 1 - Math.Max(0, tokenBudgetRatio - 1));
        var noisePenalty = Math.Min(0.3, noiseTermHits * 0.05);
        var helpfulness = Math.Clamp(
            expectedTermCoverage * 0.38
            + expectedFileCoverage * 0.32
            + (1 - nonUsefulFileRatio) * 0.2
            + boundedness * 0.1
            - noisePenalty,
            0,
            1);
        var rating = helpfulness switch {
            >= 0.72 => "Good",
            >= 0.45 => "Mixed",
            _ => "Poor",
        };

        return new ScenarioScore(
            Math.Round(expectedTermCoverage, 3),
            expectedTermHits,
            scenario.ExpectedTerms.Count,
            Math.Round(expectedFileCoverage, 3),
            expectedFileHits,
            scenario.ExpectedFileFragments.Count,
            usefulFileCount,
            nonUsefulFileCount,
            Math.Round(nonUsefulFileRatio, 3),
            noiseTermHits,
            Math.Round(tokenBudgetRatio, 3),
            Math.Max(0, scenario.ExpectedTerms.Count - expectedTermHits),
            Math.Round(helpfulness, 3),
            rating);
    }

    public static string GetResponseText(FocusedContextResponse response) {
        var builder = new StringBuilder();
        Append(builder, response.SeedType?.DisplayName);
        Append(builder, response.SeedMember?.DisplayName);
        Append(builder, response.SeedExplanation);
        Append(builder, response.StrategyExplanation);

        foreach (var type in response.Types.Concat(response.ImplementationTypes)) {
            Append(builder, type.DisplayName);
            Append(builder, type.Source.Path);
        }

        foreach (var member in response.Members) {
            Append(builder, member.DisplayName);
            Append(builder, member.Source.Path);
        }

        if (response.UsageSummary is not null) {
            foreach (var cluster in response.UsageSummary.Clusters) {
                Append(builder, cluster.ProjectName);
                Append(builder, cluster.ModuleName);

                foreach (var sample in cluster.Samples) {
                    Append(builder, sample.TypeDisplayName);
                    Append(builder, sample.MemberDisplayName);
                    Append(builder, sample.Path);
                    Append(builder, sample.Reason);
                }
            }
        }

        foreach (var file in response.Files) {
            Append(builder, file.Path);
            foreach (var typeName in file.TypeDisplayNames) {
                Append(builder, typeName);
            }

            foreach (var block in file.Blocks) {
                Append(builder, block.Title);
                Append(builder, block.Code);
            }
        }

        return builder.ToString();
    }

    private static bool IsUsefulFile(EvaluationScenario scenario, FocusedContextFileExcerpt file) {
        var path = file.Path.ToLowerInvariant();
        if (scenario.ExpectedFileFragments.Any(fragment => path.Contains(fragment.ToLowerInvariant(), StringComparison.Ordinal))) {
            return true;
        }

        var joinedTypes = string.Join(' ', file.TypeDisplayNames).ToLowerInvariant();
        return scenario.ExpectedTerms.Any(term => joinedTypes.Contains(term.ToLowerInvariant(), StringComparison.Ordinal));
    }

    private static int CountHits(IReadOnlyList<string> expectedValues, string normalizedText) {
        return expectedValues
            .Select(value => value.ToLowerInvariant())
            .Distinct(StringComparer.Ordinal)
            .Count(value => normalizedText.Contains(value, StringComparison.Ordinal));
    }

    private static double Coverage(int hits, int total) {
        return total == 0
            ? 1
            : (double)hits / total;
    }

    private static void Append(StringBuilder builder, string? value) {
        if (!string.IsNullOrWhiteSpace(value)) {
            builder.Append(value);
            builder.Append('\n');
        }
    }
}

internal static class TokenEstimator {
    public static int Estimate(int characterCount) {
        return (characterCount + 3) / 4;
    }
}

internal static class MarkdownRenderer {
    public static string RenderRunSummary(EvaluationRunSummary summary) {
        var builder = new StringBuilder();
        builder.AppendLine($"# Scenario evaluation {summary.RunLabel}");
        builder.AppendLine();
        AppendAggregate(builder, summary.Aggregate);
        builder.AppendLine();
        builder.AppendLine("## Snapshots");
        builder.AppendLine();
        foreach (var snapshot in summary.Snapshots) {
            builder.AppendLine($"- `{snapshot.Key}`: {snapshot.ProjectCount} projects, {snapshot.TypeCount} types, {snapshot.MemberCount} members, {snapshot.ElapsedMilliseconds} ms");
        }

        builder.AppendLine();
        builder.AppendLine("## Scenarios");
        builder.AppendLine();
        builder.AppendLine("| Scenario | Repo | Category | Rating | Score | Terms | Files | Tokens | Non-useful files |");
        builder.AppendLine("| --- | --- | --- | --- | ---: | ---: | ---: | ---: | ---: |");
        foreach (var scenario in summary.Scenarios.OrderBy(item => item.Key, StringComparer.Ordinal)) {
            builder.AppendLine($"| `{scenario.Key}` | {scenario.RepositoryName} | {scenario.Category} | {scenario.Score.Rating} | {scenario.Score.HelpfulnessScore:F3} | {scenario.Score.ExpectedTermHits}/{scenario.Score.ExpectedTermCount} | {scenario.FileCount} | {scenario.EstimatedTokenCount} | {scenario.Score.NonUsefulFileCount} |");
        }

        return builder.ToString();
    }

    public static string RenderScenario(
        EvaluationScenario scenario,
        EvaluationScenarioReport report,
        FocusedContextResponse? response,
        SymbolSearchResponse? searchResponse) {
        var builder = new StringBuilder();
        builder.AppendLine($"# {scenario.Key}");
        builder.AppendLine();
        builder.AppendLine("## Simulated Prompt");
        builder.AppendLine();
        builder.AppendLine(scenario.Prompt);
        builder.AppendLine();
        builder.AppendLine("## Simulated Agent Approach");
        builder.AppendLine();
        builder.AppendLine(scenario.AgentApproach);
        builder.AppendLine();
        builder.AppendLine("## Query");
        builder.AppendLine();
        builder.AppendLine($"- Repository: `{scenario.RepositoryName}`");
        builder.AppendLine($"- Category: `{scenario.Category}`");
        builder.AppendLine($"- Query text: `{scenario.QueryText}`");
        builder.AppendLine($"- Focus tags: {FormatList(scenario.FocusTags)}");
        builder.AppendLine($"- Relation hints: {FormatList(scenario.RelationHints)}");
        builder.AppendLine($"- Depth: {scenario.Depth}");
        builder.AppendLine($"- Intent: `{scenario.Intent}`");
        builder.AppendLine($"- Precision: `{scenario.Precision}`");
        builder.AppendLine();
        builder.AppendLine("## Score");
        builder.AppendLine();
        builder.AppendLine($"- Rating: `{report.Score.Rating}`");
        builder.AppendLine($"- Helpfulness score: {report.Score.HelpfulnessScore:F3}");
        builder.AppendLine($"- Expected terms: {report.Score.ExpectedTermHits}/{report.Score.ExpectedTermCount}");
        builder.AppendLine($"- Expected files: {report.Score.ExpectedFileHits}/{report.Score.ExpectedFileCount}");
        builder.AppendLine($"- Useful files: {report.Score.UsefulFileCount}");
        builder.AppendLine($"- Non-useful files: {report.Score.NonUsefulFileCount}");
        builder.AppendLine($"- Noise term hits: {report.Score.NoiseTermHits}");
        builder.AppendLine($"- Token budget ratio: {report.Score.TokenBudgetRatio:F3}");
        builder.AppendLine();
        builder.AppendLine("## Output Metrics");
        builder.AppendLine();
        builder.AppendLine($"- Search results: {report.SearchResultCount}");
        builder.AppendLine($"- Seed type: {FormatNullable(report.SeedType)}");
        builder.AppendLine($"- Seed member: {FormatNullable(report.SeedMember)}");
        builder.AppendLine($"- Files: {report.FileCount}");
        builder.AppendLine($"- Blocks: {report.BlockCount}");
        builder.AppendLine($"- Selected lines: {report.SelectedLineCount}");
        builder.AppendLine($"- Estimated tokens: {report.EstimatedTokenCount}");
        builder.AppendLine($"- Usage callers: {FormatNullable(report.TotalCallerCount?.ToString())}");
        builder.AppendLine($"- Usage clusters: {FormatNullable(report.TotalClusterCount?.ToString())}");

        if (searchResponse is not null) {
            builder.AppendLine();
            builder.AppendLine("## Symbol Search Top Results");
            builder.AppendLine();
            foreach (var result in searchResponse.Results.Take(8)) {
                builder.AppendLine($"- `{result.DisplayName}` ({result.TargetKind})");
            }
        }

        if (response is not null) {
            builder.AppendLine();
            builder.AppendLine("## Selected Files");
            builder.AppendLine();
            foreach (var file in response.Files) {
                builder.AppendLine($"- `{file.Path}`: {file.SelectedLineCount}/{file.TotalLineCount} lines, {file.Blocks.Count} blocks");
            }

            if (response.UsageSummary is not null) {
                builder.AppendLine();
                builder.AppendLine("## Usage Summary Samples");
                builder.AppendLine();
                foreach (var cluster in response.UsageSummary.Clusters.Take(8)) {
                    builder.AppendLine($"- `{cluster.ProjectName}` / `{FormatNullable(cluster.ModuleName)}`: {cluster.CallerCount} callers");
                    foreach (var sample in cluster.Samples.Take(3)) {
                        builder.AppendLine($"  - `{sample.TypeDisplayName}` -> `{sample.MemberDisplayName}`");
                    }
                }
            }
        }

        return builder.ToString();
    }

    public static string RenderComparison(EvaluationComparison comparison) {
        var builder = new StringBuilder();
        builder.AppendLine("# Scenario evaluation before/after comparison");
        builder.AppendLine();
        builder.AppendLine("## Aggregate");
        builder.AppendLine();
        builder.AppendLine($"- Scenarios: {comparison.Aggregate.ScenarioCount}");
        builder.AppendLine($"- Improved: {comparison.Aggregate.ImprovedCount}");
        builder.AppendLine($"- Regressed: {comparison.Aggregate.RegressedCount}");
        builder.AppendLine($"- Unchanged: {comparison.Aggregate.UnchangedCount}");
        builder.AppendLine($"- Average helpfulness delta: {comparison.Aggregate.AverageHelpfulnessDelta:F3}");
        builder.AppendLine($"- Estimated token delta: {comparison.Aggregate.EstimatedTokenDelta}");
        builder.AppendLine($"- Selected line delta: {comparison.Aggregate.SelectedLineDelta}");
        builder.AppendLine($"- Non-useful file delta: {comparison.Aggregate.NonUsefulFileDelta}");
        builder.AppendLine();
        builder.AppendLine("## Baseline Aggregate");
        builder.AppendLine();
        AppendAggregate(builder, comparison.BaselineAggregate);
        builder.AppendLine();
        builder.AppendLine("## After Aggregate");
        builder.AppendLine();
        AppendAggregate(builder, comparison.AfterAggregate);
        builder.AppendLine();
        builder.AppendLine("## Scenario Deltas");
        builder.AppendLine();
        builder.AppendLine("| Scenario | Repo | Category | Score delta | Token delta | Line delta | Non-useful file delta | Rating |");
        builder.AppendLine("| --- | --- | --- | ---: | ---: | ---: | ---: | --- |");
        foreach (var scenario in comparison.Scenarios.OrderByDescending(item => Math.Abs(item.HelpfulnessDelta))) {
            builder.AppendLine($"| `{scenario.Key}` | {scenario.RepositoryName} | {scenario.Category} | {scenario.HelpfulnessDelta:F3} | {scenario.EstimatedTokenDelta} | {scenario.SelectedLineDelta} | {scenario.NonUsefulFileDelta} | {scenario.BaselineRating} -> {scenario.AfterRating} |");
        }

        return builder.ToString();
    }

    private static void AppendAggregate(StringBuilder builder, EvaluationAggregate aggregate) {
        builder.AppendLine($"- Scenarios: {aggregate.ScenarioCount}");
        builder.AppendLine($"- Introduction scenarios: {aggregate.IntroductionScenarioCount}");
        builder.AppendLine($"- Average helpfulness: {aggregate.AverageHelpfulnessScore:F3}");
        builder.AppendLine($"- Average term coverage: {aggregate.AverageExpectedTermCoverage:F3}");
        builder.AppendLine($"- Average file coverage: {aggregate.AverageExpectedFileCoverage:F3}");
        builder.AppendLine($"- Average non-useful file ratio: {aggregate.AverageNonUsefulFileRatio:F3}");
        builder.AppendLine($"- Average token budget ratio: {aggregate.AverageTokenBudgetRatio:F3}");
        builder.AppendLine($"- Ratings: {aggregate.GoodCount} good, {aggregate.MixedCount} mixed, {aggregate.PoorCount} poor, {aggregate.FailedCount} failed");
        builder.AppendLine($"- Estimated tokens: {aggregate.TotalEstimatedTokens}");
        builder.AppendLine($"- Selected lines: {aggregate.TotalSelectedLines}");
        builder.AppendLine($"- Useful files: {aggregate.TotalUsefulFiles}");
        builder.AppendLine($"- Non-useful files: {aggregate.TotalNonUsefulFiles}");
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
}

internal static class JsonOptions {
    public static readonly JsonSerializerOptions Indented = new() {
        WriteIndented = true,
    };
}
