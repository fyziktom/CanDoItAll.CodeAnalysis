using CanDoItAll.CodeAnalytics.Abstractions;
using CanDoItAll.CodeAnalytics.Abstractions.Responses;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Sources;

namespace CanDoItAll.CodeAnalytics.Application.Services;

public sealed partial class CodeAnalyticsApplicationService {
    private static IReadOnlyList<FocusedContextSelectedMemberContext> CreateSelectedMemberContexts(
        IReadOnlyList<MemberFact> selectedMembers,
        TypeFact? seedType,
        MemberFact? seedMember,
        IReadOnlyList<TypeFact> implementationTypes,
        IReadOnlyList<RepresentativeConsumerCandidate> representativeConsumerCandidates,
        IReadOnlyDictionary<string, TypeFact> typesById) {
        var implementationTypeIds = implementationTypes
            .Select(item => item.TypeId)
            .ToHashSet(StringComparer.Ordinal);
        var representativeConsumerByMemberId = representativeConsumerCandidates
            .GroupBy(item => item.Member.MemberId, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.OrderByDescending(item => item.Score).First(), StringComparer.Ordinal);

        return selectedMembers
            .Select(
                member => {
                    typesById.TryGetValue(member.TypeId, out var memberType);
                    memberType ??= seedType;

                    var reasonKind = ResolveMemberReasonKind(member, seedType, seedMember, implementationTypeIds, representativeConsumerByMemberId);
                    var roleKind = ResolveMemberRoleKind(member, memberType, representativeConsumerByMemberId);
                    var excerptSource = representativeConsumerByMemberId.TryGetValue(member.MemberId, out var representativeCandidate)
                        ? representativeCandidate.Relationship.Source ?? member.Source
                        : member.Source;
                    return new FocusedContextSelectedMemberContext(
                        member,
                        excerptSource,
                        reasonKind,
                        roleKind,
                        GetMemberExcerptPriority(reasonKind, roleKind));
                })
            .ToArray();
    }

    private static FocusedContextSelectionReasonKind ResolveMemberReasonKind(
        MemberFact member,
        TypeFact? seedType,
        MemberFact? seedMember,
        ISet<string> implementationTypeIds,
        IReadOnlyDictionary<string, RepresentativeConsumerCandidate> representativeConsumerByMemberId) {
        if (string.Equals(member.MemberId, seedMember?.MemberId, StringComparison.Ordinal)) {
            return FocusedContextSelectionReasonKind.Seed;
        }

        if (seedType is not null && string.Equals(member.TypeId, seedType.TypeId, StringComparison.Ordinal)) {
            return FocusedContextSelectionReasonKind.SeedContext;
        }

        if (implementationTypeIds.Contains(member.TypeId)) {
            return FocusedContextSelectionReasonKind.Implementation;
        }

        if (representativeConsumerByMemberId.ContainsKey(member.MemberId)) {
            return FocusedContextSelectionReasonKind.RepresentativeConsumer;
        }

        return FocusedContextSelectionReasonKind.RelatedContext;
    }

    private static FocusedContextReferenceRoleKind ResolveMemberRoleKind(
        MemberFact member,
        TypeFact? memberType,
        IReadOnlyDictionary<string, RepresentativeConsumerCandidate> representativeConsumerByMemberId) {
        if (representativeConsumerByMemberId.TryGetValue(member.MemberId, out var representativeCandidate)) {
            return ClassifyReferenceRole(member, memberType, representativeCandidate.Relationship.Kind);
        }

        return ClassifyReferenceRole(member, memberType, null);
    }

    private static int GetMemberExcerptPriority(
        FocusedContextSelectionReasonKind reasonKind,
        FocusedContextReferenceRoleKind roleKind) {
        var priority = reasonKind switch {
            FocusedContextSelectionReasonKind.Seed => 400,
            FocusedContextSelectionReasonKind.SeedContext => 320,
            FocusedContextSelectionReasonKind.Implementation => 280,
            FocusedContextSelectionReasonKind.ServiceRegistration => 260,
            FocusedContextSelectionReasonKind.RepresentativeConsumer => 180,
            _ => 200,
        };
        return priority + GetRoleScoreBonus(roleKind);
    }

