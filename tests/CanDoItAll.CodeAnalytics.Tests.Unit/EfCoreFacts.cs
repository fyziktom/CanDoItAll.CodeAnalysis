using CanDoItAll.CodeAnalytics.Abstractions.Commands;
using CanDoItAll.CodeAnalytics.Abstractions.Queries;
using CanDoItAll.CodeAnalytics.Domain.Facts;
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

    [Fact]
    public async Task EfCore_collects_store_objects_contexts_and_relationships_without_runtime_query_claims() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        var service = ApplicationServiceFactory.Create(output.Path);

        var response = await service.BuildSnapshotAsync(new BuildArchitectureSnapshotCommand(FixturePaths.GetFixtureSolutionPath(), ForceRefresh: true));

        Assert.Contains(response.Snapshot.Facts.DbContexts, item => item.DisplayName == "ShopDbContext");
        Assert.Contains(response.Snapshot.Facts.DbContexts, item => item.DisplayName == "ReportingDbContext");
        Assert.Contains(
            response.Snapshot.Facts.Entities,
            item => item.DisplayName == "Order" &&
                item.TableName == "Orders" &&
                item.Schema == "sales");
        Assert.Contains(
            response.Snapshot.Facts.Entities,
            item => item.DisplayName == "ReportingSnapshot" &&
                item.TableName == "ReportingSnapshots" &&
                item.Schema == "reporting");
        Assert.Contains(
            response.Snapshot.Facts.EntityRelationships,
            item => item.Kind == EntityRelationshipKind.OneToMany &&
                item.NavigationPropertyNames.Contains("Customer", StringComparer.Ordinal) &&
                item.NavigationPropertyNames.Contains("Orders", StringComparer.Ordinal));

        var diagnosticText = string.Join(
            Environment.NewLine,
            response.Snapshot.Diagnostics.Select(item => $"{item.Code} {item.Message}"));
        Assert.DoesNotContain("N+1", diagnosticText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("AsNoTracking", diagnosticText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("split query", diagnosticText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("compiled query", diagnosticText, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task EfCore_persistence_view_filters_metadata_without_query_tuning_advice() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        var service = ApplicationServiceFactory.Create(output.Path);

        var build = await service.BuildSnapshotAsync(new BuildArchitectureSnapshotCommand(FixturePaths.GetFixtureSolutionPath(), ForceRefresh: true));
        var response = await service.GetPersistenceAsync(new SnapshotQuery(build.Snapshot.SnapshotId, SearchText: "Reporting"));

        Assert.NotNull(response);
        Assert.Contains(response!.DbContexts, item => item.DisplayName == "ReportingDbContext");
        Assert.Contains(response.Entities, item => item.DisplayName == "ReportingSnapshot");
        Assert.DoesNotContain(response.Entities, item => item.DisplayName == "Order");
        Assert.DoesNotContain(
            response.Diagnostics,
            item => item.Message.Contains("N+1", StringComparison.OrdinalIgnoreCase) ||
                item.Message.Contains("AsNoTracking", StringComparison.OrdinalIgnoreCase) ||
                item.Message.Contains("split query", StringComparison.OrdinalIgnoreCase));
    }
}
