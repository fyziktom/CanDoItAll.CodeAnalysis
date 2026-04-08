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
        var exports = new List<PreparedExport> {
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
            exports.AddRange(CreateClassDiagrams(snapshot, maxDiagramNodes, diagnostics));
            exports.AddRange(CreateErDiagrams(snapshot, maxDiagramNodes, diagnostics));
        }

        return new RenderingResult(
            exports.OrderBy(item => item.RelativePath, StringComparer.Ordinal).ToArray(),
            diagnostics.OrderBy(item => item.Code, StringComparer.Ordinal).ToArray());
    }

    private IReadOnlyList<PreparedExport> CreateClassDiagrams(
        ArchitectureSnapshot snapshot,
        int maxDiagramNodes,
        ICollection<AnalysisDiagnostic> diagnostics) {
        var exports = new List<PreparedExport>();
        var eligibleTypes = snapshot.Facts.Types
            .Where(type => IsDefaultClassDiagramCandidate(type, snapshot))
            .ToArray();

        foreach (var project in snapshot.Facts.Projects.OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase)) {
            exports.AddRange(
                CreateScopedClassDiagramExports(
                    snapshot,
                    eligibleTypes.Where(type => string.Equals(type.ProjectId, project.ProjectId, StringComparison.Ordinal)).ToArray(),
                    $"project-{Slugify(project.Name)}",
                    $"Class diagram - {project.Name}",
                    $"Type relationships within {project.Name}.",
                    maxDiagramNodes,
                    diagnostics));
        }

        foreach (var module in snapshot.Facts.Modules.OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase)) {
            exports.AddRange(
                CreateScopedClassDiagramExports(
                    snapshot,
                    eligibleTypes.Where(type => string.Equals(type.ModuleId, module.ModuleId, StringComparison.Ordinal)).ToArray(),
                    $"module-{Slugify(module.Name)}",
                    $"Class diagram - {module.Name}",
                    $"Type relationships within module {module.Name}.",
                    maxDiagramNodes,
                    diagnostics));
        }

        return exports;
    }

    private IReadOnlyList<PreparedExport> CreateScopedClassDiagramExports(
        ArchitectureSnapshot snapshot,
        IReadOnlyList<CanDoItAll.CodeAnalytics.Domain.Facts.TypeFact> scopedTypes,
        string scopeKey,
        string title,
        string description,
        int maxDiagramNodes,
        ICollection<AnalysisDiagnostic> diagnostics) {
        if (scopedTypes.Count == 0) {
            return [];
        }

        if (scopedTypes.Count > maxDiagramNodes) {
            diagnostics.Add(
                new AnalysisDiagnostic(
                    "MRD0001",
                    AnalysisDiagnosticSeverity.Info,
                    $"{title} was truncated to {maxDiagramNodes} types."));
        }

        return [
            CreateExport(
                ExportArtifactKind.MermaidClassDiagram,
                $"exports/class-diagrams/{scopeKey}.mmd",
                title,
                description,
                _classDiagramRenderer.Render(scopedTypes, FilterTypeRelationships(snapshot, scopedTypes), maxDiagramNodes)),
        ];
    }

    private IReadOnlyList<PreparedExport> CreateErDiagrams(
        ArchitectureSnapshot snapshot,
        int maxDiagramNodes,
        ICollection<AnalysisDiagnostic> diagnostics) {
        var exports = new List<PreparedExport>();

        foreach (var project in snapshot.Facts.Projects.OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase)) {
            exports.AddRange(
                CreateScopedErDiagramExports(
                    snapshot,
                    snapshot.Facts.Entities.Where(entity => string.Equals(entity.ProjectId, project.ProjectId, StringComparison.Ordinal)).ToArray(),
                    $"project-{Slugify(project.Name)}",
                    $"ER diagram - {project.Name}",
                    $"Entity relationships within {project.Name}.",
                    maxDiagramNodes,
                    diagnostics));
        }

        foreach (var module in snapshot.Facts.Modules.OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase)) {
            exports.AddRange(
                CreateScopedErDiagramExports(
                    snapshot,
                    snapshot.Facts.Entities.Where(entity => string.Equals(entity.ModuleId, module.ModuleId, StringComparison.Ordinal)).ToArray(),
                    $"module-{Slugify(module.Name)}",
                    $"ER diagram - {module.Name}",
                    $"Entity relationships within module {module.Name}.",
                    maxDiagramNodes,
                    diagnostics));
        }

        return exports;
    }

    private IReadOnlyList<PreparedExport> CreateScopedErDiagramExports(
        ArchitectureSnapshot snapshot,
        IReadOnlyList<CanDoItAll.CodeAnalytics.Domain.Facts.EntityFact> scopedEntities,
        string scopeKey,
        string title,
        string description,
        int maxDiagramNodes,
        ICollection<AnalysisDiagnostic> diagnostics) {
        if (scopedEntities.Count == 0) {
            return [];
        }

        if (scopedEntities.Count > maxDiagramNodes) {
            diagnostics.Add(
                new AnalysisDiagnostic(
                    "MRD0002",
                    AnalysisDiagnosticSeverity.Info,
                    $"{title} was truncated to {maxDiagramNodes} entities."));
        }

        return [
            CreateExport(
                ExportArtifactKind.MermaidErDiagram,
                $"exports/er-diagrams/{scopeKey}.mmd",
                title,
                description,
                _erDiagramRenderer.Render(scopedEntities, FilterEntityRelationships(snapshot, scopedEntities), maxDiagramNodes)),
        ];
    }

    private static IReadOnlyList<CanDoItAll.CodeAnalytics.Domain.Facts.TypeRelationshipFact> FilterTypeRelationships(
        ArchitectureSnapshot snapshot,
        IReadOnlyList<CanDoItAll.CodeAnalytics.Domain.Facts.TypeFact> scopedTypes) {
        var typeIds = scopedTypes.Select(type => type.TypeId).ToHashSet(StringComparer.Ordinal);
        return snapshot.Facts.TypeRelationships
            .Where(item => typeIds.Contains(item.FromTypeId) && typeIds.Contains(item.ToTypeId))
            .ToArray();
    }

    private static IReadOnlyList<CanDoItAll.CodeAnalytics.Domain.Facts.EntityRelationshipFact> FilterEntityRelationships(
        ArchitectureSnapshot snapshot,
        IReadOnlyList<CanDoItAll.CodeAnalytics.Domain.Facts.EntityFact> scopedEntities) {
        var entityIds = scopedEntities.Select(entity => entity.EntityId).ToHashSet(StringComparer.Ordinal);
        return snapshot.Facts.EntityRelationships
            .Where(item => entityIds.Contains(item.FromEntityId) && entityIds.Contains(item.ToEntityId))
            .ToArray();
    }

    private static bool IsDefaultClassDiagramCandidate(
        CanDoItAll.CodeAnalytics.Domain.Facts.TypeFact type,
        ArchitectureSnapshot snapshot) {
        var projectName = snapshot.Facts.Projects
            .FirstOrDefault(project => string.Equals(project.ProjectId, type.ProjectId, StringComparison.Ordinal))
            ?.Name;
        if (!string.IsNullOrWhiteSpace(projectName) &&
            (projectName.Contains(".Tests", StringComparison.OrdinalIgnoreCase) ||
                projectName.EndsWith("Tests", StringComparison.OrdinalIgnoreCase))) {
            return false;
        }

        var path = type.Source.Path;
        return !path.Contains("/migrations/", StringComparison.OrdinalIgnoreCase)
            && !path.Contains("/obj/", StringComparison.OrdinalIgnoreCase)
            && !path.EndsWith(".g.cs", StringComparison.OrdinalIgnoreCase)
            && !path.Contains("AutoGeneratedProgram", StringComparison.Ordinal);
    }

    private static string Slugify(string value) {
        var characters = value
            .Select(character => char.IsLetterOrDigit(character) ? char.ToLowerInvariant(character) : '-')
            .ToArray();
        var slug = new string(characters).Trim('-');
        while (slug.Contains("--", StringComparison.Ordinal)) {
            slug = slug.Replace("--", "-", StringComparison.Ordinal);
        }

        return string.IsNullOrWhiteSpace(slug)
            ? "scope"
            : slug;
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
