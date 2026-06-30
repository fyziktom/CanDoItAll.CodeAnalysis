using CanDoItAll.CodeAnalytics.Abstractions;
using CanDoItAll.CodeAnalytics.Abstractions.Responses;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Domain.Sources;

namespace CanDoItAll.CodeAnalytics.Application.Services;

public sealed partial class CodeAnalyticsApplicationService {
    private async Task<IReadOnlyList<ScoredSymbolReference>> CollectSymbolReferenceCandidatesAsync(
        ArchitectureSnapshot snapshot,
        SymbolQueryContext context,
        TypeFact type,
        MemberFact? member,
        CancellationToken cancellationToken) {
        var references = new Dictionary<string, ScoredSymbolReference>(StringComparer.Ordinal);

        foreach (var memberRelationship in snapshot.Facts.MemberRelationships) {
            if (!ShouldIncludeMemberRelationshipReference(context, type, member, memberRelationship)) {
                continue;
            }

            if (!context.MembersById.TryGetValue(memberRelationship.FromMemberId, out var sourceMember)
                || !context.TypesById.TryGetValue(sourceMember.TypeId, out var sourceType)) {
                continue;
            }

            var contextExcerpt = await CreateSymbolContextExcerptAsync(
                snapshot,
                memberRelationship.Source ?? sourceMember.Source,
                cancellationToken);
            if (contextExcerpt is null) {
                continue;
            }

            var referenceKind = MapReferenceKind(memberRelationship.Kind);
            AddSymbolReference(
                references,
                CreateReferenceKey(referenceKind, sourceType.TypeId, sourceMember.MemberId, contextExcerpt.StartLine),
                CreateSymbolReference(
                    context,
                    sourceType,
                    sourceMember,
                    referenceKind,
                    contextExcerpt),
                ScoreSymbolReference(type, sourceType, referenceKind));
        }

        if (member is null) {
            foreach (var typeRelationship in snapshot.Facts.TypeRelationships.Where(item => string.Equals(item.ToTypeId, type.TypeId, StringComparison.Ordinal))) {
                if (!context.TypesById.TryGetValue(typeRelationship.FromTypeId, out var sourceType)) {
                    continue;
                }

                var sourceMember = ResolveSourceMemberForTypeReference(context, sourceType.TypeId, typeRelationship.Source);
                var contextExcerpt = await CreateSymbolContextExcerptAsync(
                    snapshot,
                    typeRelationship.Source ?? sourceMember?.Source ?? sourceType.Source,
                    cancellationToken);
                if (contextExcerpt is null) {
                    continue;
                }

                var referenceKind = MapReferenceKind(typeRelationship.Kind);
                AddSymbolReference(
                    references,
                    CreateReferenceKey(referenceKind, sourceType.TypeId, sourceMember?.MemberId, contextExcerpt.StartLine),
                    CreateSymbolReference(context, sourceType, sourceMember, referenceKind, contextExcerpt),
                    ScoreSymbolReference(type, sourceType, referenceKind));
            }

            foreach (var service in snapshot.Facts.ServiceRegistrations.Where(item => ServiceReferencesType(item, type))) {
                var sourceType = FindTypeBySource(snapshot.Facts.Types, service.Source.Path, service.Source.Line);
                if (sourceType is null) {
                    continue;
                }

                var sourceMember = FindMemberBySource(snapshot.Facts.Members, service.Source.Path, service.Source.Line);
                var contextExcerpt = await CreateSymbolContextExcerptAsync(snapshot, service.Source, cancellationToken);
                if (contextExcerpt is null) {
                    continue;
                }

                AddSymbolReference(
                    references,
                    CreateReferenceKey(SymbolReferenceKind.ServiceRegistration, sourceType.TypeId, sourceMember?.MemberId, contextExcerpt.StartLine),
                    CreateSymbolReference(context, sourceType, sourceMember, SymbolReferenceKind.ServiceRegistration, contextExcerpt),
                    ScoreSymbolReference(type, sourceType, SymbolReferenceKind.ServiceRegistration));
            }
        }

        return references.Values.ToArray();
    }

    private static bool ShouldIncludeMemberRelationshipReference(
        SymbolQueryContext context,
        TypeFact type,
        MemberFact? member,
        MemberRelationshipFact relationship) {
        if (member is not null) {
            return string.Equals(relationship.ToMemberId, member.MemberId, StringComparison.Ordinal);
        }

        return context.MembersByTypeId.TryGetValue(type.TypeId, out var members)
            && members.Any(item => string.Equals(item.MemberId, relationship.ToMemberId, StringComparison.Ordinal));
    }

