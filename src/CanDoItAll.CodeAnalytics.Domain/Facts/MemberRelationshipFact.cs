using CanDoItAll.CodeAnalytics.Domain.Sources;

namespace CanDoItAll.CodeAnalytics.Domain.Facts;

public sealed record MemberRelationshipFact(
    string RelationshipId,
    string FromMemberId,
    string ToMemberId,
    MemberRelationshipKind Kind,
    int Weight,
    SourceReference? Source);
