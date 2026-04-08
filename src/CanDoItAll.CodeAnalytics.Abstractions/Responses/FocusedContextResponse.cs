using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record FocusedContextResponse(
    string SnapshotId,
    int Depth,
    TypeFact? SeedType,
    MemberFact? SeedMember,
    ServiceRegistrationFact? SeedService,
    IReadOnlyList<TypeFact> Types,
    IReadOnlyList<MemberFact> Members,
    IReadOnlyList<MemberRelationshipFact> MemberRelationships,
    IReadOnlyList<TypeRelationshipFact> TypeRelationships,
    IReadOnlyList<ServiceRegistrationFact> RelatedServices,
    IReadOnlyList<TypeFact> ReferenceTypes);
