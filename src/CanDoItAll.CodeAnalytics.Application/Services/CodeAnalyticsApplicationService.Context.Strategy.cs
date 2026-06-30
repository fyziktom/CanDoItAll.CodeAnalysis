using CanDoItAll.CodeAnalytics.Abstractions;
using CanDoItAll.CodeAnalytics.Abstractions.Queries;
using CanDoItAll.CodeAnalytics.Abstractions.Responses;
using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Application.Services;

public sealed partial class CodeAnalyticsApplicationService {
    private const int HighFanInCallerThreshold = 6;
    private const int HighFanInCallerTypeThreshold = 4;
    private const int HighFanInCallerProjectThreshold = 2;
    private const int MaxImplementationTypes = 4;
    private const int MaxUsageSummaryClusters = 4;
    private const int MaxRepresentativeConsumerClusters = 3;
    private const int MaxRepresentativeConsumersPerCluster = 1;

    private static FocusedContextStrategy ResolveFocusedContextStrategy(
        FocusedContextQuery query,
        TypeFact? seedType,
        MemberFact? seedMember,
        IReadOnlyList<string> seedMemberIds,
        IReadOnlyList<MemberRelationshipFact> relationships,
        IReadOnlyDictionary<string, MemberFact> membersById,
        IReadOnlyDictionary<string, TypeFact> typesById,
        IReadOnlyDictionary<string, ProjectFact> projectsById,
        IReadOnlyCollection<string> relationHints) {
        var requestedIntent = query.Intent;
        var normalizedRequestedIntent = requestedIntent == FocusedContextIntent.Behavior
            ? FocusedContextIntent.TroublePath
            : requestedIntent;
        var requestedPrecision = query.Precision;
        var helperAnalysis = AnalyzeHelperSeed(query, seedType, seedMember, seedMemberIds, relationships, membersById, typesById, projectsById);
        var resolvedIntent = normalizedRequestedIntent != FocusedContextIntent.Auto
            ? normalizedRequestedIntent
            : helperAnalysis.IsHighFanInHelper
                ? FocusedContextIntent.Definition
                : FocusedContextIntent.TroublePath;
        var resolvedPrecision = requestedPrecision != FocusedContextPrecision.Auto
            ? requestedPrecision
            : helperAnalysis.IsHighFanInHelper
                ? FocusedContextPrecision.Surgical
                : FocusedContextPrecision.Balanced;
        var effectiveDepth = resolvedIntent == FocusedContextIntent.TroublePath
            ? Math.Clamp(query.Depth, 0, 5)
            : Math.Min(Math.Clamp(query.Depth, 0, 5), 1);
        var includeImplementationTypes = resolvedIntent is FocusedContextIntent.Definition
            or FocusedContextIntent.Implementations
            or FocusedContextIntent.UsageSummary
            or FocusedContextIntent.RepresentativeConsumers;
        var includeUsageSummary = effectiveDepth > 0 && (resolvedIntent is FocusedContextIntent.UsageSummary
            or FocusedContextIntent.RepresentativeConsumers
            || helperAnalysis.IsHighFanInHelper && resolvedIntent == FocusedContextIntent.Definition);
        var includeRepresentativeConsumersInMembers = effectiveDepth > 0 && (resolvedIntent == FocusedContextIntent.RepresentativeConsumers
            || helperAnalysis.IsHighFanInHelper
            && resolvedIntent == FocusedContextIntent.Definition
            && resolvedPrecision == FocusedContextPrecision.Balanced);
        var strategyExplanation = BuildStrategyExplanation(
            requestedIntent,
            requestedPrecision,
            resolvedIntent,
            resolvedPrecision,
            helperAnalysis,
            seedType,
            effectiveDepth,
            query.Depth,
            relationHints);
        return new FocusedContextStrategy(
            requestedIntent,
            resolvedIntent,
            requestedPrecision,
            resolvedPrecision,
            helperAnalysis.IsHighFanInHelper
                ? FocusedContextSeedProfile.HighFanInHelper
                : FocusedContextSeedProfile.Standard,
            resolvedIntent == FocusedContextIntent.TroublePath
                ? FocusedContextTraversalMode.Bidirectional
                : FocusedContextTraversalMode.None,
            effectiveDepth,
            resolvedIntent != FocusedContextIntent.TroublePath,
            includeImplementationTypes,
            includeUsageSummary,
            includeRepresentativeConsumersInMembers,
            includeRepresentativeConsumersInMembers
                ? MaxRepresentativeConsumerClusters
                : 0,
            MaxRepresentativeConsumersPerCluster,
            includeUsageSummary
                ? MaxUsageSummaryClusters
                : 0,
            resolvedIntent != FocusedContextIntent.TroublePath,
            resolvedPrecision != FocusedContextPrecision.Outline,
            strategyExplanation);
    }

