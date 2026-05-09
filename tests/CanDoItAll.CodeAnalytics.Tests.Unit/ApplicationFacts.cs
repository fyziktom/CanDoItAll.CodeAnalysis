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
    public async Task Application_searches_symbols_by_exact_type_name() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        var service = ApplicationServiceFactory.Create(output.Path);

        var build = await service.BuildSnapshotAsync(new BuildArchitectureSnapshotCommand(FixturePaths.GetFixtureSolutionPath(), ForceRefresh: true));
        var response = await service.SearchSymbolsAsync(
            new SymbolSearchQuery(
                build.Snapshot.SnapshotId,
                SearchText: "IOrderService",
                SearchMode: SymbolSearchMode.Exact));

        Assert.NotNull(response);
        var result = Assert.Single(response!.Results, item => item.TargetKind == SymbolTargetKind.Type);
        Assert.Contains("IOrderService", result.DisplayName, StringComparison.Ordinal);
        Assert.Contains("interface", result.Declaration, StringComparison.Ordinal);
        Assert.Contains(SymbolMatchFieldKind.DisplayName, result.MatchFields);
    }

    [Fact]
    public async Task Application_returns_symbol_definition_for_a_member_target() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        var service = ApplicationServiceFactory.Create(output.Path);

        var build = await service.BuildSnapshotAsync(new BuildArchitectureSnapshotCommand(FixturePaths.GetFixtureSolutionPath(), ForceRefresh: true));
        var orderService = Assert.Single(build.Snapshot.Facts.Types, item => string.Equals(item.DisplayName, "Fixture.Shop.Application.Orders.OrderService", StringComparison.Ordinal));
        var placeOrder = Assert.Single(
            build.Snapshot.Facts.Members,
            item => string.Equals(item.TypeId, orderService.TypeId, StringComparison.Ordinal)
                && item.DisplayName.Contains("PlaceOrderAsync", StringComparison.Ordinal));

        var response = await service.GetSymbolDefinitionAsync(
            new SymbolDefinitionQuery(
                build.Snapshot.SnapshotId,
                orderService.TypeId,
                placeOrder.MemberId));

        Assert.NotNull(response);
        Assert.Equal(SymbolTargetKind.Member, response!.TargetKind);
        Assert.Contains("PlaceOrderAsync", response.Member!.DisplayName, StringComparison.Ordinal);
        Assert.Contains("OrderService.cs", response.Definition.Path, StringComparison.Ordinal);
        Assert.Contains("PlaceOrderAsync", response.Definition.Code, StringComparison.Ordinal);
        Assert.NotNull(response.ContainingTypeHeader);
    }

    [Fact]
    public async Task Application_lists_symbol_members_for_a_type() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        var service = ApplicationServiceFactory.Create(output.Path);

        var build = await service.BuildSnapshotAsync(new BuildArchitectureSnapshotCommand(FixturePaths.GetFixtureSolutionPath(), ForceRefresh: true));
        var orderService = Assert.Single(build.Snapshot.Facts.Types, item => string.Equals(item.DisplayName, "Fixture.Shop.Application.Orders.OrderService", StringComparison.Ordinal));

        var response = await service.GetSymbolMembersAsync(new SymbolMembersQuery(build.Snapshot.SnapshotId, orderService.TypeId));

        Assert.NotNull(response);
        Assert.Contains(response!.Members, item => item.DisplayName.Contains("PlaceOrderAsync", StringComparison.Ordinal));
        Assert.Contains(response.Members, item => item.Kind == MemberKind.Constructor);
    }

    [Fact]
    public async Task Application_lists_symbol_implementations_for_a_contract() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        var service = ApplicationServiceFactory.Create(output.Path);

        var build = await service.BuildSnapshotAsync(new BuildArchitectureSnapshotCommand(FixturePaths.GetFixtureSolutionPath(), ForceRefresh: true));
        var repositoryContract = Assert.Single(build.Snapshot.Facts.Types, item => string.Equals(item.DisplayName, "Fixture.Shop.Contracts.Persistence.IRepository<TEntity>", StringComparison.Ordinal));

        var response = await service.GetSymbolImplementationsAsync(new SymbolImplementationsQuery(build.Snapshot.SnapshotId, repositoryContract.TypeId));

        Assert.NotNull(response);
        Assert.Contains(
            response!.Implementations,
            item => item.Kind == SymbolImplementationKind.InterfaceImplementation
                && item.Type.DisplayName.Contains("EfRepository<TEntity>", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Application_returns_symbol_references_for_member_invocations() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        var service = ApplicationServiceFactory.Create(output.Path);

        var build = await service.BuildSnapshotAsync(new BuildArchitectureSnapshotCommand(FixturePaths.GetFixtureSolutionPath(), ForceRefresh: true));
        var notificationSender = Assert.Single(build.Snapshot.Facts.Types, item => string.Equals(item.DisplayName, "Fixture.Shop.Contracts.Notifications.INotificationSender", StringComparison.Ordinal));
        var sendAsync = Assert.Single(
            build.Snapshot.Facts.Members,
            item => string.Equals(item.TypeId, notificationSender.TypeId, StringComparison.Ordinal)
                && item.DisplayName.Contains("SendAsync", StringComparison.Ordinal));

        var response = await service.GetSymbolReferencesAsync(
            new SymbolReferencesQuery(
                build.Snapshot.SnapshotId,
                notificationSender.TypeId,
                sendAsync.MemberId));

        Assert.NotNull(response);
        Assert.True(response!.TotalCount > 0);
        Assert.Contains(
            response.References,
            item => item.Kind == SymbolReferenceKind.Invocation
                && item.SourceMember is not null
                && item.SourceMember.DisplayName.Contains("PlaceOrderAsync", StringComparison.Ordinal));
        Assert.Contains(
            response.References.Select(item => item.ContextExcerpt.Code),
            code => code.Contains("SendAsync", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Application_returns_symbol_references_for_type_dependencies() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        var service = ApplicationServiceFactory.Create(output.Path);

        var build = await service.BuildSnapshotAsync(new BuildArchitectureSnapshotCommand(FixturePaths.GetFixtureSolutionPath(), ForceRefresh: true));
        var shopDbContext = Assert.Single(build.Snapshot.Facts.Types, item => string.Equals(item.DisplayName, "Fixture.Shop.Infrastructure.Persistence.ShopDbContext", StringComparison.Ordinal));

        var response = await service.GetSymbolReferencesAsync(new SymbolReferencesQuery(build.Snapshot.SnapshotId, shopDbContext.TypeId));

        Assert.NotNull(response);
        Assert.True(response!.TotalCount > 0);
        Assert.Contains(
            response.References,
            item => item.SourceType.DisplayName.Contains("OrderService", StringComparison.Ordinal)
                && item.Kind is SymbolReferenceKind.ConstructorParameter or SymbolReferenceKind.Field);
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
    public async Task Application_maps_behavior_intent_to_trouble_path_for_focused_context() {
        FixtureSolutionHost.EnsurePrepared();
        using var output = new TemporaryDirectoryScope();
        var service = ApplicationServiceFactory.Create(output.Path);

        var build = await service.BuildSnapshotAsync(new BuildArchitectureSnapshotCommand(FixturePaths.GetFixtureSolutionPath(), ForceRefresh: true));
        var response = await service.GetFocusedContextAsync(
            new FocusedContextQuery(
                build.Snapshot.SnapshotId,
                Depth: 2,
                QueryText: "PlaceOrderAsync",
                Intent: FocusedContextIntent.Behavior,
                Precision: FocusedContextPrecision.Balanced));

        Assert.NotNull(response);
        Assert.Equal(FocusedContextIntent.Behavior, response!.RequestedIntent);
        Assert.Equal(FocusedContextIntent.TroublePath, response.ResolvedIntent);
        Assert.NotNull(response.SeedMember);
        Assert.Contains("behavior", response.StrategyExplanation, StringComparison.OrdinalIgnoreCase);
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

    [Fact]
    public async Task Application_switches_high_fan_in_helpers_into_surgical_definition_mode() {
        using var workspace = new TemporaryDirectoryScope();
        using var output = new TemporaryDirectoryScope();

        var snapshot = await FocusedContextHelperSnapshotFactory.CreateHighFanInHelperSnapshotAsync(workspace.Path);
        await StoreSnapshotAsync(snapshot, output.Path);

        var service = ApplicationServiceFactory.Create(output.Path);
        var response = await service.GetFocusedContextAsync(
            new FocusedContextQuery(
                snapshot.SnapshotId,
                Depth: 3,
                QueryText: "IClock"));

        Assert.NotNull(response);
        Assert.Equal(FocusedContextIntent.Definition, response!.ResolvedIntent);
        Assert.Equal(FocusedContextPrecision.Surgical, response.ResolvedPrecision);
        Assert.NotEmpty(response.ImplementationTypes);
        Assert.NotNull(response.UsageSummary);
        Assert.Equal(7, response.UsageSummary!.TotalCallerCount);
        Assert.Equal(5, response.UsageSummary.TotalClusterCount);
        Assert.True(response.UsageSummary.OmittedCallerCount > 0);
        Assert.True(response.Members.Count < 7);
        Assert.Contains(response.Files, item => item.Path.EndsWith("IClock.cs", StringComparison.Ordinal));
        Assert.Contains(response.Files, item => item.Path.EndsWith("SystemClock.cs", StringComparison.Ordinal));
        Assert.Contains(response.Members, item => item.TypeId == "type-system-clock");
    }

    [Fact]
    public async Task Application_can_render_usage_summary_without_pulling_consumer_members_into_main_selection() {
        using var workspace = new TemporaryDirectoryScope();
        using var output = new TemporaryDirectoryScope();

        var snapshot = await FocusedContextHelperSnapshotFactory.CreateHighFanInHelperSnapshotAsync(workspace.Path);
        await StoreSnapshotAsync(snapshot, output.Path);

        var service = ApplicationServiceFactory.Create(output.Path);
        var response = await service.GetFocusedContextAsync(
            new FocusedContextQuery(
                snapshot.SnapshotId,
                Depth: 2,
                QueryText: "IClock",
                Intent: FocusedContextIntent.UsageSummary,
                Precision: FocusedContextPrecision.Surgical));

        Assert.NotNull(response);
        Assert.Equal(FocusedContextIntent.UsageSummary, response!.ResolvedIntent);
        Assert.Equal(FocusedContextPrecision.Surgical, response.ResolvedPrecision);
        Assert.NotNull(response.UsageSummary);
        Assert.DoesNotContain(
            response.Members,
            item => item.TypeId is "type-order-service"
                or "type-invoice-service"
                or "type-reminder-service"
                or "type-digest-service"
                or "type-dashboard-page"
                or "type-report-builder"
                or "type-cleanup-job");
    }

    [Fact]
    public async Task Application_filters_high_fan_in_helper_usage_summary_by_relation_hints() {
        using var workspace = new TemporaryDirectoryScope();
        using var output = new TemporaryDirectoryScope();

        var snapshot = await FocusedContextHelperSnapshotFactory.CreateHighFanInHelperSnapshotAsync(workspace.Path);
        await StoreSnapshotAsync(snapshot, output.Path);

        var service = ApplicationServiceFactory.Create(output.Path);
        var response = await service.GetFocusedContextAsync(
            new FocusedContextQuery(
                snapshot.SnapshotId,
                Depth: 2,
                QueryText: "IClock",
                Intent: FocusedContextIntent.UsageSummary,
                Precision: FocusedContextPrecision.Surgical,
                RelationHints: ["DashboardPage"]));

        Assert.NotNull(response);
        Assert.Equal(["dashboardpage"], response!.RelationHints);
        Assert.NotNull(response.UsageSummary);
        Assert.Equal(1, response.UsageSummary!.TotalCallerCount);
        Assert.Single(response.UsageSummary.Clusters);
        Assert.Equal(0, response.UsageSummary.OmittedCallerCount);
        var sample = Assert.Single(response.UsageSummary.Clusters[0].Samples);
        Assert.Contains("DashboardPage", sample.TypeDisplayName, StringComparison.Ordinal);
        Assert.DoesNotContain(
            response.UsageSummary.Clusters.SelectMany(cluster => cluster.Samples),
            item => item.TypeDisplayName.Contains("OrderService", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Application_does_not_fall_back_to_broad_helper_usage_when_relation_hints_do_not_match() {
        using var workspace = new TemporaryDirectoryScope();
        using var output = new TemporaryDirectoryScope();

        var snapshot = await FocusedContextHelperSnapshotFactory.CreateHighFanInHelperSnapshotAsync(workspace.Path);
        await StoreSnapshotAsync(snapshot, output.Path);

        var service = ApplicationServiceFactory.Create(output.Path);
        var response = await service.GetFocusedContextAsync(
            new FocusedContextQuery(
                snapshot.SnapshotId,
                Depth: 2,
                QueryText: "IClock",
                Intent: FocusedContextIntent.UsageSummary,
                Precision: FocusedContextPrecision.Surgical,
                RelationHints: ["MissingWidget"]));

        Assert.NotNull(response);
        Assert.Equal(["missingwidget"], response!.RelationHints);
        Assert.Null(response.UsageSummary);
        Assert.DoesNotContain(
            response.Members,
            item => item.TypeId is "type-order-service"
                or "type-invoice-service"
                or "type-reminder-service"
                or "type-digest-service"
                or "type-dashboard-page"
                or "type-report-builder"
                or "type-cleanup-job");
    }

    [Fact]
    public async Task Application_returns_structured_selection_reasons_for_members_and_files() {
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
        Assert.NotEmpty(response!.SelectionReasons);
        Assert.Contains(
            response.SelectionReasons,
            item => item.TargetKind == FocusedContextSelectionTargetKind.Member
                && string.Equals(item.TargetId, response.SeedMember!.MemberId, StringComparison.Ordinal)
                && item.ReasonKind == FocusedContextSelectionReasonKind.Seed);
        Assert.Contains(
            response.SelectionReasons,
            item => item.TargetKind == FocusedContextSelectionTargetKind.File
                && item.TargetId.EndsWith("OrderService.cs", StringComparison.Ordinal)
                && item.ReasonKind is FocusedContextSelectionReasonKind.Seed or FocusedContextSelectionReasonKind.SeedContext);
    }

    [Fact]
    public async Task Application_supports_outline_precision_without_code_excerpts() {
        using var workspace = new TemporaryDirectoryScope();
        using var output = new TemporaryDirectoryScope();

        var snapshot = await FocusedContextHelperSnapshotFactory.CreateHighFanInHelperSnapshotAsync(workspace.Path);
        await StoreSnapshotAsync(snapshot, output.Path);

        var service = ApplicationServiceFactory.Create(output.Path);
        var response = await service.GetFocusedContextAsync(
            new FocusedContextQuery(
                snapshot.SnapshotId,
                Depth: 2,
                QueryText: "IClock",
                Intent: FocusedContextIntent.Definition,
                Precision: FocusedContextPrecision.Outline));

        Assert.NotNull(response);
        Assert.Equal(FocusedContextIntent.Definition, response!.ResolvedIntent);
        Assert.Equal(FocusedContextPrecision.Outline, response.ResolvedPrecision);
        Assert.NotEmpty(response.ImplementationTypes);
        Assert.NotNull(response.UsageSummary);
        Assert.Empty(response.Files);
        Assert.Equal(0, response.Stats.SelectedLineCount);
        Assert.NotEmpty(response.SelectionReasons);
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
