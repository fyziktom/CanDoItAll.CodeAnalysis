using CanDoItAll.CodeAnalytics.Abstractions;
using CanDoItAll.CodeAnalytics.Abstractions.Commands;
using CanDoItAll.CodeAnalytics.Abstractions.Queries;
using CanDoItAll.CodeAnalytics.Abstractions.Responses;
using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Domain.Sources;
using CanDoItAll.CodeAnalytics.Storage.Paths;
using CanDoItAll.CodeAnalytics.Storage.Snapshots;
using CanDoItAll.CodeAnalytics.Tests.Support;

namespace CanDoItAll.CodeAnalytics.Tests.Unit;

public sealed partial class ApplicationFacts {
    [Fact]
    public async Task Application_builds_and_queries_a_fixture_snapshot() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        var service = ApplicationServiceFactory.Create(output.Path);

        var build = await service.BuildSnapshotAsync(new BuildArchitectureSnapshotCommand(FixturePaths.GetFixtureSolutionPath(), ForceRefresh: true));
        var dashboard = await service.GetDashboardAsync(build.Snapshot.SnapshotId);
        var findings = await service.GetFindingsAsync(new SnapshotQuery(build.Snapshot.SnapshotId));

        Assert.NotNull(dashboard);
        Assert.NotNull(findings);
        Assert.Equal(build.Snapshot.SnapshotId, dashboard!.Snapshot.SnapshotId);
        Assert.True(findings!.Findings.Count > 0 || findings.OpenQuestions.Count > 0);
        Assert.NotEmpty(build.Snapshot.Facts.TypeRelationships);
        Assert.NotEmpty(build.Snapshot.Facts.EntityRelationships);
    }

    [Fact]
    public async Task Application_builds_a_project_scoped_snapshot_from_a_csproj_path() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        var service = ApplicationServiceFactory.Create(output.Path);

        var response = await service.BuildSnapshotAsync(
            new BuildArchitectureSnapshotCommand(
                FixturePaths.GetFixtureProjectPath("Fixture.Shop.Infrastructure"),
                ForceRefresh: true));

        Assert.Equal("Fixture.Shop.Infrastructure", response.Snapshot.Facts.Solution.Name);
        Assert.Single(response.Snapshot.Facts.Projects);
        Assert.Equal("Fixture.Shop.Infrastructure", response.Snapshot.Facts.Projects[0].Name);
        Assert.All(
            response.Snapshot.Facts.Types,
            type => Assert.Equal(response.Snapshot.Facts.Projects[0].ProjectId, type.ProjectId));
    }

    [Fact]
    public async Task Application_filters_types_by_project_and_can_expand_methods() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        var service = ApplicationServiceFactory.Create(output.Path);

        var build = await service.BuildSnapshotAsync(new BuildArchitectureSnapshotCommand(FixturePaths.GetFixtureSolutionPath(), ForceRefresh: true));
        var response = await service.GetTypesAsync(
            new TypeSearchQuery(
                build.Snapshot.SnapshotId,
                ProjectName: "Fixture.Shop.Application",
                MemberSearchText: "PlaceOrderAsync",
                IncludeMembers: true,
                MethodsOnly: true));

        Assert.NotNull(response);
        Assert.NotEmpty(response!.Types);
        Assert.All(response.Types, item => Assert.Equal("Fixture.Shop.Application", item.ProjectName));
        Assert.Contains(
            response.Types.SelectMany(item => item.Members),
            member => member.Kind == CanDoItAll.CodeAnalytics.Domain.Facts.MemberKind.Method &&
                member.DisplayName.Contains("PlaceOrderAsync", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Application_returns_solution_inventory_with_direct_and_reverse_project_references() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        var service = ApplicationServiceFactory.Create(output.Path);

        var build = await service.BuildSnapshotAsync(new BuildArchitectureSnapshotCommand(FixturePaths.GetFixtureSolutionPath(), ForceRefresh: true));
        var response = await service.GetSolutionInventoryAsync(new SolutionInventoryQuery(build.Snapshot.SnapshotId));

        Assert.NotNull(response);
        Assert.Equal(build.Snapshot.Facts.Solution.Name, response!.Solution.Name);

        var contractsProject = Assert.Single(
            response.Projects,
            item => string.Equals(item.Project.Name, "Fixture.Shop.Contracts", StringComparison.Ordinal));
        Assert.Empty(contractsProject.DirectProjectReferences);
        Assert.Equal(3, contractsProject.ReferencedByProjects.Count);
        Assert.Contains(
            contractsProject.ReferencedByProjects,
            item => string.Equals(item.ProjectName, "Fixture.Shop.Application", StringComparison.Ordinal));
        Assert.Contains(
            contractsProject.ReferencedByProjects,
            item => string.Equals(item.ProjectName, "Fixture.Shop.Infrastructure", StringComparison.Ordinal));
        Assert.Contains(
            contractsProject.ReferencedByProjects,
            item => string.Equals(item.ProjectName, "Fixture.Shop.Web", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Application_returns_project_inventory_with_documents() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        var service = ApplicationServiceFactory.Create(output.Path);

        var build = await service.BuildSnapshotAsync(new BuildArchitectureSnapshotCommand(FixturePaths.GetFixtureSolutionPath(), ForceRefresh: true));
        var response = await service.GetProjectInventoryAsync(
            new ProjectInventoryQuery(
                build.Snapshot.SnapshotId,
                ProjectName: "Fixture.Shop.Application",
                IncludeDocuments: true));

        Assert.NotNull(response);
        Assert.Equal("Fixture.Shop.Application", response!.Project.Project.Name);
        Assert.Equal(2, response.Project.DirectProjectReferences.Count);
        Assert.Contains(
            response.Project.DirectProjectReferences,
            item => string.Equals(item.ProjectName, "Fixture.Shop.Contracts", StringComparison.Ordinal));
        Assert.Contains(
            response.Project.DirectProjectReferences,
            item => string.Equals(item.ProjectName, "Fixture.Shop.Infrastructure", StringComparison.Ordinal));
        Assert.Single(response.Project.ReferencedByProjects);
        Assert.Contains(
            response.Project.Documents,
            item => item.Path.EndsWith("OrderService.cs", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Application_separates_product_and_supporting_projects_in_inventory() {
        using var output = new TemporaryDirectoryScope();
        var snapshot = CreateInventoryClassificationSnapshot();
        await StoreSnapshotAsync(snapshot, output.Path);

        var service = ApplicationServiceFactory.Create(output.Path);
        var response = await service.GetProjectInventoryAsync(
            new ProjectInventoryQuery(
                snapshot.SnapshotId,
                ProjectName: "Fixture.Shop.Application",
                IncludeDocuments: false));

        Assert.NotNull(response);
        Assert.Equal(ProjectRoleKind.Product, response!.Project.ProjectRole);
        Assert.Single(response.Project.DirectProjectReferences);
        Assert.Equal(ProjectRoleKind.Product, response.Project.DirectProjectReferences[0].ProjectRole);
        Assert.Empty(response.Project.SupportingDirectProjectReferences);
        Assert.Single(response.Project.ReferencedByProjects);
        Assert.Equal("Fixture.Shop.Web", response.Project.ReferencedByProjects[0].ProjectName);
        Assert.Equal(ProjectRoleKind.Product, response.Project.ReferencedByProjects[0].ProjectRole);
        Assert.Equal(2, response.Project.SupportingReferencedByProjects.Count);
        Assert.Contains(
            response.Project.SupportingReferencedByProjects,
            item => string.Equals(item.ProjectName, "Fixture.Shop.Application.Tests", StringComparison.Ordinal)
                && item.ProjectRole == ProjectRoleKind.Test);
        Assert.Contains(
            response.Project.SupportingReferencedByProjects,
            item => string.Equals(item.ProjectName, "Fixture.Shop.Application.Benchmarks", StringComparison.Ordinal)
                && item.ProjectRole == ProjectRoleKind.Benchmark);
    }

    [Fact]
    public async Task Application_returns_document_source_for_snapshot_document_path() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        var service = ApplicationServiceFactory.Create(output.Path);

        var build = await service.BuildSnapshotAsync(new BuildArchitectureSnapshotCommand(FixturePaths.GetFixtureSolutionPath(), ForceRefresh: true));
        var response = await service.GetDocumentSourceAsync(
            new DocumentQuery(
                build.Snapshot.SnapshotId,
                DocumentPath: "src/Fixture.Shop.Application/Orders/OrderService.cs"));

        Assert.NotNull(response);
        Assert.Equal("Fixture.Shop.Application", response!.ProjectName);
        Assert.Equal("OrderService.cs", response.Document.Name);
        Assert.Contains("public sealed partial class OrderService", response.SourceCode, StringComparison.Ordinal);
        Assert.Contains("PlaceOrderAsync", response.SourceCode, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Application_returns_document_symbols_for_snapshot_document_path() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        var service = ApplicationServiceFactory.Create(output.Path);

        var build = await service.BuildSnapshotAsync(new BuildArchitectureSnapshotCommand(FixturePaths.GetFixtureSolutionPath(), ForceRefresh: true));
        var response = await service.GetDocumentSymbolsAsync(
            new DocumentQuery(
                build.Snapshot.SnapshotId,
                DocumentPath: "src/Fixture.Shop.Application/Orders/OrderService.cs"));

        Assert.NotNull(response);
        Assert.Equal("Fixture.Shop.Application", response!.ProjectName);
        var orderService = Assert.Single(response.Types);
        Assert.Contains("OrderService", orderService.Type.DisplayName, StringComparison.Ordinal);
        Assert.Contains(
            orderService.Members,
            item => item.DisplayName.Contains("PlaceOrderAsync", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Application_document_queries_tolerate_duplicate_document_ids() {
        using var workspace = new TemporaryDirectoryScope();
        using var output = new TemporaryDirectoryScope();

        var solutionPath = Path.Combine(workspace.Path, "Fixture.Shop.slnx");
        var orderServiceDirectory = Path.Combine(workspace.Path, "src", "Fixture.Shop.Application", "Orders");
        Directory.CreateDirectory(orderServiceDirectory);
        await File.WriteAllTextAsync(solutionPath, string.Empty);
        await WriteOrderServiceSourceAsync(orderServiceDirectory);

        var original = SampleSnapshotFactory.Create();
        var duplicateDocumentId = original.Facts.Documents[0].DocumentId;
        var snapshot = original with {
            Request = original.Request with {
                SolutionPath = solutionPath,
            },
            Facts = original.Facts with {
                Documents = [
                    .. original.Facts.Documents,
                    new DocumentFact(
                        duplicateDocumentId,
                        "proj-app",
                        "src/Fixture.Shop.Application/Orders/OrderService.Duplicate.cs",
                        "OrderService.Duplicate.cs",
                        12),
                ],
            },
        };
        await StoreSnapshotAsync(snapshot, output.Path);

        var service = ApplicationServiceFactory.Create(output.Path);
        var response = await service.GetDocumentSymbolsAsync(
            new DocumentQuery(
                snapshot.SnapshotId,
                DocumentPath: "src/Fixture.Shop.Application/Orders/OrderService.cs"));

        Assert.NotNull(response);
        var orderService = Assert.Single(response!.Types);
        Assert.Contains("OrderService", orderService.Type.DisplayName, StringComparison.Ordinal);
    }

    private static async Task StoreSnapshotAsync(ArchitectureSnapshot snapshot, string outputPath) {
        var repository = new FileSnapshotRepository(new SnapshotJsonSerializer());
        var pathResolver = new SnapshotPathResolver(outputPath);
        var requestHash = repository.ComputeRequestHash(snapshot.Request, snapshot.GeneratorVersion, snapshot.SchemaVersion);
        await repository.StoreAsync(pathResolver, snapshot, requestHash, [], CancellationToken.None);
    }

    private static ArchitectureSnapshot CreateInventoryClassificationSnapshot() {
        var original = SampleSnapshotFactory.Create();
        var projects = new[]
        {
            new ProjectFact("proj-app", "Fixture.Shop.Application", "src/Fixture.Shop.Application/Fixture.Shop.Application.csproj", ["net10.0"], ["proj-infra"], [], 2),
            new ProjectFact("proj-infra", "Fixture.Shop.Infrastructure", "src/Fixture.Shop.Infrastructure/Fixture.Shop.Infrastructure.csproj", ["net10.0"], [], ["Microsoft.EntityFrameworkCore"], 1),
            new ProjectFact("proj-web", "Fixture.Shop.Web", "src/Fixture.Shop.Web/Fixture.Shop.Web.csproj", ["net10.0"], ["proj-app"], [], 1),
            new ProjectFact("proj-tests", "Fixture.Shop.Application.Tests", "tests/Fixture.Shop.Application.Tests/Fixture.Shop.Application.Tests.csproj", ["net10.0"], ["proj-app"], ["Microsoft.NET.Test.Sdk"], 1),
            new ProjectFact("proj-bench", "Fixture.Shop.Application.Benchmarks", "benchmarks/Fixture.Shop.Application.Benchmarks/Fixture.Shop.Application.Benchmarks.csproj", ["net10.0"], ["proj-app"], ["BenchmarkDotNet"], 1),
        };

        return original with {
            Facts = original.Facts with {
                Solution = original.Facts.Solution with {
                    ProjectCount = projects.Length,
                },
                Projects = projects,
            },
        };
    }

    private static Task WriteOrderServiceSourceAsync(string orderServiceDirectory) {
        return File.WriteAllTextAsync(
            Path.Combine(orderServiceDirectory, "OrderService.cs"),
            """
            namespace Fixture.Shop.Application.Orders;

            public sealed class OrderService
            {
                public OrderService()
                {
                }

                public async Task<OrderReceipt> PlaceOrderAsync(PlaceOrderCommand command, CancellationToken cancellationToken = default)
                {
                    var customer = new Customer();
                    var order = CreateOrder(command, customer);

                    _dbContext.Customers.Add(customer);
                    _dbContext.Orders.Add(order);
                    await _dbContext.SaveChangesAsync(cancellationToken);

                    return new OrderReceipt(order.Id);
                }
            }
            """);
    }
}
