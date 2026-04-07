using CanDoItAll.CodeAnalytics.Abstractions.Commands;
using CanDoItAll.CodeAnalytics.Tests.Support;

namespace CanDoItAll.CodeAnalytics.Tests.Unit;

public sealed class EfCoreFacts {
    [Fact]
    public async Task EfCore_collects_entities_and_reports_partially_supported_patterns() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        var service = ApplicationServiceFactory.Create(output.Path);

        var response = await service.BuildSnapshotAsync(new BuildArchitectureSnapshotCommand(FixturePaths.GetFixtureSolutionPath(), ForceRefresh: true));

        Assert.Contains(response.Snapshot.Facts.DbContexts, item => item.DisplayName == "ShopDbContext");
        Assert.Contains(response.Snapshot.Facts.Entities, item => item.DisplayName == "Order");
        Assert.Contains(response.Snapshot.Diagnostics, diagnostic => diagnostic.Code == "EF0003");
    }
}
