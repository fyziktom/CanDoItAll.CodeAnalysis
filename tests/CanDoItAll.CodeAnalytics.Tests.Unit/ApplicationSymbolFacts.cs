using CanDoItAll.CodeAnalytics.Abstractions;
using CanDoItAll.CodeAnalytics.Abstractions.Commands;
using CanDoItAll.CodeAnalytics.Abstractions.Queries;
using CanDoItAll.CodeAnalytics.Abstractions.Responses;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Tests.Support;

namespace CanDoItAll.CodeAnalytics.Tests.Unit;

public sealed partial class ApplicationFacts {
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
}
