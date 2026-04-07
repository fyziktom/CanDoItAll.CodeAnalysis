namespace CanDoItAll.CodeAnalytics.Domain.Facts;

public sealed record TypeRelationshipFact(
    string RelationshipId,
    string FromTypeId,
    string ToTypeId,
    TypeRelationshipKind Kind,
    int Weight);
