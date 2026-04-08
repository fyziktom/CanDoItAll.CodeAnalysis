using CanDoItAll.CodeAnalytics.Abstractions.Queries;
using CanDoItAll.CodeAnalytics.Abstractions.Responses;
using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Application.Services;

public sealed partial class CodeAnalyticsApplicationService {
    private const int MaxSeedMembers = 12;
    private const int MaxFocusedMembers = 40;
    private const int MaxFocusedTypes = 32;
    private const int MaxExternalFocusedTypes = 10;
    private const int MaxExternalTypesPerProject = 3;
    private const int MaxFocusedMemberRelationships = 80;
    private const int MaxFocusedTypeRelationships = 80;
    private const int MaxRelatedServices = 24;
    private const int MaxReferenceTypes = 6;

    public async Task<FocusedContextResponse?> GetFocusedContextAsync(
        FocusedContextQuery query,
        CancellationToken cancellationToken = default) {
        var snapshot = await GetSnapshotAsync(query.SnapshotId, cancellationToken);
        if (snapshot is null) {
            return null;
        }

        var depth = Math.Clamp(query.Depth, 0, 5);
        var typesById = snapshot.Facts.Types.ToDictionary(item => item.TypeId, StringComparer.Ordinal);
        var membersById = snapshot.Facts.Members.ToDictionary(item => item.MemberId, StringComparer.Ordinal);
        var servicesById = snapshot.Facts.ServiceRegistrations.ToDictionary(item => item.ServiceRegistrationId, StringComparer.Ordinal);
        var projectsById = snapshot.Facts.Projects.ToDictionary(item => item.ProjectId, StringComparer.Ordinal);
        var membersByTypeId = snapshot.Facts.Members
            .GroupBy(item => item.TypeId, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => (IReadOnlyList<MemberFact>)group.ToArray(), StringComparer.Ordinal);

        var seedType = !string.IsNullOrWhiteSpace(query.TypeId) && typesById.TryGetValue(query.TypeId, out var resolvedType)
            ? resolvedType
            : null;
        var seedMember = !string.IsNullOrWhiteSpace(query.MemberId) && membersById.TryGetValue(query.MemberId, out var resolvedMember)
            ? resolvedMember
            : null;
        var seedService = !string.IsNullOrWhiteSpace(query.ServiceRegistrationId) && servicesById.TryGetValue(query.ServiceRegistrationId, out var resolvedService)
            ? resolvedService
            : null;

        if (seedService is not null && seedType is null) {
            seedType = ResolveTypeForService(seedService, snapshot.Facts.Types);
        }

        if (seedMember is not null && seedType is null && typesById.TryGetValue(seedMember.TypeId, out var memberType)) {
            seedType = memberType;
        }

        if (seedMember is null && seedType is null && seedService is null) {
            return null;
        }

        var seedMemberIds = ResolveSeedMemberIds(seedType, seedMember, membersByTypeId);
        var selectedMemberIds = ExpandMemberNeighborhood(seedMemberIds, snapshot.Facts.MemberRelationships, membersById, typesById, projectsById, seedType, depth);
        var selectedMembers = selectedMemberIds
            .Where(membersById.ContainsKey)
            .Select(memberId => membersById[memberId])
            .OrderBy(item => string.Equals(item.MemberId, seedMember?.MemberId, StringComparison.Ordinal) ? 0 : 1)
            .ThenBy(item => string.Equals(item.TypeId, seedType?.TypeId, StringComparison.Ordinal) ? 0 : 1)
            .ThenBy(item => item.DisplayName, StringComparer.Ordinal)
            .ToArray();

        var selectedTypes = SelectRelevantTypes(seedType, selectedMembers, snapshot.Facts.TypeRelationships, typesById, projectsById);

        var anchorTypeIds = selectedMembers.Select(item => item.TypeId).ToHashSet(StringComparer.Ordinal);
        if (seedType is not null) {
            anchorTypeIds.Add(seedType.TypeId);
        }

        var memberIdSet = selectedMembers.Select(item => item.MemberId).ToHashSet(StringComparer.Ordinal);
        var typeIdSet = selectedTypes.Select(item => item.TypeId).ToHashSet(StringComparer.Ordinal);
        var relatedServices = SelectRelevantServices(snapshot.Facts.ServiceRegistrations, typeIdSet, snapshot.Facts.Types, projectsById, seedType);
        var referenceTypes = FindReferenceTypes(snapshot.Facts.TypeRelationships, selectedTypes, anchorTypeIds, typesById, projectsById, seedType);

        return new FocusedContextResponse(
            snapshot.SnapshotId,
            depth,
            seedType,
            seedMember,
            seedService,
            selectedTypes,
            selectedMembers,
            SelectRelevantMemberRelationships(snapshot.Facts.MemberRelationships, memberIdSet, membersById, typesById, projectsById, seedType),
            SelectRelevantTypeRelationships(snapshot.Facts.TypeRelationships, typeIdSet, anchorTypeIds),
            relatedServices,
            referenceTypes);
    }

    private static IReadOnlyList<string> ResolveSeedMemberIds(
        TypeFact? seedType,
        MemberFact? seedMember,
        IReadOnlyDictionary<string, IReadOnlyList<MemberFact>> membersByTypeId) {
        if (seedMember is not null) {
            return [seedMember.MemberId];
        }

        if (seedType is null || !membersByTypeId.TryGetValue(seedType.TypeId, out var members)) {
            return [];
        }

        var callableMembers = members
            .Where(item => item.Kind is MemberKind.Method or MemberKind.Constructor or MemberKind.Property)
            .OrderBy(item => item.Kind is MemberKind.Constructor ? 0 : item.Kind is MemberKind.Method ? 1 : 2)
            .ThenBy(item => item.DisplayName, StringComparer.Ordinal)
            .Take(MaxSeedMembers)
            .Select(item => item.MemberId)
            .ToArray();
        return callableMembers.Length > 0
            ? callableMembers
            : members.Select(item => item.MemberId).ToArray();
    }

    private static TypeFact? ResolveTypeForService(
        ServiceRegistrationFact service,
        IReadOnlyList<TypeFact> types) {
        return ResolveTypeByDisplayName(types, service.ImplementationTypeDisplayName, service.ProjectId)
            ?? ResolveTypeByDisplayName(types, service.ServiceTypeDisplayName, service.ProjectId);
    }

    private static TypeFact? ResolveTypeByDisplayName(
        IReadOnlyList<TypeFact> types,
        string? displayName,
        string projectId) {
        if (string.IsNullOrWhiteSpace(displayName)) {
            return null;
        }

        return types.FirstOrDefault(item => string.Equals(item.ProjectId, projectId, StringComparison.Ordinal) && string.Equals(item.DisplayName, displayName, StringComparison.Ordinal))
            ?? types.FirstOrDefault(item => string.Equals(item.DisplayName, displayName, StringComparison.Ordinal));
    }
}
