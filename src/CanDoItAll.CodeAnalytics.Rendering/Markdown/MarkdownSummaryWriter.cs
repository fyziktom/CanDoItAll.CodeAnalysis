using System.Text;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;

namespace CanDoItAll.CodeAnalytics.Rendering.Markdown;

public sealed class MarkdownSummaryWriter {
    public string Write(ArchitectureSnapshot snapshot) {
        var builder = new StringBuilder();
        builder.AppendLine("# Architecture Summary");
        builder.AppendLine();
        builder.AppendLine($"- Solution: `{snapshot.Facts.Solution.Name}`");
        builder.AppendLine($"- Snapshot ID: `{snapshot.SnapshotId}`");
        builder.AppendLine($"- Created UTC: `{snapshot.CreatedUtc:O}`");
        builder.AppendLine($"- Projects: `{snapshot.Insights.Summary.ProjectCount}`");
        builder.AppendLine($"- Types: `{snapshot.Insights.Summary.TypeCount}`");
        builder.AppendLine($"- Members: `{snapshot.Insights.Summary.MemberCount}`");
        builder.AppendLine($"- Services: `{snapshot.Insights.Summary.ServiceRegistrationCount}`");
        builder.AppendLine($"- Entities: `{snapshot.Insights.Summary.EntityCount}`");
        builder.AppendLine($"- Findings: `{snapshot.Insights.Summary.FindingCount}`");
        builder.AppendLine();
        builder.AppendLine("## Top Findings");
        builder.AppendLine();

        if (snapshot.Insights.Findings.Count == 0) {
            builder.AppendLine("No findings were produced.");
            builder.AppendLine();
        }
        else {
            foreach (var finding in snapshot.Insights.Findings.Take(10)) {
                builder.AppendLine($"- `{finding.RuleId}` {finding.Title}: {finding.Description}");
            }

            builder.AppendLine();
        }

        builder.AppendLine("## Diagnostics");
        builder.AppendLine();
        if (snapshot.Diagnostics.Count == 0) {
            builder.AppendLine("No diagnostics were produced.");
            builder.AppendLine();
        }
        else {
            foreach (var diagnostic in snapshot.Diagnostics.Take(10)) {
                builder.AppendLine($"- `{diagnostic.Code}` {diagnostic.Message}");
            }

            builder.AppendLine();
        }

        builder.AppendLine("## Modules");
        builder.AppendLine();
        foreach (var module in snapshot.Facts.Modules) {
            builder.AppendLine($"- `{module.Name}` ({module.TypeIds.Count} types)");
        }

        builder.AppendLine();
        builder.AppendLine("## Persistence");
        builder.AppendLine();
        foreach (var dbContext in snapshot.Facts.DbContexts) {
            builder.AppendLine($"- `{dbContext.DisplayName}` -> {dbContext.EntityTypeIds.Count} entities");
        }

        return builder.ToString().TrimEnd() + Environment.NewLine;
    }
}