    private static FocusedContextReferenceRoleKind ClassifyReferenceRole(
        MemberFact? member,
        TypeFact? type,
        MemberRelationshipKind? relationshipKind) {
        if (type is null) {
            return FocusedContextReferenceRoleKind.None;
        }

        var typeRoleKind = ClassifyTypeRole(type);
        if (member is null) {
            return typeRoleKind;
        }

        var typeName = type.DisplayName;
        var memberName = member.DisplayName;
        if (typeRoleKind == FocusedContextReferenceRoleKind.Registration
            || memberName.Contains("ConfigureServices", StringComparison.Ordinal)) {
            return FocusedContextReferenceRoleKind.Registration;
        }

        if (typeRoleKind == FocusedContextReferenceRoleKind.Factory
            || memberName.Contains("CreateDbContext", StringComparison.Ordinal)
            || memberName.Contains("CreateOptions", StringComparison.Ordinal)
            || member.ReturnTypeDisplayName.Contains("DbContext", StringComparison.Ordinal)) {
            return FocusedContextReferenceRoleKind.Factory;
        }

        if (typeRoleKind == FocusedContextReferenceRoleKind.SchemaBootstrap
            || memberName.Contains("OnModelCreating", StringComparison.Ordinal)
            || memberName.Contains("EnsureAsync", StringComparison.Ordinal) && typeName.Contains("Schema", StringComparison.Ordinal)) {
            return FocusedContextReferenceRoleKind.SchemaBootstrap;
        }

        if (typeRoleKind == FocusedContextReferenceRoleKind.PreviewLifecycle) {
            return FocusedContextReferenceRoleKind.PreviewLifecycle;
        }

        if (relationshipKind is MemberRelationshipKind.Invocation
            or MemberRelationshipKind.ObjectCreation
            or MemberRelationshipKind.PropertyAccess) {
            return FocusedContextReferenceRoleKind.ConsumerService;
        }

        return FocusedContextReferenceRoleKind.None;
    }

    private static FocusedContextReferenceRoleKind ClassifyTypeRole(TypeFact type) {
        var typeName = type.DisplayName;
        var sourcePath = NormalizePath(type.Source.Path);
        if (sourcePath.Contains("/dependencyinjection/", StringComparison.OrdinalIgnoreCase)
            || typeName.Contains("ServiceCollectionExtensions", StringComparison.Ordinal)) {
            return FocusedContextReferenceRoleKind.Registration;
        }

        if (typeName.Contains("Factory", StringComparison.Ordinal)) {
            return FocusedContextReferenceRoleKind.Factory;
        }

        if (typeName.Contains("SchemaInitializer", StringComparison.Ordinal)
            || typeName.Contains("ModelRegistry", StringComparison.Ordinal)) {
            return FocusedContextReferenceRoleKind.SchemaBootstrap;
        }

        if (typeName.Contains("Preview", StringComparison.Ordinal)
            || typeName.Contains("SceneHost", StringComparison.Ordinal)
            || sourcePath.Contains("/canvas/", StringComparison.OrdinalIgnoreCase)) {
            return FocusedContextReferenceRoleKind.PreviewLifecycle;
        }

        return FocusedContextReferenceRoleKind.None;
    }

    private static int GetRoleScoreBonus(FocusedContextReferenceRoleKind roleKind) {
        return roleKind switch {
            FocusedContextReferenceRoleKind.Registration => 52,
            FocusedContextReferenceRoleKind.Factory => 40,
            FocusedContextReferenceRoleKind.SchemaBootstrap => 32,
            FocusedContextReferenceRoleKind.PreviewLifecycle => 22,
            FocusedContextReferenceRoleKind.ConsumerService => 12,
            _ => 0,
        };
    }

    private static IReadOnlyList<FocusedContextSelectionReason> BuildMemberSelectionReasons(
        IReadOnlyList<FocusedContextSelectedMemberContext> memberContexts) {
        return memberContexts
            .Select(
                item => new FocusedContextSelectionReason(
                    FocusedContextSelectionTargetKind.Member,
                    item.Member.MemberId,
                    item.ReasonKind,
                    item.RoleKind))
            .ToArray();
    }

    private sealed record FocusedContextSelectedMemberContext(
        MemberFact Member,
        SourceReference ExcerptSource,
        FocusedContextSelectionReasonKind ReasonKind,
        FocusedContextReferenceRoleKind RoleKind,
        int Priority);
}
