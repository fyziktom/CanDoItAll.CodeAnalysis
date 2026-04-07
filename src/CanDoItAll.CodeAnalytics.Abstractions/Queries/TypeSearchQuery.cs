namespace CanDoItAll.CodeAnalytics.Abstractions.Queries;

public sealed record TypeSearchQuery(
    string SnapshotId,
    string? SearchText = null,
    string? ProjectName = null,
    string? MemberSearchText = null,
    bool IncludeMembers = false,
    bool MethodsOnly = false);
