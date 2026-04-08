using CanDoItAll.CodeAnalytics.Abstractions;
using CanDoItAll.CodeAnalytics.Abstractions.Queries;
using CanDoItAll.CodeAnalytics.Abstractions.Responses;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;

namespace CanDoItAll.CodeAnalytics.Application.Services;

public sealed partial class CodeAnalyticsApplicationService {
    private const int MaxSymbolSearchResults = 60;
    private const int MaxSymbolReferenceResults = 80;

    public async Task<SymbolSearchResponse?> SearchSymbolsAsync(
        SymbolSearchQuery query,
        CancellationToken cancellationToken = default) {
        var snapshot = await GetSnapshotAsync(query.SnapshotId, cancellationToken);
        if (snapshot is null) {
            return null;
        }

        var context = CreateSymbolQueryContext(snapshot);
        var take = Math.Clamp(query.Take, 1, MaxSymbolSearchResults);
        if (string.IsNullOrWhiteSpace(query.SearchText)) {
            return new SymbolSearchResponse(
                snapshot.SnapshotId,
                query.SearchText,
                query.ProjectName,
                query.SearchMode,
                context.AvailableProjects,
                null,
                []);
        }

        if (!TryCreateSymbolMatcher(query.SearchText, query.SearchMode, out var matcher, out var validationError)) {
            return new SymbolSearchResponse(
                snapshot.SnapshotId,
                query.SearchText,
                query.ProjectName,
                query.SearchMode,
                context.AvailableProjects,
                validationError,
                []);
        }

        var includeDeclarationField = ShouldIncludeDeclarationField(query.SearchMode, query.SearchText);
        var results = new List<ScoredSymbolSearchResult>();

        if (query.IncludeTypes) {
            foreach (var type in snapshot.Facts.Types) {
                if (!MatchesProjectFilter(context, type.ProjectId, query.ProjectName)) {
                    continue;
                }

                var declaration = BuildTypeDeclaration(type);
                var matchFields = CollectSymbolMatchFields(
                    matcher,
                    (type.DisplayName, SymbolMatchFieldKind.DisplayName),
                    (includeDeclarationField ? declaration : null, SymbolMatchFieldKind.Declaration),
                    (type.XmlSummary, SymbolMatchFieldKind.Summary),
                    (type.Source.Path, SymbolMatchFieldKind.Path));
                if (matchFields.Count == 0) {
                    continue;
                }

                var names = ResolveSymbolNames(context, type);
                results.Add(
                    new ScoredSymbolSearchResult(
                        CreateTypeSearchResult(type, names, declaration, matchFields),
                        ScoreTypeSearchResult(type, matcher, declaration, matchFields)));
            }
        }

        if (query.IncludeMembers) {
            foreach (var member in snapshot.Facts.Members) {
                if (!context.TypesById.TryGetValue(member.TypeId, out var type)) {
                    continue;
                }

                if (!MatchesProjectFilter(context, type.ProjectId, query.ProjectName)) {
                    continue;
                }

                var declaration = BuildMemberDeclaration(member);
                var matchFields = CollectSymbolMatchFields(
                    matcher,
                    (member.DisplayName, SymbolMatchFieldKind.DisplayName),
                    (includeDeclarationField ? declaration : null, SymbolMatchFieldKind.Declaration),
                    (member.Source.Path, SymbolMatchFieldKind.Path));
                if (matchFields.Count == 0) {
                    continue;
                }

                var names = ResolveSymbolNames(context, type);
                results.Add(
                    new ScoredSymbolSearchResult(
                        CreateMemberSearchResult(type, member, names, declaration, matchFields),
                        ScoreMemberSearchResult(type, member, matcher, declaration, matchFields)));
            }
        }

        return new SymbolSearchResponse(
            snapshot.SnapshotId,
            query.SearchText,
            query.ProjectName,
            query.SearchMode,
            context.AvailableProjects,
            null,
            results
                .OrderByDescending(item => item.Score)
                .ThenBy(item => item.Result.TargetKind)
                .ThenBy(item => item.Result.DisplayName, StringComparer.Ordinal)
                .ThenBy(item => item.Result.Path, StringComparer.Ordinal)
                .Take(take)
                .Select(item => item.Result)
                .ToArray());
    }

