using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Exports;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Rendering.Markdown;
using CanDoItAll.CodeAnalytics.Rendering.Mermaid;

namespace CanDoItAll.CodeAnalytics.Rendering.Exports;

public sealed class ExportBundleBuilder {
    private readonly MarkdownSummaryWriter _markdownSummaryWriter;
    private readonly ProjectGraphMermaidRenderer _projectGraphRenderer;
    private readonly ClassDiagramMermaidRenderer _classDiagramRenderer;
    private readonly ErDiagramMermaidRenderer _erDiagramRenderer;

    public ExportBundleBuilder(
        MarkdownSummaryWriter markdownSummaryWriter,
        ProjectGraphMermaidRenderer projectGraphRenderer,
        ClassDiagramMermaidRenderer classDiagramRenderer,
        ErDiagramMermaidRenderer erDiagramRenderer) {
        _markdownSummaryWriter = markdownSummaryWriter;
        _projectGraphRenderer = projectGraphRenderer;
        _classDiagramRenderer = classDiagramRenderer;
        _erDiagramRenderer = erDiagramRenderer;
    }

    public RenderingResult Build(ArchitectureSnapshot snapshot, int maxDiagramNodes) {
        var diagnostics = new List<AnalysisDiagnostic>();
        var exports = new List<PreparedExport>
        {
            CreateExport(
                ExportArtifactKind.MarkdownSummary,
                "exports/summary.md",
                "Markdown summary",
                "High-level architecture summary.",
                _markdownSummaryWriter.Write(snapshot)),
        };

        if (snapshot.Request.IncludeMermaidExports) {
            exports.Add(
                CreateExport(
                    ExportArtifactKind.MermaidProjectGraph,
                    "exports/project-graph.mmd",
                    "Project graph",
                    "Project dependency diagram.",
                    _projectGraphRenderer.Render(snapshot.Facts.Projects, snapshot.Facts.Dependencies)));
            exports.Add(CreateClassDiagram(snapshot, maxDiagramNodes, diagnostics));
            exports.Add(CreateErDiagram(snapshot, maxDiagramNodes, diagnostics));
        }

        return new RenderingResult(
            exports.OrderBy(item => item.RelativePath, StringComparer.Ordinal).ToArray(),
            diagnostics.OrderBy(item => item.Code, StringComparer.Ordinal).ToArray());
    }

    private PreparedExport CreateClassDiagram(
        ArchitectureSnapshot snapshot,
        int maxDiagramNodes,
        ICollection<AnalysisDiagnostic> diagnostics) {
        if (snapshot.Facts.Types.Count > maxDiagramNodes) {
            diagnostics.Add(
                new AnalysisDiagnostic(
                    "MRD0001",
                    AnalysisDiagnosticSeverity.Info,
                    $"Class diagram truncated to {maxDiagramNodes} types."));
        }

        var content = _classDiagramRenderer.Render(snapshot.Facts.Types, maxDiagramNodes);
        return CreateExport(
            ExportArtifactKind.MermaidClassDiagram,
            "exports/class-diagram.mmd",
            "Class diagram",
            "Type inheritance and interface relationships.",
            content);
    }

    private PreparedExport CreateErDiagram(
        ArchitectureSnapshot snapshot,
        int maxDiagramNodes,
        ICollection<AnalysisDiagnostic> diagnostics) {
        if (snapshot.Facts.Entities.Count > maxDiagramNodes) {
            diagnostics.Add(
                new AnalysisDiagnostic(
                    "MRD0002",
                    AnalysisDiagnosticSeverity.Info,
                    $"ER diagram truncated to {maxDiagramNodes} entities."));
        }

        var content = _erDiagramRenderer.Render(snapshot.Facts.Entities, maxDiagramNodes);
        return CreateExport(
            ExportArtifactKind.MermaidErDiagram,
            "exports/er-diagram.mmd",
            "ER diagram",
            "Entity relationship overview.",
            content);
    }

    private static PreparedExport CreateExport(
        ExportArtifactKind kind,
        string relativePath,
        string title,
        string description,
        string content) {
        return new PreparedExport(kind, relativePath, title, description, content);
    }
}
