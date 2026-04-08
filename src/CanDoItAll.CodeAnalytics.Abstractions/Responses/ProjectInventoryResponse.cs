namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record ProjectInventoryResponse(
    string SnapshotId,
    ProjectInventoryItem Project);