    public async Task<SymbolDefinitionResponse?> GetSymbolDefinitionAsync(
        SymbolDefinitionQuery query,
        CancellationToken cancellationToken = default) {
        var snapshot = await GetSnapshotAsync(query.SnapshotId, cancellationToken);
        if (snapshot is null) {
            return null;
        }

        var context = CreateSymbolQueryContext(snapshot);
        if (!TryResolveSymbolTarget(context, query.TypeId, query.MemberId, out var type, out var member)) {
            return null;
        }

        var names = ResolveSymbolNames(context, type);
        var definition = await CreateSymbolDefinitionExcerptAsync(
            snapshot,
            member?.Source ?? type.Source,
            member is null ? BuildTypeDefinitionLineLimit(type) : MaxMemberDefinitionLines,
            cancellationToken);
        if (definition is null) {
            return null;
        }

        var containingTypeHeader = member is null
            ? null
            : await CreateContainingTypeHeaderAsync(snapshot, type.Source, cancellationToken);

        return new SymbolDefinitionResponse(
            snapshot.SnapshotId,
            member is null ? SymbolTargetKind.Type : SymbolTargetKind.Member,
            names.ProjectName,
            names.ModuleName,
            names.NamespaceName,
            type,
            member,
            member is null ? BuildTypeDeclaration(type) : BuildMemberDeclaration(member),
            type.XmlSummary,
            definition,
            containingTypeHeader);
    }

    public async Task<SymbolMembersResponse?> GetSymbolMembersAsync(
        SymbolMembersQuery query,
        CancellationToken cancellationToken = default) {
        var snapshot = await GetSnapshotAsync(query.SnapshotId, cancellationToken);
        if (snapshot is null) {
            return null;
        }

        var context = CreateSymbolQueryContext(snapshot);
        if (!context.TypesById.TryGetValue(query.TypeId, out var type)) {
            return null;
        }

        var names = ResolveSymbolNames(context, type);
        var members = context.MembersByTypeId.TryGetValue(type.TypeId, out var resolvedMembers)
            ? resolvedMembers
                .OrderBy(item => item.Kind)
                .ThenBy(item => item.DisplayName, StringComparer.Ordinal)
                .ToArray()
            : [];

        return new SymbolMembersResponse(
            snapshot.SnapshotId,
            names.ProjectName,
            names.ModuleName,
            names.NamespaceName,
            type,
            members);
    }

