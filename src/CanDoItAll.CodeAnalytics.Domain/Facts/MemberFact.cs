using CanDoItAll.CodeAnalytics.Domain.Sources;

namespace CanDoItAll.CodeAnalytics.Domain.Facts;

public sealed record MemberFact(
    string MemberId,
    string TypeId,
    string DisplayName,
    MemberKind Kind,
    string ReturnTypeDisplayName,
    IReadOnlyList<string> ParameterDisplayNames,
    SourceReference Source);
