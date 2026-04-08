namespace CanDoItAll.CodeAnalytics.Abstractions.Queries;

public sealed record SymbolMembersQuery(
    string SnapshotId,
    string TypeId);
