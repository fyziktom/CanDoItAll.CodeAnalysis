namespace CanDoItAll.CodeAnalytics.Abstractions.Queries;

public sealed record FocusedContextQuery(
    string SnapshotId,
    string? TypeId = null,
    string? MemberId = null,
    string? ServiceRegistrationId = null,
    int Depth = 2);
