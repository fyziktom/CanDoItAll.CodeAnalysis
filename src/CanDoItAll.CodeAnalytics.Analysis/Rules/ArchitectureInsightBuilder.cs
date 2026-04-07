using CanDoItAll.CodeAnalytics.Analysis.Graphs;
using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Identifiers;
using CanDoItAll.CodeAnalytics.Domain.Insights;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;

namespace CanDoItAll.CodeAnalytics.Analysis.Rules;

public sealed class ArchitectureInsightBuilder {
    private readonly StronglyConnectedComponentFinder _componentFinder;

    public ArchitectureInsightBuilder(StronglyConnectedComponentFinder componentFinder) {
        _componentFinder = componentFinder;
    }

    public ArchitectureInsights Build(
        AnalysisRequest request,
        ArchitectureFacts facts,
        IReadOnlyList<AnalysisDiagnostic> diagnostics) {
        if (!request.IncludeRisks) {
            return new ArchitectureInsights(
                CreateSummary(facts, diagnostics, [], []),
                [],
                [],
                [],
                []);
        }

        var cycles = BuildCycles(facts);
        var findings = BuildFindings(facts, diagnostics, cycles);
        var openQuestions = BuildOpenQuestions(diagnostics);
        var hotspots = BuildHotspots(facts);

        return new ArchitectureInsights(
            CreateSummary(facts, diagnostics, findings, openQuestions),
            cycles,
            hotspots,
            findings,
            openQuestions);
    }

    private IReadOnlyList<CycleInsight> BuildCycles(ArchitectureFacts facts) {
        var projectCycles = CreateCycles(facts.Dependencies, DependencyKind.ProjectReference, "Project");
        var moduleCycles = CreateCycles(facts.Dependencies, DependencyKind.ModuleDependency, "Module");
        var typeCycles = CreateCycles(facts.Dependencies, DependencyKind.TypeDependency, "Type");

        return projectCycles
            .Concat(moduleCycles)
            .Concat(typeCycles)
            .OrderBy(cycle => cycle.Level, StringComparer.Ordinal)
            .ThenBy(cycle => string.Join("|", cycle.NodeIds), StringComparer.Ordinal)
            .ToArray();
    }

    private IReadOnlyList<CycleInsight> CreateCycles(
        IReadOnlyList<DependencyEdgeFact> dependencies,
        DependencyKind dependencyKind,
        string level) {
        var relevantEdges = dependencies.Where(edge => edge.Kind == dependencyKind).ToArray();
        var nodes = relevantEdges
            .SelectMany(edge => new[] { edge.FromId, edge.ToId })
            .Distinct(StringComparer.Ordinal)
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();

        var adjacency = nodes.ToDictionary(
            node => node,
            node => (IReadOnlyList<string>)relevantEdges
                .Where(edge => string.Equals(edge.FromId, node, StringComparison.Ordinal))
                .Select(edge => edge.ToId)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(value => value, StringComparer.Ordinal)
                .ToArray(),
            StringComparer.Ordinal);

        return _componentFinder.FindCycles(adjacency)
            .Select(component => new CycleInsight(level, component))
            .ToArray();
    }

