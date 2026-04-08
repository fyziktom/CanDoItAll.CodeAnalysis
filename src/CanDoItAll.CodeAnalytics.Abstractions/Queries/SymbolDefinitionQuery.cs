namespace CanDoItAll.CodeAnalytics.Abstractions.Queries;

public sealed record SymbolDefinitionQuery(
    string SnapshotId,
    string TypeId,
    string? MemberId = null);
