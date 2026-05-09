using CanDoItAll.CodeAnalytics.Abstractions;
using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record FocusedContextResponse(
    string SnapshotId,
    int Depth,
    string? QueryText,
    IReadOnlyList<string> FocusTags,
    IReadOnlyList<string> RelationHints,
    FocusedContextIntent RequestedIntent,
    FocusedContextIntent ResolvedIntent,
    FocusedContextPrecision RequestedPrecision,
    FocusedContextPrecision ResolvedPrecision,
    string? StrategyExplanation,
    string? SeedExplanation,
    TypeFact? SeedType,
    MemberFact? SeedMember,
    ServiceRegistrationFact? SeedService,
    IReadOnlyList<TypeFact> ImplementationTypes,
    IReadOnlyList<TypeFact> Types,
    IReadOnlyList<MemberFact> Members,
    IReadOnlyList<MemberRelationshipFact> MemberRelationships,
    IReadOnlyList<TypeRelationshipFact> TypeRelationships,
    IReadOnlyList<ServiceRegistrationFact> RelatedServices,
    IReadOnlyList<TypeFact> ReferenceTypes,
    FocusedContextUsageSummary? UsageSummary,
    IReadOnlyList<FocusedContextSelectionReason> SelectionReasons,
    FocusedContextStats Stats,
    IReadOnlyList<FocusedContextFileExcerpt> Files);