    private static FocusedContextMemberSelectionResult SelectFocusedMembers(
        TypeFact? seedType,
        MemberFact? seedMember,
        IReadOnlyList<string> seedMemberIds,
        IReadOnlyList<TypeFact> allTypes,
        IReadOnlyList<MemberRelationshipFact> relationships,
        IReadOnlyDictionary<string, MemberFact> membersById,
        IReadOnlyDictionary<string, TypeFact> typesById,
        IReadOnlyDictionary<string, ProjectFact> projectsById,
        IReadOnlyDictionary<string, ModuleFact> modulesById,
        IReadOnlyDictionary<string, IReadOnlyList<MemberFact>> membersByTypeId,
        FocusedContextStrategy strategy,
        IReadOnlyCollection<string> focusTags,
        IReadOnlyCollection<string> relationHints) {
        if (!strategy.UseTargetedSelection) {
            var expandedMemberIds = ExpandMemberNeighborhood(
                seedMemberIds,
                relationships,
                membersById,
                typesById,
                projectsById,
                seedType,
                strategy.EffectiveDepth,
                focusTags,
                relationHints,
                strategy.TraversalMode);
            return new FocusedContextMemberSelectionResult(expandedMemberIds.ToArray(), [], [], [], null);
        }

        var implementationTypes = strategy.IncludeImplementationTypes
            ? FindImplementationTypes(seedType, allTypes, projectsById, focusTags)
            : [];
        var implementationMembers = strategy.IncludeImplementationTypes
            ? FindImplementationMembers(seedType, seedMemberIds, implementationTypes, membersById, membersByTypeId, focusTags)
            : [];
        var usageTargetMemberIds = seedMemberIds
            .Concat(implementationMembers.Select(item => item.MemberId))
            .Distinct(StringComparer.Ordinal)
            .ToHashSet(StringComparer.Ordinal);
        var representativeClusters = strategy.EffectiveDepth > 0 && (strategy.IncludeUsageSummary || strategy.IncludeRepresentativeConsumersInMembers)
            ? CreateRepresentativeConsumerClusters(
                usageTargetMemberIds,
                relationships,
                membersById,
                typesById,
                projectsById,
                modulesById,
                seedType,
                implementationTypes,
                focusTags,
                relationHints)
            : [];
        var representativeConsumers = strategy.IncludeRepresentativeConsumersInMembers
            ? SelectRepresentativeConsumerMembers(
                representativeClusters,
                strategy.RepresentativeConsumerClusterLimit,
                strategy.RepresentativeConsumersPerCluster)
            : [];
        var usageSummary = strategy.IncludeUsageSummary
            ? BuildUsageSummary(
                representativeClusters,
                strategy.UsageSummaryClusterLimit,
                strategy.RepresentativeConsumersPerCluster)
            : null;
        var selectedMemberIds = new List<string>(seedMemberIds.Count + implementationMembers.Count + representativeConsumers.Count);
        AddDistinctMemberIds(selectedMemberIds, seedMemberIds);
        AddDistinctMemberIds(selectedMemberIds, implementationMembers.Select(item => item.MemberId));
        AddDistinctMemberIds(selectedMemberIds, representativeConsumers.Select(item => item.MemberId));
        return new FocusedContextMemberSelectionResult(
            selectedMemberIds,
            implementationTypes,
            implementationMembers,
            representativeClusters
                .SelectMany(item => item.Candidates)
                .Where(item => representativeConsumers.Any(member => string.Equals(member.MemberId, item.Member.MemberId, StringComparison.Ordinal)))
                .ToArray(),
            usageSummary);

        static void AddDistinctMemberIds(ICollection<string> target, IEnumerable<string> source) {
            var seen = target.ToHashSet(StringComparer.Ordinal);
            foreach (var item in source) {
                if (!seen.Add(item)) {
                    continue;
                }

                target.Add(item);
            }
        }
    }

