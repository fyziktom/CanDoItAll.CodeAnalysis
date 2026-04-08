using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Application.Services;

public sealed partial class CodeAnalyticsApplicationService {
    private static IReadOnlyList<TypeFact> SelectRelevantTypes(
        TypeFact? seedType,
        IReadOnlyList<MemberFact> selectedMembers,
        IReadOnlyList<TypeRelationshipFact> relationships,
        IReadOnlyDictionary<string, TypeFact> typesById,
        IReadOnlyDictionary<string, ProjectFact> projectsById) {
        var selectedTypeIds = selectedMembers.Select(item => item.TypeId).ToHashSet(StringComparer.Ordinal);
        if (seedType is not null) {
            selectedTypeIds.Add(seedType.TypeId);
        }

        var anchorTypes = selectedTypeIds
            .Where(typesById.ContainsKey)
            .Select(typeId => typesById[typeId])
            .ToArray();
        var anchorProjectIds = anchorTypes.Select(item => item.ProjectId).ToHashSet(StringComparer.Ordinal);
        var anchorModuleIds = anchorTypes.Select(item => item.ModuleId).ToHashSet(StringComparer.Ordinal);
        var anchorPaths = selectedMembers.Select(item => item.Source.Path).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (seedType is not null) {
            anchorPaths.Add(seedType.Source.Path);
        }

        var candidates = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var relationship in relationships) {
            CollectTypeCandidate(relationship.FromTypeId, relationship.ToTypeId, relationship);
            CollectTypeCandidate(relationship.ToTypeId, relationship.FromTypeId, relationship);
        }

        var selectedTypes = anchorTypes
            .OrderBy(item => string.Equals(item.TypeId, seedType?.TypeId, StringComparison.Ordinal) ? 0 : 1)
            .ThenBy(item => item.DisplayName, StringComparer.Ordinal)
            .ToList();
        if (selectedTypes.Count >= MaxFocusedTypes) {
            return selectedTypes.Take(MaxFocusedTypes).ToArray();
        }

        var externalCountsByProject = new Dictionary<string, int>(StringComparer.Ordinal);
        var externalCount = 0;
        foreach (var candidate in candidates
                     .Select(item => new ScoredTypeCandidate(typesById[item.Key], item.Value))
                     .OrderByDescending(item => item.Score)
                     .ThenBy(item => item.Type.DisplayName, StringComparer.Ordinal)) {
            if (selectedTypes.Count >= MaxFocusedTypes) {
                break;
            }

            var isAnchorProject = anchorProjectIds.Contains(candidate.Type.ProjectId);
            if (!isAnchorProject) {
                if (externalCount >= MaxExternalFocusedTypes) {
                    continue;
                }

                var currentCount = externalCountsByProject.TryGetValue(candidate.Type.ProjectId, out var value) ? value : 0;
                if (currentCount >= MaxExternalTypesPerProject) {
                    continue;
                }

                externalCountsByProject[candidate.Type.ProjectId] = currentCount + 1;
                externalCount++;
            }

            selectedTypes.Add(candidate.Type);
        }

        return selectedTypes.ToArray();

