using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Exports;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Insights;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Domain.Sources;

namespace CanDoItAll.CodeAnalytics.Tests.Support;

public static class SampleSnapshotFactory {
    public static ArchitectureSnapshot Create() {
        var request = new AnalysisRequest(
            "/repo/Fixture.Shop/Fixture.Shop.slnx",
            [],
            [],
            true,
            true,
            true,
            true,
            true);
        var solution = new SolutionFact("Fixture.Shop", request.SolutionPath, 2, 3);
        var projects = new[]
        {
            new ProjectFact("proj-app", "Fixture.Shop.Application", "src/Fixture.Shop.Application/Fixture.Shop.Application.csproj", ["net10.0"], ["proj-infra"], [], 2),
            new ProjectFact("proj-infra", "Fixture.Shop.Infrastructure", "src/Fixture.Shop.Infrastructure/Fixture.Shop.Infrastructure.csproj", ["net10.0"], [], ["Microsoft.EntityFrameworkCore"], 1),
        };
        var documents = new[]
        {
            new DocumentFact("doc-1", "proj-app", "src/Fixture.Shop.Application/Orders/OrderService.cs", "OrderService.cs", 42),
        };
        var modules = new[]
        {
            new ModuleFact("mod-orders", "proj-app", "Fixture.Shop.Application.Orders", "Fixture.Shop.Application.Orders", ["ns-orders"], ["type-order-service"]),
        };
        var namespaces = new[]
        {
            new NamespaceFact("ns-orders", "proj-app", "mod-orders", "Fixture.Shop.Application.Orders", ["type-order-service"]),
        };
        var types = new[]
        {
            new TypeFact(
                "type-order-service",
                "proj-app",
                "mod-orders",
                "ns-orders",
                "Fixture.Shop.Application.Orders.OrderService",
                TypeKind.Class,
                null,
                ["Fixture.Shop.Contracts.Orders.IOrderService"],
                ["member-order-ctor", "member-order-method"],
                "Places orders against the shop database.",
                new SourceReference("src/Fixture.Shop.Application/Orders/OrderService.cs", 8, 21)),
        };
        var members = new[]
        {
            new MemberFact("member-order-ctor", "type-order-service", "OrderService.OrderService()", MemberKind.Constructor, "OrderService", [], new SourceReference("src/Fixture.Shop.Application/Orders/OrderService.cs", 12, 12)),
            new MemberFact("member-order-method", "type-order-service", "PlaceOrderAsync", MemberKind.Method, "Task<OrderReceipt>", ["PlaceOrderCommand"], new SourceReference("src/Fixture.Shop.Application/Orders/OrderService.cs", 20, 18)),
        };
        var services = new[]
        {
            new ServiceRegistrationFact("svc-order", "proj-app", "mod-orders", ServiceLifetimeKind.Scoped, "Fixture.Shop.Contracts.Orders.IOrderService", "Fixture.Shop.Application.Orders.OrderService", "AddScoped", false, new SourceReference("src/Fixture.Shop.Web/Program.cs", 10, 1)),
        };
        var dbContexts = new[]
        {
            new DbContextFact("dbctx-shop", "type-shop-dbcontext", "proj-infra", "mod-persistence", "ShopDbContext", ["ent-customer", "ent-order"], new SourceReference("src/Fixture.Shop.Infrastructure/Persistence/ShopDbContext.cs", 8, 21)),
        };
        var entities = new[]
        {
            new EntityFact("ent-customer", "type-customer", "proj-infra", "mod-persistence", "Customer", "Customers", "sales", ["Id"], ["ent-order"], new SourceReference("src/Fixture.Shop.Infrastructure/Persistence/Entities/Customer.cs", 3, 21)),
            new EntityFact("ent-order", "type-order", "proj-infra", "mod-persistence", "Order", "Orders", "sales", ["Id"], ["ent-customer"], new SourceReference("src/Fixture.Shop.Infrastructure/Persistence/Entities/Order.cs", 3, 21)),
        };
        var entityRelationships = new[]
        {
            new EntityRelationshipFact("entrel-customer-orders", "ent-customer", "ent-order", EntityRelationshipKind.OneToMany, ["Customer", "Orders"]),
        };
        var dependencies = new[]
        {
            new DependencyEdgeFact("dep-project", DependencyKind.ProjectReference, "proj-app", "proj-infra", 1),
        };
        var facts = new ArchitectureFacts(
            solution,
            projects,
            documents,
            modules,
            namespaces,
            types,
            members,
            [],
            services,
            dbContexts,
            entities,
            entityRelationships,
            dependencies);
        var insights = new ArchitectureInsights(
            new RiskSummaryInsight(2, 1, 2, 1, 2, 1, 1, 1),
            [],
            [new HotspotInsight("type-order-service", "Type", 0.72, "fan-in=2, members=2")],
            [new FindingInsight("finding-layering", "LAYERING-001", FindingSeverity.Warning, FindingCategory.Layering, "Application depends on Infrastructure", "Fixture.Shop.Application references infrastructure.", "This couples orchestration to persistence details.", 0.92, ["proj-app", "proj-infra"])],
            [new OpenQuestionInsight("question-di", "Collector ambiguity remains", "Factory registration is only partially interpreted.", 0.55, [])]);
        var exports = new ArchitectureExports(
            [
                new ExportArtifact(ExportArtifactKind.SnapshotJson, "snapshot.json", "Snapshot JSON", "Canonical architecture snapshot."),
                new ExportArtifact(ExportArtifactKind.MarkdownSummary, "exports/summary.md", "Markdown summary", "High-level architecture summary.", 128),
            ]);
        var diagnostics = new[]
        {
            new AnalysisDiagnostic("DI0001", AnalysisDiagnosticSeverity.Info, "Factory registration is only partially interpreted.", new SourceReference("src/Fixture.Shop.Web/Program.cs", 14, 1)),
        };

        return new ArchitectureSnapshot(
            "1.1.0",
            "0.1.0",
            "snap-fixture-001",
            DateTimeOffset.Parse("2026-04-07T12:00:00Z"),
            request,
            facts,
            insights,
            exports,
            diagnostics);
    }
}
