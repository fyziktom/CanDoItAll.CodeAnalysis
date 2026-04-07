using System.Text;
using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Rendering.Mermaid;

public sealed class ProjectGraphMermaidRenderer {
    public string Render(IReadOnlyList<ProjectFact> projects, IReadOnlyList<DependencyEdgeFact> dependencies) {
        var builder = new StringBuilder();
        builder.AppendLine("flowchart LR");

        foreach (var project in projects.OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase)) {
            builder.AppendLine($"    {project.ProjectId}[\"{project.Name}\"]");
        }

        foreach (var edge in dependencies
            .Where(edge => edge.Kind == DependencyKind.ProjectReference)
            .OrderBy(edge => edge.FromId, StringComparer.Ordinal)
            .ThenBy(edge => edge.ToId, StringComparer.Ordinal)) {
            builder.AppendLine($"    {edge.FromId} --> {edge.ToId}");
        }

        return builder.ToString().TrimEnd() + Environment.NewLine;
    }
}
