using CanDoItAll.CodeAnalytics.Abstractions.Commands;
using CanDoItAll.CodeAnalytics.Tests.Support;

namespace CanDoItAll.CodeAnalytics.Tests.Integration;

public sealed class DiRegistrationFacts {
    [Fact]
    public async Task DiRegistration_snapshot_contains_service_lifetimes_and_factory_diagnostics() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        var service = ApplicationServiceFactory.Create(output.Path);

        var response = await service.BuildSnapshotAsync(new BuildArchitectureSnapshotCommand(FixturePaths.GetFixtureSolutionPath(), ForceRefresh: true));

        Assert.Contains(response.Snapshot.Facts.ServiceRegistrations, registration => registration.Lifetime == CanDoItAll.CodeAnalytics.Domain.Facts.ServiceLifetimeKind.Scoped);
        Assert.Contains(response.Snapshot.Diagnostics, diagnostic => diagnostic.Code == "DI0001");
    }
}
