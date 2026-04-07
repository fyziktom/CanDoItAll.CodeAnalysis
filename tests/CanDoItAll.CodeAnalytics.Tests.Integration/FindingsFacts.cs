using CanDoItAll.CodeAnalytics.Abstractions.Commands;
using CanDoItAll.CodeAnalytics.Tests.Support;

namespace CanDoItAll.CodeAnalytics.Tests.Integration;

public sealed class FindingsFacts {
    [Fact]
    public async Task Findings_include_layering_and_collector_uncertainty() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        var service = ApplicationServiceFactory.Create(output.Path);

        var response = await service.BuildSnapshotAsync(new BuildArchitectureSnapshotCommand(FixturePaths.GetFixtureSolutionPath(), ForceRefresh: true));

        Assert.Contains(response.Snapshot.Insights.Findings, finding => finding.RuleId == "LAYERING-001");
        Assert.NotEmpty(response.Snapshot.Insights.OpenQuestions);
    }
}
