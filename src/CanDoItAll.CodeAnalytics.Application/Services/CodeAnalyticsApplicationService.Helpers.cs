using System.Text;
using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Exports;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Insights;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Rendering.Exports;

namespace CanDoItAll.CodeAnalytics.Application.Services;

public sealed partial class CodeAnalyticsApplicationService {
    private static ArchitectureSnapshot CreateFallbackSnapshot(
        AnalysisRequest request,
        string snapshotId,
        ArchitectureFacts facts,
        IReadOnlyList<AnalysisDiagnostic> diagnostics) {
        return new ArchitectureSnapshot(
            SchemaVersion,
            "fallback",
            snapshotId,
            DateTimeOffset.UtcNow,
            request,
            facts,
            CreateEmptyInsights(diagnostics),
            ArchitectureExports.Empty,
            diagnostics);
    }

    private static ArchitectureFacts CreateEmptyFacts(AnalysisRequest request) {
        var solutionName = Path.GetFileNameWithoutExtension(request.SolutionPath);
        return new ArchitectureFacts(
            new SolutionFact(solutionName, request.SolutionPath, 0, 0),
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            []);
    }

    private static ArchitectureInsights CreateEmptyInsights(IReadOnlyList<AnalysisDiagnostic> diagnostics) {
        return new ArchitectureInsights(
            new RiskSummaryInsight(0, 0, 0, 0, 0, 0, 0, diagnostics.Count),
            [],
            [],
            [],
            []);
    }

    private static IReadOnlyList<AnalysisDiagnostic> SortDiagnostics(IReadOnlyList<AnalysisDiagnostic> diagnostics) {
        return diagnostics
            .OrderBy(diagnostic => diagnostic.Code, StringComparer.Ordinal)
            .ThenBy(diagnostic => diagnostic.Message, StringComparer.Ordinal)
            .ToArray();
    }

    private static ArchitectureExports CreateExports(IReadOnlyList<PreparedExport> exports) {
        var artifacts = exports
            .Select(
                export => new ExportArtifact(
                    export.Kind,
                    export.RelativePath,
                    export.Title,
                    export.Description,
                    Encoding.UTF8.GetByteCount(export.Content)))
            .OrderBy(artifact => artifact.RelativePath, StringComparer.Ordinal)
            .Append(
                new ExportArtifact(
                    ExportArtifactKind.SnapshotJson,
                    "snapshot.json",
                    "Snapshot JSON",
                    "Canonical architecture snapshot."))
            .ToArray();

        return new ArchitectureExports(artifacts);
    }
}
