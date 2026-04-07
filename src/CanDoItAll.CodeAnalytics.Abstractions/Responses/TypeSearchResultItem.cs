using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record TypeSearchResultItem(
    string ProjectName,
    string ModuleName,
    string NamespaceName,
    TypeFact Type,
    IReadOnlyList<MemberFact> Members);
