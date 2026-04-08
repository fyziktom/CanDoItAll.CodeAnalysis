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
        int depth) {
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
                CollectMemberCandidate(candidates, relationship.FromMemberId, relationship.ToMemberId, relationship.Kind);
                CollectMemberCandidate(candidates, relationship.ToMemberId, relationship.FromMemberId, relationship.Kind);
            }

            if (candidates.Count == 0) {
                break;
            }

            var next = candidates
                .Select(item => new ScoredMemberCandidate(membersById[item.Key], item.Value))
                .OrderByDescending(item => item.Score)
                .ThenBy(item => item.Member.DisplayName, StringComparer.Ordinal)
                .Take(remaining)
                .Select(item => item.Member.MemberId)
                .ToHashSet(StringComparer.Ordinal);

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

            var score = ScoreMemberCandidate(candidateMember, candidateType, seedTypeIds, relationshipKind);
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
        MemberRelationshipKind relationshipKind) {
        var score = 0;
        if (seedTypeIds.Contains(type.TypeId)) {
            score += 50;
        }

        score += member.Kind switch {
            MemberKind.Method => 18,
            MemberKind.Constructor => 16,
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

        return score;
    }

    private sealed record ScoredMemberCandidate(MemberFact Member, int Score);
}