    private static IReadOnlyList<MemberFact> OrderSelectedMembers(
        IReadOnlyCollection<string> selectedMemberIds,
        IReadOnlyDictionary<string, MemberFact> membersById,
        TypeFact? seedType,
        MemberFact? seedMember,
        IReadOnlyCollection<string> focusTags,
        IReadOnlyList<TypeFact> implementationTypes,
        IReadOnlyList<RepresentativeConsumerCandidate> representativeConsumerCandidates,
        IReadOnlyDictionary<string, TypeFact> typesById,
        FocusedContextStrategy strategy) {
        var implementationTypeIds = implementationTypes
            .Select(item => item.TypeId)
            .ToHashSet(StringComparer.Ordinal);
        var representativeConsumerIds = representativeConsumerCandidates
            .Select(item => item.Member.MemberId)
            .ToHashSet(StringComparer.Ordinal);
        return selectedMemberIds
            .Where(membersById.ContainsKey)
            .Select(memberId => membersById[memberId])
            .OrderBy(item => GetSelectedMemberBucket(item, seedType, seedMember, implementationTypeIds, representativeConsumerIds, strategy))
            .ThenByDescending(item => GetSelectedMemberScore(item, seedType, focusTags, implementationTypeIds, representativeConsumerIds, typesById))
            .ThenBy(item => item.DisplayName, StringComparer.Ordinal)
            .Take(MaxFocusedMembers)
            .ToArray();
    }

    private static int GetSelectedMemberBucket(
        MemberFact member,
        TypeFact? seedType,
        MemberFact? seedMember,
        ISet<string> implementationTypeIds,
        ISet<string> representativeConsumerIds,
        FocusedContextStrategy strategy) {
        if (string.Equals(member.MemberId, seedMember?.MemberId, StringComparison.Ordinal)) {
            return 0;
        }

        if (seedType is not null && string.Equals(member.TypeId, seedType.TypeId, StringComparison.Ordinal)) {
            return 1;
        }

        if (implementationTypeIds.Contains(member.TypeId)) {
            return 2;
        }

        if (strategy.IncludeRepresentativeConsumersInMembers && representativeConsumerIds.Contains(member.MemberId)) {
            return 3;
        }

        return 4;
    }

    private static int GetSelectedMemberScore(
        MemberFact member,
        TypeFact? seedType,
        IReadOnlyCollection<string> focusTags,
        ISet<string> implementationTypeIds,
        ISet<string> representativeConsumerIds,
        IReadOnlyDictionary<string, TypeFact> typesById) {
        var score = GetFocusTagScore(
            focusTags,
            member.DisplayName,
            member.ReturnTypeDisplayName,
            string.Join(' ', member.ParameterDisplayNames));
        if (seedType is not null && string.Equals(member.TypeId, seedType.TypeId, StringComparison.Ordinal)) {
            score += 40;
        }

        if (implementationTypeIds.Contains(member.TypeId)) {
            score += 28;
        }

        if (representativeConsumerIds.Contains(member.MemberId)) {
            score += 16;
        }

        if (typesById.TryGetValue(member.TypeId, out var memberType)) {
            score += GetRoleScoreBonus(ClassifyReferenceRole(member, memberType, null));
        }

        return score;
    }

