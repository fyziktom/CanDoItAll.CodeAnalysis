using CanDoItAll.CodeAnalytics.Abstractions;
using CanDoItAll.CodeAnalytics.Abstractions.Responses;
using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Application.Services;

public sealed partial class CodeAnalyticsApplicationService {
    private sealed record FocusedContextStrategy(
        FocusedContextIntent RequestedIntent,
        FocusedContextIntent ResolvedIntent,
        FocusedContextPrecision RequestedPrecision,
        FocusedContextPrecision ResolvedPrecision,
        FocusedContextSeedProfile SeedProfile,
        FocusedContextTraversalMode TraversalMode,
        int EffectiveDepth,
        bool UseTargetedSelection,
        bool IncludeImplementationTypes,
        bool IncludeUsageSummary,
        bool IncludeRepresentativeConsumersInMembers,
        int RepresentativeConsumerClusterLimit,
        int RepresentativeConsumersPerCluster,
        int UsageSummaryClusterLimit,
        bool DisableReferenceTypes,
        bool EmitCodeExcerpts,
        string StrategyExplanation);

    private sealed record FocusedContextMemberSelectionResult(
        IReadOnlyList<string> SelectedMemberIds,
        IReadOnlyList<TypeFact> ImplementationTypes,
        IReadOnlyList<MemberFact> ImplementationMembers,
        IReadOnlyList<RepresentativeConsumerCandidate> RepresentativeConsumerCandidates,
        FocusedContextUsageSummary? UsageSummary);

    private sealed record HelperSeedAnalysis(
        bool IsHighFanInHelper,
        int IncomingCallerCount,
        int CallerTypeCount,
        int CallerProjectCount);

    private sealed record RepresentativeConsumerCandidate(
        MemberFact Member,
        TypeFact Type,
        MemberRelationshipFact Relationship,
        int Score);

    private sealed record RepresentativeConsumerCluster(
        string ProjectId,
        string ProjectName,
        string? ModuleId,
        string? ModuleName,
        int CallerCount,
        int Score,
        IReadOnlyList<RepresentativeConsumerCandidate> Candidates);

    private sealed record UsageClusterKey(string ProjectId, string? ModuleId);

    private enum FocusedContextSeedProfile {
        Standard,
        HighFanInHelper,
    }

    private enum FocusedContextTraversalMode {
        None,
        InboundOnly,
        OutboundOnly,
        Bidirectional,
    }
}
