namespace CanDoItAll.CodeAnalytics.Abstractions.Queries;

public sealed record SymbolImplementationsQuery(
    string SnapshotId,
    string TypeId);