    private static IReadOnlyList<FindingInsight> BuildFindings(
        ArchitectureFacts facts,
        IReadOnlyList<AnalysisDiagnostic> diagnostics,
        IReadOnlyList<CycleInsight> cycles) {
        var findings = new List<FindingInsight>();

        foreach (var cycle in cycles) {
            findings.Add(
                new FindingInsight(
                    StableId.ForFinding($"cycle:{cycle.Level}:{string.Join('|', cycle.NodeIds)}"),
                    "DEPENDENCY-001",
                    cycle.Level == "Project" ? FindingSeverity.Error : FindingSeverity.Warning,
                    FindingCategory.Dependency,
                    $"{cycle.Level} cycle detected",
                    $"The {cycle.Level.ToLowerInvariant()} graph contains a strongly connected component.",
                    "Strongly connected components make architectural direction harder to reason about.",
                    0.95,
                    cycle.NodeIds));
        }

        foreach (var project in facts.Projects) {
            if (project.Name.Contains(".Application", StringComparison.Ordinal) &&
                project.ProjectReferences.Any(reference => facts.Projects.Any(candidate =>
                    string.Equals(candidate.ProjectId, reference, StringComparison.Ordinal) &&
                    candidate.Name.Contains(".Infrastructure", StringComparison.Ordinal)))) {
                findings.Add(
                    new FindingInsight(
                        StableId.ForFinding($"layering:{project.ProjectId}"),
                        "LAYERING-001",
                        FindingSeverity.Warning,
                        FindingCategory.Layering,
                        "Application depends on Infrastructure",
                        $"{project.Name} directly references an Infrastructure project.",
                        "This couples business orchestration to persistence details.",
                        0.92,
                        project.ProjectReferences.Append(project.ProjectId).OrderBy(value => value, StringComparer.Ordinal).ToArray(),
                        new Domain.Sources.SourceReference(project.Path)));
            }
        }

        foreach (var document in facts.Documents.Where(document => document.LineCount > 350)) {
            findings.Add(
                new FindingInsight(
                    StableId.ForFinding($"file-size:{document.DocumentId}"),
                    "COMPLEXITY-001",
                    FindingSeverity.Warning,
                    FindingCategory.Complexity,
                    "Large source file detected",
                    $"{document.Path} has {document.LineCount} lines.",
                    "Oversized files are harder to review and usually hide multiple responsibilities.",
                    0.88,
                    [document.DocumentId],
                    new Domain.Sources.SourceReference(document.Path)));
        }

        foreach (var type in facts.Types.Where(type => type.MemberIds.Count > 8)) {
            findings.Add(
                new FindingInsight(
                    StableId.ForFinding($"type-size:{type.TypeId}"),
                    "COMPLEXITY-002",
                    FindingSeverity.Info,
                    FindingCategory.Complexity,
                    "Type with many members",
                    $"{type.DisplayName} exposes {type.MemberIds.Count} source members.",
                    "Large types often become aggregation points for unrelated behavior.",
                    0.71,
                    [type.TypeId],
                    type.Source));
        }

        foreach (var diagnostic in diagnostics.Where(diagnostic => diagnostic.Code.StartsWith("XML", StringComparison.Ordinal))) {
            findings.Add(
                new FindingInsight(
                    StableId.ForFinding($"documentation:{diagnostic.Code}:{diagnostic.Message}"),
                    "DOCUMENTATION-001",
                    FindingSeverity.Info,
                    FindingCategory.Documentation,
                    "XML documentation could not be normalized",
                    diagnostic.Message,
                    "Malformed XML documentation reduces the quality of generated summaries.",
                    0.8,
                    [],
                    diagnostic.Source));
        }

        return findings
            .OrderBy(finding => finding.RuleId, StringComparer.Ordinal)
            .ThenBy(finding => finding.Title, StringComparer.Ordinal)
            .ToArray();
    }

    private static IReadOnlyList<OpenQuestionInsight> BuildOpenQuestions(IReadOnlyList<AnalysisDiagnostic> diagnostics) {
        return diagnostics
            .Where(diagnostic => diagnostic.Code.StartsWith("DI", StringComparison.Ordinal) || diagnostic.Code.StartsWith("EF", StringComparison.Ordinal))
            .Where(diagnostic => diagnostic.Severity != AnalysisDiagnosticSeverity.Error)
            .Select(
                diagnostic => new OpenQuestionInsight(
                    StableId.ForFinding($"question:{diagnostic.Code}:{diagnostic.Message}"),
                    "Collector ambiguity remains",
                    diagnostic.Message,
                    0.55,
                    [],
                    diagnostic.Source))
            .OrderBy(question => question.Description, StringComparer.Ordinal)
            .ToArray();
    }

    private static IReadOnlyList<HotspotInsight> BuildHotspots(ArchitectureFacts facts) {
        var incomingCounts = facts.Dependencies
            .Where(edge => edge.Kind is DependencyKind.TypeDependency or DependencyKind.ServiceDependency)
            .GroupBy(edge => edge.ToId)
            .ToDictionary(group => group.Key, group => group.Sum(edge => edge.Weight), StringComparer.Ordinal);

        return facts.Types
            .Select(
                type => {
                    incomingCounts.TryGetValue(type.TypeId, out var fanIn);
                    var score = Math.Min(1d, ((fanIn * 0.4d) + (type.MemberIds.Count * 0.2d)) / 10d);
                    return new HotspotInsight(
                        type.TypeId,
                        "Type",
                        Math.Round(score, 2, MidpointRounding.AwayFromZero),
                        $"fan-in={fanIn}, members={type.MemberIds.Count}");
                })
            .Where(hotspot => hotspot.Score > 0)
            .OrderByDescending(hotspot => hotspot.Score)
            .ThenBy(hotspot => hotspot.NodeId, StringComparer.Ordinal)
            .Take(10)
            .ToArray();
    }

    private static RiskSummaryInsight CreateSummary(
        ArchitectureFacts facts,
        IReadOnlyList<AnalysisDiagnostic> diagnostics,
        IReadOnlyList<FindingInsight> findings,
        IReadOnlyList<OpenQuestionInsight> openQuestions) {
        return new RiskSummaryInsight(
            facts.Projects.Count,
            facts.Types.Count,
            facts.Members.Count,
            facts.ServiceRegistrations.Count,
            facts.Entities.Count,
            findings.Count,
            openQuestions.Count,
            diagnostics.Count);
    }
}