    private static MemberFact? ResolveSourceMemberForTypeReference(
        SymbolQueryContext context,
        string typeId,
        SourceReference? source) {
        if (source is null || !context.MembersByTypeId.TryGetValue(typeId, out var members)) {
            return null;
        }

        return members
            .Where(item => string.Equals(NormalizePath(item.Source.Path), NormalizePath(source.Path), StringComparison.OrdinalIgnoreCase))
            .OrderBy(item => source.Line.HasValue && SourceContainsLine(item.Source, source.Line.Value) ? 0 : 1)
            .ThenBy(item => source.Line.HasValue && item.Source.Line.HasValue ? Math.Abs(item.Source.Line.Value - source.Line.Value) : int.MaxValue)
            .ThenBy(item => item.DisplayName, StringComparer.Ordinal)
            .FirstOrDefault();
    }

    private static bool ServiceReferencesType(ServiceRegistrationFact service, TypeFact type) {
        return string.Equals(service.ServiceTypeDisplayName, type.DisplayName, StringComparison.Ordinal)
            || string.Equals(service.ImplementationTypeDisplayName, type.DisplayName, StringComparison.Ordinal);
    }

    private static string CreateReferenceKey(
        SymbolReferenceKind kind,
        string sourceTypeId,
        string? sourceMemberId,
        int startLine) {
        return $"{kind}:{sourceTypeId}:{sourceMemberId}:{startLine}";
    }

    private static SymbolReferenceItem CreateSymbolReference(
        SymbolQueryContext context,
        TypeFact sourceType,
        MemberFact? sourceMember,
        SymbolReferenceKind kind,
        SymbolSourceExcerpt contextExcerpt) {
        var names = ResolveSymbolNames(context, sourceType);
        return new SymbolReferenceItem(
            kind,
            names.ProjectName,
            names.ModuleName,
            names.NamespaceName,
            sourceType,
            sourceMember,
            contextExcerpt.Path,
            contextExcerpt.StartLine,
            contextExcerpt);
    }

    private static int ScoreSymbolReference(
        TypeFact targetType,
        TypeFact sourceType,
        SymbolReferenceKind kind) {
        var score = kind switch {
            SymbolReferenceKind.Invocation => 240,
            SymbolReferenceKind.PropertyAccess => 220,
            SymbolReferenceKind.ObjectCreation => 210,
            SymbolReferenceKind.ConstructorParameter => 180,
            SymbolReferenceKind.MethodParameter => 170,
            SymbolReferenceKind.MethodReturn => 150,
            SymbolReferenceKind.Property => 150,
            SymbolReferenceKind.Field => 140,
            SymbolReferenceKind.Event => 130,
            SymbolReferenceKind.ServiceRegistration => 200,
            _ => 100,
        };

        if (string.Equals(sourceType.ProjectId, targetType.ProjectId, StringComparison.Ordinal)) {
            score += 80;
        }

        if (string.Equals(sourceType.ModuleId, targetType.ModuleId, StringComparison.Ordinal)) {
            score += 40;
        }

        return score;
    }

    private static void AddSymbolReference(
        IDictionary<string, ScoredSymbolReference> references,
        string key,
        SymbolReferenceItem reference,
        int score) {
        if (!references.TryGetValue(key, out var existing) || score > existing.Score) {
            references[key] = new ScoredSymbolReference(reference, score);
        }
    }

    private static SymbolReferenceKind MapReferenceKind(MemberRelationshipKind relationshipKind) {
        return relationshipKind switch {
            MemberRelationshipKind.Invocation => SymbolReferenceKind.Invocation,
            MemberRelationshipKind.ObjectCreation => SymbolReferenceKind.ObjectCreation,
            MemberRelationshipKind.PropertyAccess => SymbolReferenceKind.PropertyAccess,
            MemberRelationshipKind.FieldAccess => SymbolReferenceKind.FieldAccess,
            _ => SymbolReferenceKind.Invocation,
        };
    }

    private static SymbolReferenceKind MapReferenceKind(TypeRelationshipKind relationshipKind) {
        return relationshipKind switch {
            TypeRelationshipKind.ConstructorParameter => SymbolReferenceKind.ConstructorParameter,
            TypeRelationshipKind.MethodParameter => SymbolReferenceKind.MethodParameter,
            TypeRelationshipKind.MethodReturn => SymbolReferenceKind.MethodReturn,
            TypeRelationshipKind.Property => SymbolReferenceKind.Property,
            TypeRelationshipKind.Field => SymbolReferenceKind.Field,
            TypeRelationshipKind.Event => SymbolReferenceKind.Event,
            _ => SymbolReferenceKind.Property,
        };
    }
}
