using CanDoItAll.CodeAnalytics.Domain.Exports;

namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record ExportsViewResponse(
    string SnapshotId,
    IReadOnlyList<ExportArtifact> Artifacts);
