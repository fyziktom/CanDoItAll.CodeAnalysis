using CanDoItAll.CodeAnalytics.Analysis.Graphs;
using CanDoItAll.CodeAnalytics.Analysis.Rules;
using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Tests.Support;

namespace CanDoItAll.CodeAnalytics.Tests.Unit;

public sealed class RiskRulesFacts {
    [Fact]
    public void RiskRules_emits_layering_findings_and_open_questions() {
        var snapshot = SampleSnapshotFactory.Create();
        var builder = new ArchitectureInsightBuilder(new StronglyConnectedComponentFinder());

        var insights = builder.Build(
            snapshot.Request,
            snapshot.Facts,
            [new AnalysisDiagnostic("DI0001", AnalysisDiagnosticSeverity.Info, "Factory registration is only partially interpreted.")]);

        Assert.Contains(insights.Findings, finding => finding.RuleId == "LAYERING-001");
        Assert.Contains(insights.OpenQuestions, question => question.Description.Contains("Factory registration", StringComparison.Ordinal));
    }
}
