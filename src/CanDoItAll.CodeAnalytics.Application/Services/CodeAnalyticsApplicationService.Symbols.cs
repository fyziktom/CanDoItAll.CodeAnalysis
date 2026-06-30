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

}
