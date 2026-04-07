using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;

namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record SnapshotBuildResponse(
    ArchitectureSnapshot Snapshot,
    bool FromCache,
    IReadOnlyList<AnalysisDiagnostic> Diagnostics);
