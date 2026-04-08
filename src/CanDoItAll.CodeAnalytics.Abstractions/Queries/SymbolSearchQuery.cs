namespace CanDoItAll.CodeAnalytics.Abstractions.Queries;

public sealed record SymbolSearchQuery(
    string SnapshotId,
    string? SearchText = null,
    string? ProjectName = null,
    SymbolSearchMode SearchMode = SymbolSearchMode.Contains,
    bool IncludeTypes = true,
    bool IncludeMembers = true,
    int Take = 40);
