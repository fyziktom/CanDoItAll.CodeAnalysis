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

        var content = renderer.Render(snapshot.Facts.Entities, 50);

        GoldenFileAssert.EqualToFile("exports/er-diagram.mmd", content);
    }
}
