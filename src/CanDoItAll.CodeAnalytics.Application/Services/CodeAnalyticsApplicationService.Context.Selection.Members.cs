using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Application.Services;

public sealed partial class CodeAnalyticsApplicationService {
    private static HashSet<string> ExpandMemberNeighborhood(
        IReadOnlyList<string> seedMemberIds,
        IReadOnlyList<MemberRelationshipFact> relationships,
        IReadOnlyDictionary<string, MemberFact> membersById,
        IReadOnlyDictionary<string, TypeFact> typesById,
        IReadOnlyDictionary<string, ProjectFact> projectsById,
        TypeFact? seedType,
        int depth,
        IReadOnlyCollection<string> focusTags,
        FocusedContextTraversalMode traversalMode) {
        var selected = seedMemberIds.ToHashSet(StringComparer.Ordinal);
        var frontier = seedMemberIds.ToHashSet(StringComparer.Ordinal);
        var seedTypeIds = seedMemberIds
            .Select(memberId => membersById.TryGetValue(memberId, out var member) ? member.TypeId : null)
            .OfType<string>()
            .ToHashSet(StringComparer.Ordinal);

        if (seedType is not null) {
            seedTypeIds.Add(seedType.TypeId);
        }

        for (var currentDepth = 0; currentDepth < depth; currentDepth++) {
            var remaining = MaxFocusedMembers - selected.Count;
            if (remaining <= 0) {
                break;
            }

            var candidates = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var relationship in relationships) {
                if (traversalMode is FocusedContextTraversalMode.Bidirectional or FocusedContextTraversalMode.OutboundOnly) {
                    CollectMemberCandidate(candidates, relationship.FromMemberId, relationship.ToMemberId, relationship.Kind);
                }

                if (traversalMode is FocusedContextTraversalMode.Bidirectional or FocusedContextTraversalMode.InboundOnly) {
                    CollectMemberCandidate(candidates, relationship.ToMemberId, relationship.FromMemberId, relationship.Kind);
                }
            }

            if (candidates.Count == 0) {
                break;
            }

            var next = SelectNextMemberFrontier(candidates, membersById, typesById, seedType, remaining);

            foreach (var memberId in next) {
                selected.Add(memberId);
            }

            frontier = next;
            if (frontier.Count == 0) {
                break;
            }
        }

        return selected;

