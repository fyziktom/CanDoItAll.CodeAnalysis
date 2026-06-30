using CanDoItAll.CodeAnalytics.Abstractions;
using CanDoItAll.CodeAnalytics.Abstractions.Queries;
using CanDoItAll.CodeAnalytics.Abstractions.Responses;
using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Application.Services;

public sealed partial class CodeAnalyticsApplicationService {
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
        int requestedDepth,
        IReadOnlyCollection<string> relationHints) {
        var depthNote = effectiveDepth < requestedDepth
            ? " Consumer expansion is capped to direct usages."
            : string.Empty;
        var relationNote = relationHints.Count > 0
            ? " Relation hints constrain representative usage sampling."
            : string.Empty;
        if (helperAnalysis.IsHighFanInHelper && seedType is not null && requestedIntent == FocusedContextIntent.Auto && requestedPrecision == FocusedContextPrecision.Auto) {
            return $"Auto resolved to {NormalizeSearchToken(resolvedPrecision.ToString())} {NormalizeIntentText(resolvedIntent)} mode because {seedType.DisplayName} spans {helperAnalysis.IncomingCallerCount} callers across {helperAnalysis.CallerProjectCount} projects.{depthNote}{relationNote}";
        }

        if (requestedIntent != FocusedContextIntent.Auto || requestedPrecision != FocusedContextPrecision.Auto) {
            return requestedIntent == FocusedContextIntent.Behavior
                ? $"Mapped legacy behavior mode to {NormalizeIntentText(resolvedIntent)} with {NormalizeSearchToken(resolvedPrecision.ToString())} precision.{depthNote}{relationNote}"
                : $"Used requested {NormalizeIntentText(resolvedIntent)} mode with {NormalizeSearchToken(resolvedPrecision.ToString())} precision.{depthNote}{relationNote}";
        }

        return relationHints.Count > 0
            ? "Used default trouble-path expansion. Relation hints bias related member and usage selection."
            : "Used default trouble-path expansion.";
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
        IReadOnlyCollection<string> focusTags,
        IReadOnlyCollection<string> relationHints) {
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

            var projectName = projectsById.TryGetValue(callerType.ProjectId, out var project)
                ? project.Name
                : callerType.ProjectId;
            var moduleName = !string.IsNullOrWhiteSpace(callerType.ModuleId) && modulesById.TryGetValue(callerType.ModuleId, out var module)
                ? module.Name
                : null;
            var relationScore = GetRelationHintScore(
                relationHints,
                callerMember.DisplayName,
                callerMember.ReturnTypeDisplayName,
                string.Join(' ', callerMember.ParameterDisplayNames),
                callerType.DisplayName,
                callerType.Source.Path,
                projectName,
                moduleName);
            if (relationHints.Count > 0 && relationScore <= 0) {
                continue;
            }

            var score = ScoreRepresentativeConsumer(callerMember, callerType, relationship, seedType, focusTags) + relationScore;
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
        var roleKind = ClassifyReferenceRole(candidate.Member, candidate.Type, candidate.Relationship.Kind);
        return new FocusedContextUsageSample(
            candidate.Type.TypeId,
            candidate.Type.DisplayName,
            candidate.Member.MemberId,
            candidate.Member.DisplayName,
            candidate.Member.Source.Path,
            candidate.Member.Source.Line,
            roleKind == FocusedContextReferenceRoleKind.None
                ? $"{candidate.Relationship.Kind} usage sample."
                : $"{roleKind} sample.");
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

        score += GetRoleScoreBonus(ClassifyReferenceRole(member, type, relationship.Kind));
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
            FocusedContextIntent.Behavior => "behavior",
            FocusedContextIntent.TroublePath => "trouble-path",
            FocusedContextIntent.UsageSummary => "usage-summary",
            FocusedContextIntent.RepresentativeConsumers => "representative-consumers",
            _ => NormalizeSearchToken(intent.ToString()),
        };
    }
}
