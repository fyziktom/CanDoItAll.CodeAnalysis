using CanDoItAll.CodeAnalytics.Rendering.Mermaid;
using CanDoItAll.CodeAnalytics.Tests.Support;

namespace CanDoItAll.CodeAnalytics.Tests.Unit;

public sealed class MermaidFacts {
    [Fact]
    public void Mermaid_renders_the_expected_project_graph() {
        var snapshot = SampleSnapshotFactory.Create();
        var renderer = new ProjectGraphMermaidRenderer();

        var content = renderer.Render(snapshot.Facts.Projects, snapshot.Facts.Dependencies);

        GoldenFileAssert.EqualToFile("exports/project-graph.mmd", content);
    }

    [Fact]
    public void Mermaid_renders_the_expected_er_diagram() {
        var snapshot = SampleSnapshotFactory.Create();
        var renderer = new ErDiagramMermaidRenderer();

        var content = renderer.Render(snapshot.Facts.Entities, snapshot.Facts.EntityRelationships, 50);

        GoldenFileAssert.EqualToFile("exports/er-diagram.mmd", content);
        Assert.Contains("||--o{", content, StringComparison.Ordinal);
    }

    [Fact]
    public void Mermaid_renders_class_diagram_with_safe_aliases_and_member_relations() {
        var renderer = new ClassDiagramMermaidRenderer();
        var types = new[]
        {
            new CanDoItAll.CodeAnalytics.Domain.Facts.TypeFact(
                "type-base",
                "proj-app",
                "mod-core",
                "ns-core",
                "Fixture.Shop.Core.BaseHandler",
                CanDoItAll.CodeAnalytics.Domain.Facts.TypeKind.Class,
                null,
                [],
                [],
                null,
                new CanDoItAll.CodeAnalytics.Domain.Sources.SourceReference("src/BaseHandler.cs", 1, 1)),
            new CanDoItAll.CodeAnalytics.Domain.Facts.TypeFact(
                "type-interface",
                "proj-app",
                "mod-core",
                "ns-core",
                "Fixture.Shop.Core.ICommandHandler<PlaceOrder>",
                CanDoItAll.CodeAnalytics.Domain.Facts.TypeKind.Interface,
                null,
                [],
                [],
                null,
                new CanDoItAll.CodeAnalytics.Domain.Sources.SourceReference("src/ICommandHandler.cs", 1, 1)),
            new CanDoItAll.CodeAnalytics.Domain.Facts.TypeFact(
                "type-repository",
                "proj-app",
                "mod-core",
                "ns-core",
                "Fixture.Shop.Core.OrderRepository",
                CanDoItAll.CodeAnalytics.Domain.Facts.TypeKind.Class,
                null,
                [],
                [],
                null,
                new CanDoItAll.CodeAnalytics.Domain.Sources.SourceReference("src/OrderRepository.cs", 1, 1)),
            new CanDoItAll.CodeAnalytics.Domain.Facts.TypeFact(
                "type-handler",
                "proj-app",
                "mod-core",
                "ns-core",
                "Fixture.Shop.Core.PlaceOrderHandler<TRequest>",
                CanDoItAll.CodeAnalytics.Domain.Facts.TypeKind.Class,
                "Fixture.Shop.Core.BaseHandler",
                ["Fixture.Shop.Core.ICommandHandler<PlaceOrder>"],
                [],
                null,
                new CanDoItAll.CodeAnalytics.Domain.Sources.SourceReference("src/PlaceOrderHandler.cs", 1, 1)),
        };
        var relationships = new[]
        {
            new CanDoItAll.CodeAnalytics.Domain.Facts.TypeRelationshipFact(
                "typerel-property",
                "type-handler",
                "type-repository",
                CanDoItAll.CodeAnalytics.Domain.Facts.TypeRelationshipKind.Property,
                1,
                new CanDoItAll.CodeAnalytics.Domain.Sources.SourceReference("src/PlaceOrderHandler.cs", 12, 9)),
            new CanDoItAll.CodeAnalytics.Domain.Facts.TypeRelationshipFact(
                "typerel-method-return",
                "type-handler",
                "type-repository",
                CanDoItAll.CodeAnalytics.Domain.Facts.TypeRelationshipKind.MethodReturn,
                1,
                new CanDoItAll.CodeAnalytics.Domain.Sources.SourceReference("src/PlaceOrderHandler.cs", 18, 9)),
        };

        var content = renderer.Render(types, relationships, 50);

        Assert.Contains("class T0001", content, StringComparison.Ordinal);
        Assert.Contains("&lt;PlaceOrder&gt;", content, StringComparison.Ordinal);
        Assert.Contains("<|--", content, StringComparison.Ordinal);
        Assert.Contains("<|..", content, StringComparison.Ordinal);
        Assert.Contains("-->", content, StringComparison.Ordinal);
        Assert.Contains("property", content, StringComparison.Ordinal);
        Assert.Contains("returns", content, StringComparison.Ordinal);
        Assert.DoesNotContain("--|>", content, StringComparison.Ordinal);
        Assert.DoesNotContain("..|>", content, StringComparison.Ordinal);
    }
}