    public async Task<SymbolImplementationsResponse?> GetSymbolImplementationsAsync(
        SymbolImplementationsQuery query,
        CancellationToken cancellationToken = default) {
        var snapshot = await GetSnapshotAsync(query.SnapshotId, cancellationToken);
        if (snapshot is null) {
            return null;
        }

        var context = CreateSymbolQueryContext(snapshot);
        if (!context.TypesById.TryGetValue(query.TypeId, out var type)) {
            return null;
        }

        var names = ResolveSymbolNames(context, type);
        var implementations = snapshot.Facts.Types
            .Where(candidate => !string.Equals(candidate.TypeId, type.TypeId, StringComparison.Ordinal))
            .Select(
                candidate => {
                    var implementationKind = ResolveImplementationKind(type, candidate);
                    if (implementationKind is null) {
                        return null;
                    }

                    var candidateNames = ResolveSymbolNames(context, candidate);
                    return new SymbolImplementationItem(
                        implementationKind.Value,
                        candidateNames.ProjectName,
                        candidateNames.ModuleName,
                        candidateNames.NamespaceName,
                        candidate);
                })
            .Where(item => item is not null)
            .Cast<SymbolImplementationItem>()
            .OrderBy(item => item.Kind)
            .ThenBy(item => item.ProjectName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(item => item.NamespaceName, StringComparer.Ordinal)
            .ThenBy(item => item.Type.DisplayName, StringComparer.Ordinal)
            .ToArray();

        return new SymbolImplementationsResponse(
            snapshot.SnapshotId,
            names.ProjectName,
            names.ModuleName,
            names.NamespaceName,
            type,
            implementations);
    }

    public async Task<SymbolReferencesResponse?> GetSymbolReferencesAsync(
        SymbolReferencesQuery query,
        CancellationToken cancellationToken = default) {
        var snapshot = await GetSnapshotAsync(query.SnapshotId, cancellationToken);
        if (snapshot is null) {
            return null;
        }

        var context = CreateSymbolQueryContext(snapshot);
        if (!TryResolveSymbolTarget(context, query.TypeId, query.MemberId, out var type, out var member)) {
            return null;
        }

        var take = Math.Clamp(query.Take, 1, MaxSymbolReferenceResults);
        var referenceCandidates = await CollectSymbolReferenceCandidatesAsync(snapshot, context, type, member, cancellationToken);
        var references = referenceCandidates
            .OrderByDescending(item => item.Score)
            .ThenBy(item => item.Reference.ProjectName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(item => item.Reference.NamespaceName, StringComparer.Ordinal)
            .ThenBy(item => item.Reference.SourceType.DisplayName, StringComparer.Ordinal)
            .ThenBy(item => item.Reference.Line ?? int.MaxValue)
            .Take(take)
            .Select(item => item.Reference)
            .ToArray();

        return new SymbolReferencesResponse(
            snapshot.SnapshotId,
            member is null ? SymbolTargetKind.Type : SymbolTargetKind.Member,
            type,
            member,
            referenceCandidates.Count,
            references);
    }

    private static SymbolQueryContext CreateSymbolQueryContext(ArchitectureSnapshot snapshot) {
        var typesById = snapshot.Facts.Types.ToDictionary(item => item.TypeId, StringComparer.Ordinal);
        var membersById = snapshot.Facts.Members.ToDictionary(item => item.MemberId, StringComparer.Ordinal);
        var membersByTypeId = snapshot.Facts.Members
            .GroupBy(item => item.TypeId, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<MemberFact>)group.ToArray(),
                StringComparer.Ordinal);
        var projectsById = snapshot.Facts.Projects.ToDictionary(item => item.ProjectId, StringComparer.Ordinal);
        var modulesById = snapshot.Facts.Modules.ToDictionary(item => item.ModuleId, StringComparer.Ordinal);
        var namespacesById = snapshot.Facts.Namespaces.ToDictionary(item => item.NamespaceId, StringComparer.Ordinal);
        var availableProjects = snapshot.Facts.Projects
            .Select(item => item.Name)
            .OrderBy(item => item, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new SymbolQueryContext(
            typesById,
            membersById,
            membersByTypeId,
            projectsById,
            modulesById,
            namespacesById,
            availableProjects);
    }

    private static bool TryResolveSymbolTarget(
        SymbolQueryContext context,
        string typeId,
        string? memberId,
        out TypeFact type,
        out MemberFact? member) {
        if (!context.TypesById.TryGetValue(typeId, out type!)) {
            member = null;
            return false;
        }

        if (string.IsNullOrWhiteSpace(memberId)) {
            member = null;
            return true;
        }

        if (!context.MembersById.TryGetValue(memberId, out var resolvedMember)
            || !string.Equals(resolvedMember.TypeId, type.TypeId, StringComparison.Ordinal)) {
            member = null;
            return false;
        }

        member = resolvedMember;
        return true;
    }

    private static bool MatchesProjectFilter(
        SymbolQueryContext context,
        string projectId,
        string? projectName) {
        if (string.IsNullOrWhiteSpace(projectName)) {
            return true;
        }

        return context.ProjectsById.TryGetValue(projectId, out var project)
            && string.Equals(project.Name, projectName, StringComparison.OrdinalIgnoreCase);
    }

    private static SymbolNames ResolveSymbolNames(SymbolQueryContext context, TypeFact type) {
        return new SymbolNames(
            context.ProjectsById.TryGetValue(type.ProjectId, out var project) ? project.Name : type.ProjectId,
            context.ModulesById.TryGetValue(type.ModuleId, out var module) ? module.Name : type.ModuleId,
            context.NamespacesById.TryGetValue(type.NamespaceId, out var @namespace) ? @namespace.Name : type.NamespaceId);
    }

    private static SymbolSearchResultItem CreateTypeSearchResult(
        TypeFact type,
        SymbolNames names,
        string declaration,
        IReadOnlyList<SymbolMatchFieldKind> matchFields) {
        return new SymbolSearchResultItem(
            SymbolTargetKind.Type,
            names.ProjectName,
            names.ModuleName,
            names.NamespaceName,
            type.Source.Path,
            type.Source.Line,
            type.DisplayName,
            declaration,
            null,
            type.TypeId,
            null,
            matchFields);
    }

    private static SymbolSearchResultItem CreateMemberSearchResult(
        TypeFact type,
        MemberFact member,
        SymbolNames names,
        string declaration,
        IReadOnlyList<SymbolMatchFieldKind> matchFields) {
        return new SymbolSearchResultItem(
            SymbolTargetKind.Member,
            names.ProjectName,
            names.ModuleName,
            names.NamespaceName,
            member.Source.Path,
            member.Source.Line,
            member.DisplayName,
            declaration,
            type.DisplayName,
            type.TypeId,
            member.MemberId,
            matchFields);
    }

    private static int ScoreTypeSearchResult(
        TypeFact type,
        SymbolMatcher matcher,
        string declaration,
        IReadOnlyList<SymbolMatchFieldKind> matchFields) {
        var score = 0;

        if (matchFields.Contains(SymbolMatchFieldKind.DisplayName)) {
            score += ScoreSearchMatch(matcher, type.DisplayName, 460, 320);
        }

        if (matchFields.Contains(SymbolMatchFieldKind.Declaration)) {
            score += ScoreSearchMatch(matcher, declaration, 360, 220);
        }

        if (matchFields.Contains(SymbolMatchFieldKind.Summary)) {
            score += ScoreSearchMatch(matcher, type.XmlSummary, 140, 80);
        }

        if (matchFields.Contains(SymbolMatchFieldKind.Path)) {
            score += ScoreSearchMatch(matcher, type.Source.Path, 90, 40);
        }

        return score;
    }

    private static int ScoreMemberSearchResult(
        TypeFact type,
        MemberFact member,
        SymbolMatcher matcher,
        string declaration,
        IReadOnlyList<SymbolMatchFieldKind> matchFields) {
        var score = 0;

        if (matchFields.Contains(SymbolMatchFieldKind.DisplayName)) {
            score += ScoreSearchMatch(matcher, member.DisplayName, 480, 340);
        }

        if (matchFields.Contains(SymbolMatchFieldKind.Declaration)) {
            score += ScoreSearchMatch(matcher, declaration, 380, 240);
        }

        if (matchFields.Contains(SymbolMatchFieldKind.Path)) {
            score += ScoreSearchMatch(matcher, member.Source.Path, 90, 40);
        }

        if (type.Kind == TypeKind.Interface) {
            score += 20;
        }

        return score;
    }

    private static SymbolImplementationKind? ResolveImplementationKind(TypeFact targetType, TypeFact candidate) {
        if (candidate.InterfaceDisplayNames.Any(item => string.Equals(item, targetType.DisplayName, StringComparison.Ordinal))) {
            return SymbolImplementationKind.InterfaceImplementation;
        }

        if (!string.IsNullOrWhiteSpace(candidate.BaseTypeDisplayName)
            && string.Equals(candidate.BaseTypeDisplayName, targetType.DisplayName, StringComparison.Ordinal)) {
            return SymbolImplementationKind.DerivedType;
        }

        return null;
    }

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
        CanDoItAll.CodeAnalytics.Domain.Sources.SourceReference? source) {
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

    private sealed record SymbolNames(
        string ProjectName,
        string ModuleName,
        string NamespaceName);

    private sealed record SymbolQueryContext(
        IReadOnlyDictionary<string, TypeFact> TypesById,
        IReadOnlyDictionary<string, MemberFact> MembersById,
        IReadOnlyDictionary<string, IReadOnlyList<MemberFact>> MembersByTypeId,
        IReadOnlyDictionary<string, ProjectFact> ProjectsById,
        IReadOnlyDictionary<string, ModuleFact> ModulesById,
        IReadOnlyDictionary<string, NamespaceFact> NamespacesById,
        IReadOnlyList<string> AvailableProjects);

    private sealed record ScoredSymbolSearchResult(SymbolSearchResultItem Result, int Score);

    private sealed record ScoredSymbolReference(SymbolReferenceItem Reference, int Score);
}
