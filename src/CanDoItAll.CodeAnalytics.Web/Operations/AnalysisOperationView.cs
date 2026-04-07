using CanDoItAll.CodeAnalytics.Abstractions.Progress;
using CanDoItAll.CodeAnalytics.Domain.Diagnostics;

namespace CanDoItAll.CodeAnalytics.Web.Operations;

public sealed record AnalysisOperationView(
    string OperationId,
    string WorkspacePath,
    IReadOnlyList<string> ScopeProjectNames,
    AnalysisOperationStatus Status,
    DateTimeOffset CreatedUtc,
    DateTimeOffset? StartedUtc,
    DateTimeOffset? CompletedUtc,
    string? SnapshotId,
    bool FromCache,
    IReadOnlyList<AnalysisProgressEvent> ProgressEvents,
    IReadOnlyList<AnalysisDiagnostic> Diagnostics,
    string? ErrorMessage);
