using CanDoItAll.CodeAnalytics.Abstractions.Commands;
using CanDoItAll.CodeAnalytics.Abstractions.Queries;
using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Sources;
using CanDoItAll.CodeAnalytics.Storage.Paths;
using CanDoItAll.CodeAnalytics.Storage.Snapshots;
using CanDoItAll.CodeAnalytics.Tests.Support;

namespace CanDoItAll.CodeAnalytics.Tests.Unit;

public sealed class ApplicationFacts {
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
    public async Task Application_returns_focused_context_for_a_service_seed() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        var service = ApplicationServiceFactory.Create(output.Path);

        var build = await service.BuildSnapshotAsync(new BuildArchitectureSnapshotCommand(FixturePaths.GetFixtureSolutionPath(), ForceRefresh: true));
        var seedService = Assert.Single(
            build.Snapshot.Facts.ServiceRegistrations,
            item => string.Equals(item.ServiceTypeDisplayName, "Fixture.Shop.Contracts.Orders.IOrderService", StringComparison.Ordinal));
        var response = await service.GetFocusedContextAsync(new FocusedContextQuery(build.Snapshot.SnapshotId, ServiceRegistrationId: seedService.ServiceRegistrationId, Depth: 2));

        Assert.NotNull(response);
        Assert.NotNull(response!.SeedService);
        Assert.NotEmpty(response.Types);
        Assert.NotEmpty(response.Members);
        Assert.Contains(response.RelatedServices, item => item.ServiceRegistrationId == seedService.ServiceRegistrationId);
    }

    [Fact]
    public async Task Application_resolves_focused_context_from_prompt_text_and_returns_file_excerpts() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        var service = ApplicationServiceFactory.Create(output.Path);

        var build = await service.BuildSnapshotAsync(new BuildArchitectureSnapshotCommand(FixturePaths.GetFixtureSolutionPath(), ForceRefresh: true));
        var response = await service.GetFocusedContextAsync(
            new FocusedContextQuery(
                build.Snapshot.SnapshotId,
                Depth: 2,
                QueryText: "PlaceOrderAsync",
                FocusTags: ["Db"]));

        Assert.NotNull(response);
        Assert.NotNull(response!.SeedMember);
        Assert.Contains("PlaceOrderAsync", response.SeedMember!.DisplayName, StringComparison.Ordinal);
        Assert.Contains(response.FocusTags, item => string.Equals(item, "db", StringComparison.Ordinal));
        Assert.NotEmpty(response.Files);
        Assert.True(response.Stats.SelectedLineCount > 0);
        Assert.Contains(response.Files, file => file.Path.EndsWith("OrderService.cs", StringComparison.Ordinal));
        Assert.Contains(
            response.Files.SelectMany(file => file.Blocks),
            block => block.Code.Contains("SaveChangesAsync", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Application_resolves_focused_context_from_diagnostic_text_when_source_is_available() {
        using var workspace = new TemporaryDirectoryScope();
        using var output = new TemporaryDirectoryScope();

        var solutionPath = Path.Combine(workspace.Path, "Fixture.Shop.slnx");
        var orderServiceDirectory = Path.Combine(workspace.Path, "src", "Fixture.Shop.Application", "Orders");
        Directory.CreateDirectory(orderServiceDirectory);
        await File.WriteAllTextAsync(solutionPath, string.Empty);
        await WriteOrderServiceSourceAsync(orderServiceDirectory);

        var original = SampleSnapshotFactory.Create();
        var snapshot = original with {
            Request = original.Request with {
                SolutionPath = solutionPath,
            },
            Diagnostics = [
                new AnalysisDiagnostic(
                    "DI0001",
                    AnalysisDiagnosticSeverity.Info,
                    "Factory registration is only partially interpreted.",
                    new SourceReference("src/Fixture.Shop.Application/Orders/OrderService.cs", 20, 1, 20, 1)),
            ],
        };
        var repository = new FileSnapshotRepository(new SnapshotJsonSerializer());
        var pathResolver = new SnapshotPathResolver(output.Path);
        var requestHash = repository.ComputeRequestHash(snapshot.Request, snapshot.GeneratorVersion, snapshot.SchemaVersion);
        await repository.StoreAsync(pathResolver, snapshot, requestHash, [], CancellationToken.None);

        var service = ApplicationServiceFactory.Create(output.Path);
        var response = await service.GetFocusedContextAsync(
            new FocusedContextQuery(
                snapshot.SnapshotId,
                Depth: 1,
                QueryText: "Factory registration is only partially interpreted."));

        Assert.NotNull(response);
        Assert.NotNull(response!.SeedMember);
        Assert.Contains("diagnostic", response.SeedExplanation, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("PlaceOrderAsync", response.SeedMember!.DisplayName, StringComparison.Ordinal);
        Assert.NotEmpty(response.Files);
    }

    [Fact]
    public async Task Application_merges_duplicate_document_paths_when_building_focused_context_files() {
        using var workspace = new TemporaryDirectoryScope();
        using var output = new TemporaryDirectoryScope();

        var solutionPath = Path.Combine(workspace.Path, "Fixture.Shop.slnx");
        var orderServiceDirectory = Path.Combine(workspace.Path, "src", "Fixture.Shop.Application", "Orders");
        Directory.CreateDirectory(orderServiceDirectory);
        await File.WriteAllTextAsync(solutionPath, string.Empty);
        await WriteOrderServiceSourceAsync(orderServiceDirectory);

        var original = SampleSnapshotFactory.Create();
        var snapshot = original with {
            Request = original.Request with {
                SolutionPath = solutionPath,
            },
            Facts = original.Facts with {
                Documents = [
                    .. original.Facts.Documents,
                    new DocumentFact("doc-duplicate", "proj-app", @"src\Fixture.Shop.Application\Orders\OrderService.cs", "OrderService.cs", 99),
                ],
            },
        };
        var repository = new FileSnapshotRepository(new SnapshotJsonSerializer());
        var pathResolver = new SnapshotPathResolver(output.Path);
        var requestHash = repository.ComputeRequestHash(snapshot.Request, snapshot.GeneratorVersion, snapshot.SchemaVersion);
        await repository.StoreAsync(pathResolver, snapshot, requestHash, [], CancellationToken.None);

        var service = ApplicationServiceFactory.Create(output.Path);
        var response = await service.GetFocusedContextAsync(
            new FocusedContextQuery(
                snapshot.SnapshotId,
                Depth: 1,
                QueryText: "PlaceOrderAsync"));

        Assert.NotNull(response);
        var file = Assert.Single(response!.Files, item => item.Path.EndsWith("OrderService.cs", StringComparison.Ordinal));
        Assert.Equal(99, file.TotalLineCount);
    }

    [Fact]
    public async Task Application_prefers_behavioral_seed_members_for_type_name_queries() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        var service = ApplicationServiceFactory.Create(output.Path);

        var build = await service.BuildSnapshotAsync(new BuildArchitectureSnapshotCommand(FixturePaths.GetFixtureSolutionPath(), ForceRefresh: true));
        var response = await service.GetFocusedContextAsync(
            new FocusedContextQuery(
                build.Snapshot.SnapshotId,
                Depth: 1,
                QueryText: "OrderService"));

        Assert.NotNull(response);
        Assert.NotNull(response!.SeedType);
        Assert.NotNull(response.SeedMember);
        Assert.Equal(MemberKind.Method, response.SeedMember!.Kind);
        Assert.DoesNotContain("OrderService(", response.SeedMember.DisplayName, StringComparison.Ordinal);
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
