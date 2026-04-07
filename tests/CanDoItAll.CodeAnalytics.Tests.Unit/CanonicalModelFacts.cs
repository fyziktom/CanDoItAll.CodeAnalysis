using CanDoItAll.CodeAnalytics.Tests.Support;

namespace CanDoItAll.CodeAnalytics.Tests.Unit;

public sealed class CanonicalModelFacts {
    [Fact]
    public void CanonicalModel_snapshot_represents_the_required_sections() {
        var snapshot = SampleSnapshotFactory.Create();

        Assert.NotEmpty(snapshot.Facts.Documents);
        Assert.NotEmpty(snapshot.Facts.Namespaces);
        Assert.NotEmpty(snapshot.Facts.Types);
        Assert.NotEmpty(snapshot.Facts.Members);
        Assert.NotEmpty(snapshot.Facts.ServiceRegistrations);
        Assert.NotEmpty(snapshot.Facts.DbContexts);
        Assert.NotEmpty(snapshot.Facts.Entities);
        Assert.NotEmpty(snapshot.Exports.Artifacts);
    }

    [Fact]
    public void CanonicalModel_keeps_facts_and_insights_separate() {
        var snapshot = SampleSnapshotFactory.Create();

        Assert.Empty(snapshot.Facts.Types.Select(type => type.TypeId).Intersect(snapshot.Insights.Findings.Select(finding => finding.FindingId)));
        Assert.True(snapshot.Insights.Summary.FindingCount >= snapshot.Insights.Findings.Count);
    }
}
