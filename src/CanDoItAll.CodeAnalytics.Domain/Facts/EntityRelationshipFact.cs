using CanDoItAll.CodeAnalytics.Domain.Sources;

namespace CanDoItAll.CodeAnalytics.Domain.Facts;

public sealed record EntityRelationshipFact(
    string RelationshipId,
    string FromEntityId,
    string ToEntityId,
    EntityRelationshipKind Kind,
    IReadOnlyList<string> NavigationPropertyNames,
    SourceReference? Source);
