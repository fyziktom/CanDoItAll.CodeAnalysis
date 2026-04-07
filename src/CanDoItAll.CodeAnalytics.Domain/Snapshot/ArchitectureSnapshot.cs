using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Exports;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Insights;

namespace CanDoItAll.CodeAnalytics.Domain.Snapshot;

public sealed record ArchitectureSnapshot(
    string SchemaVersion,
    string GeneratorVersion,
    string SnapshotId,
    DateTimeOffset CreatedUtc,
    AnalysisRequest Request,
    ArchitectureFacts Facts,
    ArchitectureInsights Insights,
    ArchitectureExports Exports,
    IReadOnlyList<AnalysisDiagnostic> Diagnostics);
