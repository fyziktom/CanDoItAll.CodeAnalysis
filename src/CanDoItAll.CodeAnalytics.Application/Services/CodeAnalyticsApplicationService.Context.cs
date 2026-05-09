using CanDoItAll.CodeAnalytics.Abstractions.Queries;
using CanDoItAll.CodeAnalytics.Abstractions.Responses;
using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Application.Services;

public sealed partial class CodeAnalyticsApplicationService {
    private const int MaxSeedMembers = 4;
    private const int MaxFocusedMembers = 40;
    private const int MaxFocusedTypes = 32;
    private const int MaxExternalFocusedTypes = 10;
    private const int MaxExternalTypesPerProject = 3;
    private const int MaxFocusedMemberRelationships = 80;
    private const int MaxFocusedTypeRelationships = 80;
    private const int MaxRelatedServices = 24;
    private const int MaxReferenceTypes = 6;
    private const int MaxFocusedFiles = 8;
    private const int MaxExcerptBlocks = 16;
    private const int MaxExcerptBlocksPerFile = 6;
    private const int MaxRepresentativeMembersPerType = 1;
    private const int MaxFrontierMembersPerType = 2;
    private const int MaxFrontierMembersInSeedProject = 4;
    private const int MaxFrontierMembersPerExternalProject = 2;
    private const int MaxFrontierExternalProjects = 4;
    private const int DefaultExcerptPaddingLines = 1;
    private const int DefaultExcerptLength = 8;
    private const int DefaultTypeHeaderLength = 10;

    public async Task<FocusedContextResponse?> GetFocusedContextAsync(
        FocusedContextQuery query,
        CancellationToken cancellationToken = default) {
        var snapshot = await GetSnapshotAsync(query.SnapshotId, cancellationToken);
        if (snapshot is null) {
            return null;
        }

        var depth = Math.Clamp(query.Depth, 0, 5);
        var focusTags = NormalizeFocusTags(query.FocusTags);
        var relationHints = NormalizeRelationHints(query.RelationHints);
        var typesById = snapshot.Facts.Types.ToDictionary(item => item.TypeId, StringComparer.Ordinal);
        var membersById = snapshot.Facts.Members.ToDictionary(item => item.MemberId, StringComparer.Ordinal);
        var servicesById = snapshot.Facts.ServiceRegistrations.ToDictionary(item => item.ServiceRegistrationId, StringComparer.Ordinal);
        var projectsById = snapshot.Facts.Projects.ToDictionary(item => item.ProjectId, StringComparer.Ordinal);
        var modulesById = snapshot.Facts.Modules.ToDictionary(item => item.ModuleId, StringComparer.Ordinal);
        var membersByTypeId = snapshot.Facts.Members
            .GroupBy(item => item.TypeId, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => (IReadOnlyList<MemberFact>)group.ToArray(), StringComparer.Ordinal);

        var seed = ResolveFocusedContextSeed(
            query,
            snapshot,
            typesById,
            membersById,
            servicesById,
            projectsById,
            membersByTypeId,
            focusTags);
        if (!seed.HasSeed) {
            return null;
        }

        var seedType = seed.SeedType;
        var seedMember = seed.SeedMember;
        var seedService = seed.SeedService;
        var seedMemberIds = ResolveSeedMemberIds(seedType, seedMember, membersByTypeId, query.QueryText, focusTags);
        var strategy = ResolveFocusedContextStrategy(
            query,
            seedType,
            seedMember,
            seedMemberIds,
            snapshot.Facts.MemberRelationships,
            membersById,
            typesById,
            projectsById,
            relationHints);
        var memberSelection = SelectFocusedMembers(
            seedType,
            seedMember,
            seedMemberIds,
            snapshot.Facts.Types,
            snapshot.Facts.MemberRelationships,
            membersById,
            typesById,
            projectsById,
            modulesById,
            membersByTypeId,
            strategy,
            focusTags,
            relationHints);
        var selectedMembers = OrderSelectedMembers(
            memberSelection.SelectedMemberIds,
            membersById,
            seedType,
            seedMember,
            focusTags,
            memberSelection.ImplementationTypes,
            memberSelection.RepresentativeConsumerCandidates,
            typesById,
            strategy);
        var selectedMemberContexts = CreateSelectedMemberContexts(
            selectedMembers,
            seedType,
            seedMember,
            memberSelection.ImplementationTypes,
            memberSelection.RepresentativeConsumerCandidates,
            typesById);
        var selectedTypes = SelectRelevantTypes(
            seedType,
            memberSelection.ImplementationTypes,
            selectedMembers,
            snapshot.Facts.TypeRelationships,
            typesById,
            projectsById,
            focusTags,
            strategy);

        var anchorTypeIds = selectedMembers.Select(item => item.TypeId).ToHashSet(StringComparer.Ordinal);
        if (seedType is not null) {
            anchorTypeIds.Add(seedType.TypeId);
        }

        var memberIdSet = selectedMembers.Select(item => item.MemberId).ToHashSet(StringComparer.Ordinal);
        var typeIdSet = selectedTypes.Select(item => item.TypeId).ToHashSet(StringComparer.Ordinal);
        var relatedServices = SelectRelevantServices(snapshot.Facts.ServiceRegistrations, typeIdSet, snapshot.Facts.Types, projectsById, seedType, focusTags);
        var referenceTypes = FindReferenceTypes(
            snapshot.Facts.TypeRelationships,
            selectedTypes,
            anchorTypeIds,
            typesById,
            projectsById,
            seedType,
            focusTags,
            strategy);
        var fileBuild = await BuildFocusedContextFilesAsync(
            snapshot,
            seedType,
            seedMember,
            selectedTypes,
            selectedMemberContexts,
            membersByTypeId,
            relatedServices,
            focusTags,
            strategy,
            cancellationToken);
        var selectionReasons = BuildMemberSelectionReasons(selectedMemberContexts)
            .Concat(fileBuild.SelectionReasons)
            .Distinct()
            .ToArray();
        var stats = BuildFocusedContextStats(fileBuild.Files);

        return new FocusedContextResponse(
            snapshot.SnapshotId,
            strategy.EffectiveDepth,
            query.QueryText?.Trim(),
            focusTags,
            relationHints,
            strategy.RequestedIntent,
            strategy.ResolvedIntent,
            strategy.RequestedPrecision,
            strategy.ResolvedPrecision,
            strategy.StrategyExplanation,
            seed.SeedExplanation,
            seedType,
            seedMember,
            seedService,
            memberSelection.ImplementationTypes,
            selectedTypes,
            selectedMembers,
            SelectRelevantMemberRelationships(snapshot.Facts.MemberRelationships, memberIdSet, membersById, typesById, projectsById, seedType),
            SelectRelevantTypeRelationships(snapshot.Facts.TypeRelationships, typeIdSet, anchorTypeIds),
            relatedServices,
            referenceTypes,
            memberSelection.UsageSummary,
            selectionReasons,
            stats,
            fileBuild.Files);
    }