        void CollectMemberCandidate(
            IDictionary<string, int> candidates,
            string frontierMemberId,
            string candidateMemberId,
            MemberRelationshipKind relationshipKind) {
            if (!frontier.Contains(frontierMemberId) || selected.Contains(candidateMemberId)) {
                return;
            }

            if (!membersById.TryGetValue(candidateMemberId, out var candidateMember)) {
                return;
            }

            if (!typesById.TryGetValue(candidateMember.TypeId, out var candidateType)) {
                return;
            }

            if (ShouldExcludeFromFocusedContext(candidateType, projectsById, seedType)) {
                return;
            }

            var score = ScoreMemberCandidate(candidateMember, candidateType, seedTypeIds, relationshipKind, focusTags);
            if (candidates.TryGetValue(candidateMemberId, out var existingScore)) {
                if (score > existingScore) {
                    candidates[candidateMemberId] = score;
                }
            }
            else {
                candidates.Add(candidateMemberId, score);
            }
        }
    }

    private static IReadOnlyList<MemberRelationshipFact> SelectRelevantMemberRelationships(
        IReadOnlyList<MemberRelationshipFact> relationships,
        ISet<string> memberIdSet,
        IReadOnlyDictionary<string, MemberFact> membersById,
        IReadOnlyDictionary<string, TypeFact> typesById,
        IReadOnlyDictionary<string, ProjectFact> projectsById,
        TypeFact? seedType) {
        return relationships
            .Where(item => memberIdSet.Contains(item.FromMemberId) && memberIdSet.Contains(item.ToMemberId))
            .Where(item => MemberRelationshipIsVisible(item, membersById, typesById, projectsById, seedType))
            .OrderBy(item => item.Kind)
            .ThenBy(item => item.FromMemberId, StringComparer.Ordinal)
            .ThenBy(item => item.ToMemberId, StringComparer.Ordinal)
            .Take(MaxFocusedMemberRelationships)
            .ToArray();
    }

    private static bool MemberRelationshipIsVisible(
        MemberRelationshipFact relationship,
        IReadOnlyDictionary<string, MemberFact> membersById,
        IReadOnlyDictionary<string, TypeFact> typesById,
        IReadOnlyDictionary<string, ProjectFact> projectsById,
        TypeFact? seedType) {
        return TryResolveType(relationship.FromMemberId, membersById, typesById, out var fromType)
            && TryResolveType(relationship.ToMemberId, membersById, typesById, out var toType)
            && !ShouldExcludeFromFocusedContext(fromType, projectsById, seedType)
            && !ShouldExcludeFromFocusedContext(toType, projectsById, seedType);
    }

    private static bool TryResolveType(
        string memberId,
        IReadOnlyDictionary<string, MemberFact> membersById,
        IReadOnlyDictionary<string, TypeFact> typesById,
        out TypeFact type) {
        if (membersById.TryGetValue(memberId, out var member) && typesById.TryGetValue(member.TypeId, out type!)) {
            return true;
        }

        type = null!;
        return false;
    }

    private static int ScoreMemberCandidate(
        MemberFact member,
        TypeFact type,
        ISet<string> seedTypeIds,
        MemberRelationshipKind relationshipKind,
        IReadOnlyCollection<string> focusTags) {
        var score = 0;
        if (seedTypeIds.Contains(type.TypeId)) {
            score += 50;
        }

        score += member.Kind switch {
            MemberKind.Method => 18,
            MemberKind.Constructor => 8,
            MemberKind.Property => 10,
            MemberKind.Field => 8,
            _ => 0,
        };

        score += relationshipKind switch {
            MemberRelationshipKind.Invocation => 12,
            MemberRelationshipKind.ObjectCreation => 10,
            MemberRelationshipKind.PropertyAccess => 6,
            MemberRelationshipKind.FieldAccess => 4,
            _ => 0,
        };

        score += GetFocusTagScore(
            focusTags,
            member.DisplayName,
            member.ReturnTypeDisplayName,
            string.Join(' ', member.ParameterDisplayNames),
            type.DisplayName,
            type.Source.Path);
        return score;
    }

    private static HashSet<string> SelectNextMemberFrontier(
        IReadOnlyDictionary<string, int> candidates,
        IReadOnlyDictionary<string, MemberFact> membersById,
        IReadOnlyDictionary<string, TypeFact> typesById,
        TypeFact? seedType,
        int remaining) {
        var selected = new HashSet<string>(StringComparer.Ordinal);
        var selectedPerType = new Dictionary<string, int>(StringComparer.Ordinal);
        var selectedPerProject = new Dictionary<string, int>(StringComparer.Ordinal);
        var externalProjects = new HashSet<string>(StringComparer.Ordinal);
        var seedProjectId = seedType?.ProjectId;

        foreach (var candidate in candidates
                     .Select(item => new ScoredMemberCandidate(membersById[item.Key], item.Value))
                     .OrderByDescending(item => item.Score)
                     .ThenBy(item => item.Member.DisplayName, StringComparer.Ordinal)) {
            if (selected.Count >= remaining) {
                break;
            }

            if (!typesById.TryGetValue(candidate.Member.TypeId, out var type)) {
                continue;
            }

            var typeCount = selectedPerType.TryGetValue(type.TypeId, out var resolvedTypeCount)
                ? resolvedTypeCount
                : 0;
            if (typeCount >= MaxFrontierMembersPerType) {
                continue;
            }

            var isSeedProject = string.Equals(type.ProjectId, seedProjectId, StringComparison.Ordinal);
            var projectLimit = isSeedProject
                ? MaxFrontierMembersInSeedProject
                : MaxFrontierMembersPerExternalProject;
            var projectCount = selectedPerProject.TryGetValue(type.ProjectId, out var resolvedProjectCount)
                ? resolvedProjectCount
                : 0;
            if (projectCount >= projectLimit) {
                continue;
            }

            if (!isSeedProject && !externalProjects.Contains(type.ProjectId) && externalProjects.Count >= MaxFrontierExternalProjects) {
                continue;
            }

            selected.Add(candidate.Member.MemberId);
            selectedPerType[type.TypeId] = typeCount + 1;
            selectedPerProject[type.ProjectId] = projectCount + 1;
            if (!isSeedProject) {
                externalProjects.Add(type.ProjectId);
            }
        }

        return selected;
    }

    private sealed record ScoredMemberCandidate(MemberFact Member, int Score);
}
