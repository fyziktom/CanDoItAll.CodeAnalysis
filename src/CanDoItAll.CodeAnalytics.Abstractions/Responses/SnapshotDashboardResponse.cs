using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Insights;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;

namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record SnapshotDashboardResponse(
    ArchitectureSnapshot Snapshot,
    IReadOnlyList<FindingInsight> TopFindings,
    IReadOnlyList<AnalysisDiagnostic> TopDiagnostics,
    IReadOnlyList<RecentSnapshotItem> RecentSnapshots);