    private static IReadOnlyList<string> ResolveSeedMemberIds(
        TypeFact? seedType,
        MemberFact? seedMember,
        IReadOnlyDictionary<string, IReadOnlyList<MemberFact>> membersByTypeId,
        string? queryText,
        IReadOnlyCollection<string> focusTags) {
        if (seedMember is not null) {
            return [seedMember.MemberId];
        }

        if (seedType is null || !membersByTypeId.TryGetValue(seedType.TypeId, out var members)) {
            return [];
        }

        var callableMembers = RankSeedMembers(seedType, members, queryText, focusTags)
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

    private static IReadOnlyList<MemberFact> RankSeedMembers(
        TypeFact type,
        IReadOnlyList<MemberFact> members,
        string? queryText,
        IReadOnlyCollection<string> focusTags) {
        return members
            .Where(item => item.Kind is MemberKind.Method or MemberKind.Constructor or MemberKind.Property)
            .Select(item => new ScoredSeedMember(item, ScoreSeedMember(type, item, queryText, focusTags)))
            .OrderByDescending(item => item.Score)
            .ThenBy(item => item.Member.DisplayName, StringComparer.Ordinal)
            .Select(item => item.Member)
            .ToArray();
    }

    private sealed record ResolvedFocusedContextSeed(
        TypeFact? SeedType,
        MemberFact? SeedMember,
        ServiceRegistrationFact? SeedService,
        string? SeedExplanation) {
        public bool HasSeed => SeedType is not null || SeedMember is not null || SeedService is not null;
    }

    private sealed record ScoredSeedMember(MemberFact Member, int Score);
}
