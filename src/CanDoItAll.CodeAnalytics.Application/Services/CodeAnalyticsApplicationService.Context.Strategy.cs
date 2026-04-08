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
        IReadOnlyDictionary<string, ProjectFact> projectsById) {
        var requestedIntent = query.Intent;
        var requestedPrecision = query.Precision;
        var helperAnalysis = AnalyzeHelperSeed(query, seedType, seedMember, seedMemberIds, relationships, membersById, typesById, projectsById);
        var resolvedIntent = requestedIntent != FocusedContextIntent.Auto
            ? requestedIntent
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
            || helperAnalysis.IsHighFanInHelper && resolvedIntent == FocusedContextIntent.Definition);
        var strategyExplanation = BuildStrategyExplanation(
            requestedIntent,
            requestedPrecision,
            resolvedIntent,
            resolvedPrecision,
            helperAnalysis,
            seedType,
            effectiveDepth,
            query.Depth);
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
        IReadOnlyCollection<string> focusTags) {
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
                focusTags)
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
            representativeConsumers,
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
        IReadOnlyList<MemberFact> representativeConsumers,
        FocusedContextStrategy strategy) {
        var implementationTypeIds = implementationTypes
            .Select(item => item.TypeId)
            .ToHashSet(StringComparer.Ordinal);
        var representativeConsumerIds = representativeConsumers
            .Select(item => item.MemberId)
            .ToHashSet(StringComparer.Ordinal);
        return selectedMemberIds
            .Where(membersById.ContainsKey)
            .Select(memberId => membersById[memberId])
            .OrderBy(item => GetSelectedMemberBucket(item, seedType, seedMember, implementationTypeIds, representativeConsumerIds, strategy))
            .ThenByDescending(item => GetSelectedMemberScore(item, seedType, focusTags, implementationTypeIds, representativeConsumerIds))
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
        ISet<string> representativeConsumerIds) {
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

    private static HelperSeedAnalysis AnalyzeHelperSeed(
        FocusedContextQuery query,
        TypeFact? seedType,
        MemberFact? seedMember,
        IReadOnlyList<string> seedMemberIds,
        IReadOnlyList<MemberRelationshipFact> relationships,
        IReadOnlyDictionary<string, MemberFact> membersById,
        IReadOnlyDictionary<string, TypeFact> typesById,
        IReadOnlyDictionary<string, ProjectFact> projectsById) {
        if (seedType is null || seedMemberIds.Count == 0) {
            return new HelperSeedAnalysis(false, 0, 0, 0);
        }

        var explicitSymbolSeed = !string.IsNullOrWhiteSpace(query.TypeId)
            || !string.IsNullOrWhiteSpace(query.MemberId)
            || query.QueryText is not null && IsTypeIdentityQuery(query.QueryText, seedType)
            || query.QueryText is not null && seedMember is not null && MemberNameMatchesQuery(seedMember, query.QueryText);
        if (!explicitSymbolSeed && query.Intent == FocusedContextIntent.Auto) {
            return new HelperSeedAnalysis(false, 0, 0, 0);
        }

        var targetMemberIds = seedMemberIds.ToHashSet(StringComparer.Ordinal);
        var callerMembers = relationships
            .Where(item => targetMemberIds.Contains(item.ToMemberId))
            .Select(item => membersById.TryGetValue(item.FromMemberId, out var member) ? member : null)
            .OfType<MemberFact>()
            .Where(item => typesById.TryGetValue(item.TypeId, out var type) && !ShouldExcludeFromFocusedContext(type, projectsById, seedType))
            .ToArray();
        var callerCount = callerMembers
            .Select(item => item.MemberId)
            .Distinct(StringComparer.Ordinal)
            .Count();
        var callerTypeCount = callerMembers
            .Select(item => item.TypeId)
            .Distinct(StringComparer.Ordinal)
            .Count();
        var callerProjectCount = callerMembers
            .Select(item => typesById[item.TypeId].ProjectId)
            .Distinct(StringComparer.Ordinal)
            .Count();
        var isHighFanInHelper = callerCount >= HighFanInCallerThreshold
            && (callerTypeCount >= HighFanInCallerTypeThreshold || callerProjectCount >= HighFanInCallerProjectThreshold);
        return new HelperSeedAnalysis(isHighFanInHelper, callerCount, callerTypeCount, callerProjectCount);
    }

    private static string BuildStrategyExplanation(
        FocusedContextIntent requestedIntent,
        FocusedContextPrecision requestedPrecision,
        FocusedContextIntent resolvedIntent,
        FocusedContextPrecision resolvedPrecision,
        HelperSeedAnalysis helperAnalysis,
        TypeFact? seedType,
        int effectiveDepth,
        int requestedDepth) {
        var depthNote = effectiveDepth < requestedDepth
            ? " Consumer expansion is capped to direct usages."
            : string.Empty;
        if (helperAnalysis.IsHighFanInHelper && seedType is not null && requestedIntent == FocusedContextIntent.Auto && requestedPrecision == FocusedContextPrecision.Auto) {
            return $"Auto resolved to {NormalizeSearchToken(resolvedPrecision.ToString())} {NormalizeIntentText(resolvedIntent)} mode because {seedType.DisplayName} spans {helperAnalysis.IncomingCallerCount} callers across {helperAnalysis.CallerProjectCount} projects.{depthNote}";
        }

        if (requestedIntent != FocusedContextIntent.Auto || requestedPrecision != FocusedContextPrecision.Auto) {
            return $"Used requested {NormalizeIntentText(resolvedIntent)} mode with {NormalizeSearchToken(resolvedPrecision.ToString())} precision.{depthNote}";
        }

        return "Used default trouble-path expansion.";
    }

    private static IReadOnlyList<RepresentativeConsumerCluster> CreateRepresentativeConsumerClusters(
        IReadOnlyCollection<string> targetMemberIds,
        IReadOnlyList<MemberRelationshipFact> relationships,
        IReadOnlyDictionary<string, MemberFact> membersById,
        IReadOnlyDictionary<string, TypeFact> typesById,
        IReadOnlyDictionary<string, ProjectFact> projectsById,
        IReadOnlyDictionary<string, ModuleFact> modulesById,
        TypeFact? seedType,
        IReadOnlyList<TypeFact> implementationTypes,
        IReadOnlyCollection<string> focusTags) {
        if (targetMemberIds.Count == 0) {
            return [];
        }

        var excludedTypeIds = implementationTypes
            .Select(item => item.TypeId)
            .ToHashSet(StringComparer.Ordinal);
        if (seedType is not null) {
            excludedTypeIds.Add(seedType.TypeId);
        }

        var candidateByMemberId = new Dictionary<string, RepresentativeConsumerCandidate>(StringComparer.Ordinal);
        foreach (var relationship in relationships.Where(item => targetMemberIds.Contains(item.ToMemberId))) {
            if (!membersById.TryGetValue(relationship.FromMemberId, out var callerMember)) {
                continue;
            }

            if (!typesById.TryGetValue(callerMember.TypeId, out var callerType)) {
                continue;
            }

            if (excludedTypeIds.Contains(callerType.TypeId) || ShouldExcludeFromFocusedContext(callerType, projectsById, seedType)) {
                continue;
            }

            var score = ScoreRepresentativeConsumer(callerMember, callerType, relationship, seedType, focusTags);
            var candidate = new RepresentativeConsumerCandidate(callerMember, callerType, relationship, score);
            if (candidateByMemberId.TryGetValue(callerMember.MemberId, out var existingCandidate)) {
                if (score > existingCandidate.Score) {
                    candidateByMemberId[callerMember.MemberId] = candidate;
                }
            }
            else {
                candidateByMemberId.Add(callerMember.MemberId, candidate);
            }
        }

        return candidateByMemberId.Values
            .GroupBy(item => new UsageClusterKey(item.Type.ProjectId, item.Type.ModuleId))
            .Select(
                group => {
                    var orderedCandidates = group
                        .OrderByDescending(item => item.Score)
                        .ThenBy(item => item.Member.DisplayName, StringComparer.Ordinal)
                        .ToArray();
                    var projectName = projectsById.TryGetValue(group.Key.ProjectId, out var project)
                        ? project.Name
                        : group.Key.ProjectId;
                    var moduleName = !string.IsNullOrWhiteSpace(group.Key.ModuleId) && modulesById.TryGetValue(group.Key.ModuleId, out var module)
                        ? module.Name
                        : null;
                    var callerCount = orderedCandidates.Length;
                    var clusterScore = callerCount * 100 + orderedCandidates.Max(item => item.Score);
                    return new RepresentativeConsumerCluster(
                        group.Key.ProjectId,
                        projectName,
                        group.Key.ModuleId,
                        moduleName,
                        callerCount,
                        clusterScore,
                        orderedCandidates);
                })
            .OrderByDescending(item => item.Score)
            .ThenBy(item => item.ProjectName, StringComparer.Ordinal)
            .ThenBy(item => item.ModuleName ?? string.Empty, StringComparer.Ordinal)
            .ToArray();
    }

    private static IReadOnlyList<MemberFact> SelectRepresentativeConsumerMembers(
        IReadOnlyList<RepresentativeConsumerCluster> clusters,
        int maxClusters,
        int maxSamplesPerCluster) {
        if (maxClusters <= 0 || maxSamplesPerCluster <= 0 || clusters.Count == 0) {
            return [];
        }

        return clusters
            .Take(maxClusters)
            .SelectMany(item => item.Candidates.Take(maxSamplesPerCluster).Select(candidate => candidate.Member))
            .GroupBy(item => item.MemberId, StringComparer.Ordinal)
            .Select(group => group.First())
            .ToArray();
    }

    private static FocusedContextUsageSummary? BuildUsageSummary(
        IReadOnlyList<RepresentativeConsumerCluster> clusters,
        int maxClusters,
        int maxSamplesPerCluster) {
        if (maxClusters <= 0 || clusters.Count == 0) {
            return null;
        }

        var selectedClusters = clusters
            .Take(maxClusters)
            .Select(
                cluster => new FocusedContextUsageCluster(
                    cluster.ProjectId,
                    cluster.ProjectName,
                    cluster.ModuleId,
                    cluster.ModuleName,
                    cluster.CallerCount,
                    cluster.Candidates
                        .Take(maxSamplesPerCluster)
                        .Select(CreateUsageSample)
                        .ToArray()))
            .ToArray();
        var totalCallerCount = clusters.Sum(item => item.CallerCount);
        var displayedCallerCount = selectedClusters.Sum(item => item.CallerCount);
        return new FocusedContextUsageSummary(
            totalCallerCount,
            clusters.Count,
            Math.Max(0, totalCallerCount - displayedCallerCount),
            selectedClusters);
    }

    private static FocusedContextUsageSample CreateUsageSample(RepresentativeConsumerCandidate candidate) {
        return new FocusedContextUsageSample(
            candidate.Type.TypeId,
            candidate.Type.DisplayName,
            candidate.Member.MemberId,
            candidate.Member.DisplayName,
            candidate.Member.Source.Path,
            candidate.Member.Source.Line,
            $"{candidate.Relationship.Kind} usage sample.");
    }

    private static int ScoreRepresentativeConsumer(
        MemberFact member,
        TypeFact type,
        MemberRelationshipFact relationship,
        TypeFact? seedType,
        IReadOnlyCollection<string> focusTags) {
        var score = relationship.Kind switch {
            MemberRelationshipKind.Invocation => 30,
            MemberRelationshipKind.PropertyAccess => 22,
            MemberRelationshipKind.FieldAccess => 16,
            MemberRelationshipKind.ObjectCreation => 14,
            _ => 0,
        };
        score += member.Kind switch {
            MemberKind.Method => 16,
            MemberKind.Property => 8,
            _ => 0,
        };
        if (seedType is not null && string.Equals(type.ProjectId, seedType.ProjectId, StringComparison.Ordinal)) {
            score += 10;
        }

        if (seedType is not null && string.Equals(type.ModuleId, seedType.ModuleId, StringComparison.Ordinal)) {
            score += 6;
        }

        score += GetFocusTagScore(
            focusTags,
            member.DisplayName,
            member.ReturnTypeDisplayName,
            type.DisplayName,
            type.Source.Path);
        return score;
    }

    private static string NormalizeIntentText(FocusedContextIntent intent) {
        return intent switch {
            FocusedContextIntent.TroublePath => "trouble-path",
            FocusedContextIntent.UsageSummary => "usage-summary",
            FocusedContextIntent.RepresentativeConsumers => "representative-consumers",
            _ => NormalizeSearchToken(intent.ToString()),
        };
    }

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
        string StrategyExplanation);

    private sealed record FocusedContextMemberSelectionResult(
        IReadOnlyList<string> SelectedMemberIds,
        IReadOnlyList<TypeFact> ImplementationTypes,
        IReadOnlyList<MemberFact> ImplementationMembers,
        IReadOnlyList<MemberFact> RepresentativeConsumers,
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