        void CollectTypeCandidate(string anchorTypeId, string candidateTypeId, TypeRelationshipFact relationship) {
            if (!selectedTypeIds.Contains(anchorTypeId) || selectedTypeIds.Contains(candidateTypeId)) {
                return;
            }

            if (!typesById.TryGetValue(candidateTypeId, out var candidateType)) {
                return;
            }

            if (ShouldExcludeFromFocusedContext(candidateType, projectsById, seedType)) {
                return;
            }

            var isAnchorProject = anchorProjectIds.Contains(candidateType.ProjectId);
            if (!isAnchorProject && (relationship.Source is null || !anchorPaths.Contains(relationship.Source.Path))) {
                return;
            }

            var score = ScoreTypeCandidate(candidateType, relationship, anchorProjectIds, anchorModuleIds, anchorPaths);
            if (candidates.TryGetValue(candidateTypeId, out var existingScore)) {
                if (score > existingScore) {
                    candidates[candidateTypeId] = score;
                }
            }
            else {
                candidates.Add(candidateTypeId, score);
            }
        }
    }

    private static IReadOnlyList<TypeRelationshipFact> SelectRelevantTypeRelationships(
        IReadOnlyList<TypeRelationshipFact> relationships,
        ISet<string> typeIdSet,
        ISet<string> anchorTypeIds) {
        return relationships
            .Where(item => typeIdSet.Contains(item.FromTypeId) && typeIdSet.Contains(item.ToTypeId))
            .OrderBy(item => anchorTypeIds.Contains(item.FromTypeId) || anchorTypeIds.Contains(item.ToTypeId) ? 0 : 1)
            .ThenByDescending(item => item.Weight)
            .ThenBy(item => item.Kind)
            .ThenBy(item => item.FromTypeId, StringComparer.Ordinal)
            .ThenBy(item => item.ToTypeId, StringComparer.Ordinal)
            .Take(MaxFocusedTypeRelationships)
            .ToArray();
    }

    private static IReadOnlyList<TypeFact> FindReferenceTypes(
        IReadOnlyList<TypeRelationshipFact> relationships,
        IReadOnlyList<TypeFact> selectedTypes,
        ISet<string> anchorTypeIds,
        IReadOnlyDictionary<string, TypeFact> typesById,
        IReadOnlyDictionary<string, ProjectFact> projectsById,
        TypeFact? seedType) {
        var selectedTypeIds = selectedTypes.Select(item => item.TypeId).ToHashSet(StringComparer.Ordinal);
        var anchorProjectIds = anchorTypeIds
            .Where(typesById.ContainsKey)
            .Select(typeId => typesById[typeId].ProjectId)
            .ToHashSet(StringComparer.Ordinal);
        var selectedModuleIds = selectedTypes.Select(item => item.ModuleId).ToHashSet(StringComparer.Ordinal);
        var candidates = new Dictionary<string, int>(StringComparer.Ordinal);

        foreach (var relationship in relationships) {
            CollectReferenceCandidate(relationship.FromTypeId, relationship.ToTypeId, relationship);
            CollectReferenceCandidate(relationship.ToTypeId, relationship.FromTypeId, relationship);
        }

        return candidates
            .Select(item => new ScoredTypeCandidate(typesById[item.Key], item.Value))
            .OrderByDescending(item => item.Score)
            .ThenBy(item => item.Type.DisplayName, StringComparer.Ordinal)
            .Take(MaxReferenceTypes)
            .Select(item => item.Type)
            .ToArray();

        void CollectReferenceCandidate(string anchorTypeId, string candidateTypeId, TypeRelationshipFact relationship) {
            if (!anchorTypeIds.Contains(anchorTypeId) || selectedTypeIds.Contains(candidateTypeId)) {
                return;
            }

            if (!typesById.TryGetValue(candidateTypeId, out var candidateType)) {
                return;
            }

            if (!anchorProjectIds.Contains(candidateType.ProjectId) || ShouldExcludeFromFocusedContext(candidateType, projectsById, seedType)) {
                return;
            }

            var score = 0;
            if (selectedModuleIds.Contains(candidateType.ModuleId)) {
                score += 30;
            }

            if (anchorTypeIds.Contains(anchorTypeId)) {
                score += 20;
            }

            score += relationship.Kind switch {
                TypeRelationshipKind.ConstructorParameter => 10,
                TypeRelationshipKind.MethodParameter => 10,
                TypeRelationshipKind.MethodReturn => 9,
                TypeRelationshipKind.Field => 7,
                TypeRelationshipKind.Property => 6,
                _ => 0,
            };

            score += Math.Min(relationship.Weight, 6);

            if (candidates.TryGetValue(candidateTypeId, out var existingScore)) {
                if (score > existingScore) {
                    candidates[candidateTypeId] = score;
                }
            }
            else {
                candidates.Add(candidateTypeId, score);
            }
        }
    }

    private static int ScoreTypeCandidate(
        TypeFact candidateType,
        TypeRelationshipFact relationship,
        ISet<string> anchorProjectIds,
        ISet<string> anchorModuleIds,
        ISet<string> anchorPaths) {
        var score = 0;
        if (anchorProjectIds.Contains(candidateType.ProjectId)) {
            score += 40;
        }
        else {
            score += 10;
        }

        if (anchorModuleIds.Contains(candidateType.ModuleId)) {
            score += 20;
        }

        if (relationship.Source is not null && anchorPaths.Contains(relationship.Source.Path)) {
            score += 15;
        }

        score += relationship.Kind switch {
            TypeRelationshipKind.ConstructorParameter => 12,
            TypeRelationshipKind.MethodParameter => 10,
            TypeRelationshipKind.MethodReturn => 9,
            TypeRelationshipKind.Field => 7,
            TypeRelationshipKind.Property => 6,
            _ => 0,
        };

        score += Math.Min(relationship.Weight, 8);
        return score;
    }

    private sealed record ScoredTypeCandidate(TypeFact Type, int Score);
}
