namespace CanDoItAll.CodeAnalytics.Abstractions.Queries;

public sealed record SymbolReferencesQuery(
    string SnapshotId,
    string TypeId,
    string? MemberId = null,
    int Take = 40);
