using CanDoItAll.CodeAnalytics.Abstractions.Commands;
using CanDoItAll.CodeAnalytics.Tests.Support;

namespace CanDoItAll.CodeAnalytics.Tests.Unit;

public sealed class ServiceRegistrationFacts {
    [Fact]
    public async Task ServiceRegistration_captures_conventional_and_factory_patterns() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        var service = ApplicationServiceFactory.Create(output.Path);

        var response = await service.BuildSnapshotAsync(new BuildArchitectureSnapshotCommand(FixturePaths.GetFixtureSolutionPath(), ForceRefresh: true));

        Assert.Contains(response.Snapshot.Facts.ServiceRegistrations, item => item.ServiceTypeDisplayName.Contains("IOrderService", StringComparison.Ordinal));
        Assert.Contains(response.Snapshot.Diagnostics, diagnostic => diagnostic.Code == "DI0001");
    }
}
