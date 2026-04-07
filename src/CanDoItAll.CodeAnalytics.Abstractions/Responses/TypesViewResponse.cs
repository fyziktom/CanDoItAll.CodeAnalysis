namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record TypesViewResponse(
    string SnapshotId,
    string? SearchText,
    string? ProjectName,
    string? MemberSearchText,
    bool IncludeMembers,
    bool MethodsOnly,
    IReadOnlyList<string> AvailableProjects,
    IReadOnlyList<TypeSearchResultItem> Types);