    private static IReadOnlyList<TypeFact> FindImplementationTypes(
        TypeFact? seedType,
        IReadOnlyList<TypeFact> allTypes,
        IReadOnlyDictionary<string, ProjectFact> projectsById,
        IReadOnlyCollection<string> focusTags) {
        if (seedType is null) {
            return [];
        }

        return allTypes
            .Where(item => !string.Equals(item.TypeId, seedType.TypeId, StringComparison.Ordinal))
            .Where(item => !ShouldExcludeFromFocusedContext(item, projectsById, seedType))
            .Where(item => TypeImplementsSeed(item, seedType))
            .OrderBy(item => string.Equals(item.ProjectId, seedType.ProjectId, StringComparison.Ordinal) ? 0 : 1)
            .ThenByDescending(item => GetFocusTagScore(focusTags, CreateFocusTagText(item)))
            .ThenBy(item => item.DisplayName, StringComparer.Ordinal)
            .Take(MaxImplementationTypes)
            .ToArray();
    }

    private static IReadOnlyList<MemberFact> FindImplementationMembers(
        TypeFact? seedType,
        IReadOnlyList<string> seedMemberIds,
        IReadOnlyList<TypeFact> implementationTypes,
        IReadOnlyDictionary<string, MemberFact> membersById,
        IReadOnlyDictionary<string, IReadOnlyList<MemberFact>> membersByTypeId,
        IReadOnlyCollection<string> focusTags) {
        if (implementationTypes.Count == 0) {
            return [];
        }

        var contractMembers = seedMemberIds
            .Where(membersById.ContainsKey)
            .Select(memberId => membersById[memberId])
            .ToArray();
        var matches = new List<MemberFact>();

        foreach (var implementationType in implementationTypes) {
            if (!membersByTypeId.TryGetValue(implementationType.TypeId, out var members)) {
                continue;
            }

            var matchedForType = false;
            foreach (var contractMember in contractMembers) {
                var match = members.FirstOrDefault(item => MembersShareContractShape(contractMember, item));
                if (match is null) {
                    continue;
                }

                matches.Add(match);
                matchedForType = true;
            }

            if (matchedForType || seedType is null) {
                continue;
            }

            var representativeMember = RankRepresentativeMembers(implementationType, members, focusTags).FirstOrDefault();
            if (representativeMember is not null) {
                matches.Add(representativeMember);
            }
        }

        return matches
            .GroupBy(item => item.MemberId, StringComparer.Ordinal)
            .Select(group => group.First())
            .ToArray();
    }

    private static bool MembersShareContractShape(MemberFact contractMember, MemberFact implementationMember) {
        if (contractMember.Kind != implementationMember.Kind) {
            return false;
        }

        if (!string.Equals(
                NormalizeSearchToken(GetTrailingIdentifier(contractMember.DisplayName)),
                NormalizeSearchToken(GetTrailingIdentifier(implementationMember.DisplayName)),
                StringComparison.Ordinal)) {
            return false;
        }

        return contractMember.Kind switch {
            MemberKind.Method => contractMember.ParameterDisplayNames.Count == implementationMember.ParameterDisplayNames.Count,
            MemberKind.Property => true,
            MemberKind.Field => true,
            _ => false,
        };
    }

    private static bool TypeImplementsSeed(TypeFact candidateType, TypeFact seedType) {
        return string.Equals(candidateType.BaseTypeDisplayName, seedType.DisplayName, StringComparison.Ordinal)
            || candidateType.InterfaceDisplayNames.Any(item => string.Equals(item, seedType.DisplayName, StringComparison.Ordinal));
    }

}
