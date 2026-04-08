namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record SymbolSearchResponse(
    string SnapshotId,
    string? SearchText,
    string? ProjectName,
    SymbolSearchMode SearchMode,
    IReadOnlyList<string> AvailableProjects,
    string? ValidationError,
    IReadOnlyList<SymbolSearchResultItem> Results);
