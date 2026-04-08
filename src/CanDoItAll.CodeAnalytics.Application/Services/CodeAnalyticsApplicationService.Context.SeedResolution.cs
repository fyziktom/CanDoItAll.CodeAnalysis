using CanDoItAll.CodeAnalytics.Abstractions.Queries;
using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Domain.Sources;

namespace CanDoItAll.CodeAnalytics.Application.Services;

public sealed partial class CodeAnalyticsApplicationService {
    private static readonly char[] SearchTokenSeparators = [' ', '\t', '\r', '\n', '.', ':', ',', ';', '(', ')', '[', ']', '{', '}', '<', '>', '-', '_', '/', '\\', '"', '\''];

    private static ResolvedFocusedContextSeed ResolveFocusedContextSeed(
        FocusedContextQuery query,
        ArchitectureSnapshot snapshot,
        IReadOnlyDictionary<string, TypeFact> typesById,
        IReadOnlyDictionary<string, MemberFact> membersById,
        IReadOnlyDictionary<string, ServiceRegistrationFact> servicesById,
        IReadOnlyDictionary<string, ProjectFact> projectsById,
        IReadOnlyDictionary<string, IReadOnlyList<MemberFact>> membersByTypeId,
        IReadOnlyCollection<string> focusTags) {
        var explicitSeedType = !string.IsNullOrWhiteSpace(query.TypeId) && typesById.TryGetValue(query.TypeId, out var resolvedType)
            ? resolvedType
            : null;
        var explicitSeedMember = !string.IsNullOrWhiteSpace(query.MemberId) && membersById.TryGetValue(query.MemberId, out var resolvedMember)
            ? resolvedMember
            : null;
        var explicitSeedService = !string.IsNullOrWhiteSpace(query.ServiceRegistrationId) && servicesById.TryGetValue(query.ServiceRegistrationId, out var resolvedService)
            ? resolvedService
            : null;

        if (explicitSeedService is not null && explicitSeedType is null) {
            explicitSeedType = ResolveTypeForService(explicitSeedService, snapshot.Facts.Types);
        }

        if (explicitSeedMember is not null && explicitSeedType is null && typesById.TryGetValue(explicitSeedMember.TypeId, out var memberType)) {
            explicitSeedType = memberType;
        }

        if (explicitSeedType is not null || explicitSeedMember is not null || explicitSeedService is not null) {
            return new ResolvedFocusedContextSeed(
                explicitSeedType,
                explicitSeedMember,
                explicitSeedService,
                explicitSeedMember is not null
                    ? "Resolved from explicit member id."
                    : explicitSeedType is not null && explicitSeedService is null
                        ? "Resolved from explicit type id."
                        : "Resolved from explicit service id.");
        }

        if (string.IsNullOrWhiteSpace(query.QueryText)) {
            return new ResolvedFocusedContextSeed(null, null, null, null);
        }

        var bestCandidate = FindBestSeedCandidate(query.QueryText, snapshot, projectsById, membersByTypeId, focusTags);
        return bestCandidate is null
            ? new ResolvedFocusedContextSeed(null, null, null, null)
            : new ResolvedFocusedContextSeed(
                bestCandidate.Type,
                bestCandidate.Member,
                bestCandidate.Service,
                bestCandidate.Explanation);
    }

    private static SeedCandidate? FindBestSeedCandidate(
        string queryText,
        ArchitectureSnapshot snapshot,
        IReadOnlyDictionary<string, ProjectFact> projectsById,
        IReadOnlyDictionary<string, IReadOnlyList<MemberFact>> membersByTypeId,
        IReadOnlyCollection<string> focusTags) {
        SeedCandidate? best = null;

        foreach (var diagnostic in snapshot.Diagnostics) {
            var score = ScoreSearchText(queryText, diagnostic.Code, diagnostic.Message, diagnostic.Source?.Path);
            if (score <= 0 || diagnostic.Source is null) {
                continue;
            }

            var member = FindMemberBySource(snapshot.Facts.Members, diagnostic.Source.Path, diagnostic.Source.Line);
            if (member is not null) {
                var type = snapshot.Facts.Types.FirstOrDefault(item => string.Equals(item.TypeId, member.TypeId, StringComparison.Ordinal));
                if (type is not null && !ShouldExcludeFromFocusedContext(type, projectsById, null)) {
                    var candidateScore = score + 260 + GetFocusTagScore(
                        focusTags,
                        member.DisplayName,
                        member.ReturnTypeDisplayName,
                        type.DisplayName,
                        type.Source.Path);
                    best = SelectHigherScore(
                        best,
                        new SeedCandidate(
                            type,
                            member,
                            null,
                            candidateScore,
                            $"Resolved from diagnostic {diagnostic.Code} near {diagnostic.Source.Path}:{diagnostic.Source.Line}."));
                }
            }
            else {
                var type = FindTypeBySource(snapshot.Facts.Types, diagnostic.Source.Path, diagnostic.Source.Line);
                if (type is not null && !ShouldExcludeFromFocusedContext(type, projectsById, null)) {
                    var candidateScore = score + 220 + GetFocusTagScore(focusTags, type.DisplayName, type.XmlSummary, type.Source.Path);
                    best = SelectHigherScore(
                        best,
                        new SeedCandidate(
                            type,
                            null,
                            null,
                            candidateScore,
                            $"Resolved from diagnostic {diagnostic.Code} near {diagnostic.Source.Path}:{diagnostic.Source.Line}."));
                }
            }
        }

        foreach (var member in snapshot.Facts.Members) {
            var type = snapshot.Facts.Types.FirstOrDefault(item => string.Equals(item.TypeId, member.TypeId, StringComparison.Ordinal));
            if (type is null || ShouldExcludeFromFocusedContext(type, projectsById, null)) {
                continue;
            }

            var score = ScoreSearchText(
                queryText,
                member.DisplayName,
                member.ReturnTypeDisplayName,
                string.Join(' ', member.ParameterDisplayNames),
                member.Source.Path,
                type.DisplayName,
                type.XmlSummary);
            if (score <= 0) {
                continue;
            }

            best = SelectHigherScore(
                best,
                new SeedCandidate(
                    type,
                    member,
                    null,
                    score + 180 + GetFocusTagScore(
                        focusTags,
                        member.DisplayName,
                        member.ReturnTypeDisplayName,
                        string.Join(' ', member.ParameterDisplayNames),
                        type.DisplayName,
                        type.Source.Path),
                    $"Resolved from prompt text to member {member.DisplayName}."));
        }

        foreach (var type in snapshot.Facts.Types) {
            if (ShouldExcludeFromFocusedContext(type, projectsById, null)) {
                continue;
            }

            var score = ScoreSearchText(queryText, type.DisplayName, type.XmlSummary, type.Source.Path);
            if (score <= 0) {
                continue;
            }

            best = SelectHigherScore(
                best,
                new SeedCandidate(
                    type,
                    ChooseSeedMember(type, membersByTypeId),
                    null,
                    score + 140 + GetFocusTagScore(focusTags, type.DisplayName, type.XmlSummary, type.Source.Path),
                    $"Resolved from prompt text to type {type.DisplayName}."));
        }

        foreach (var service in snapshot.Facts.ServiceRegistrations) {
            var score = ScoreSearchText(
                queryText,
                service.ServiceTypeDisplayName,
                service.ImplementationTypeDisplayName,
                service.Source.Path);
            if (score <= 0) {
                continue;
            }

            var type = ResolveTypeForService(service, snapshot.Facts.Types);
            if (type is not null && ShouldExcludeFromFocusedContext(type, projectsById, null)) {
                continue;
            }

            best = SelectHigherScore(
                best,
                new SeedCandidate(
                    type,
                    type is null ? null : ChooseSeedMember(type, membersByTypeId),
                    service,
                    score + 120 + GetFocusTagScore(
                        focusTags,
                        service.ServiceTypeDisplayName,
                        service.ImplementationTypeDisplayName,
                        service.Source.Path,
                        type?.DisplayName),
                    $"Resolved from prompt text to service {service.ServiceTypeDisplayName}."));
        }

        return best;
    }

    private static SeedCandidate? SelectHigherScore(SeedCandidate? current, SeedCandidate candidate) {
        if (current is null) {
            return candidate;
        }

        return candidate.Score > current.Score
            ? candidate
            : current;
    }

    private static int ScoreSearchText(string queryText, params string?[] values) {
        var trimmedQuery = queryText.Trim();
        if (string.IsNullOrWhiteSpace(trimmedQuery)) {
            return 0;
        }

        var normalizedQuery = NormalizeSearchToken(trimmedQuery);
        var haystacks = values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value!.Trim())
            .ToArray();
        if (haystacks.Length == 0) {
            return 0;
        }

        var score = 0;
        foreach (var haystack in haystacks) {
            if (string.Equals(haystack, trimmedQuery, StringComparison.OrdinalIgnoreCase)) {
                score += 220;
            }

            if (haystack.Contains(trimmedQuery, StringComparison.OrdinalIgnoreCase)) {
                score += 120;
            }

            var normalizedHaystack = NormalizeSearchToken(haystack);
            if (normalizedHaystack.Contains(normalizedQuery, StringComparison.Ordinal)) {
                score += 80;
            }

            var matchedTokens = 0;
            foreach (var token in ExtractSearchTokens(trimmedQuery)) {
                if (normalizedHaystack.Contains(token, StringComparison.Ordinal)) {
                    matchedTokens++;
                    score += token.Length >= 5 ? 18 : 10;
                }
            }

            if (matchedTokens > 1) {
                score += matchedTokens * 6;
            }
        }

        return score;
    }

    private static IEnumerable<string> ExtractSearchTokens(string queryText) {
        return queryText
            .Split(SearchTokenSeparators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(NormalizeSearchToken)
            .Where(token => token.Length >= 3)
            .Distinct(StringComparer.Ordinal);
    }

    private static MemberFact? FindMemberBySource(
        IReadOnlyList<MemberFact> members,
        string path,
        int? line) {
        return members
            .Where(item => string.Equals(NormalizePath(item.Source.Path), NormalizePath(path), StringComparison.OrdinalIgnoreCase))
            .OrderBy(item => line.HasValue && SourceContainsLine(item.Source, line.Value) ? 0 : 1)
            .ThenBy(item => line.HasValue && item.Source.Line.HasValue ? Math.Abs(item.Source.Line.Value - line.Value) : int.MaxValue)
            .ThenBy(item => item.DisplayName, StringComparer.Ordinal)
            .FirstOrDefault();
    }

    private static TypeFact? FindTypeBySource(
        IReadOnlyList<TypeFact> types,
        string path,
        int? line) {
        return types
            .Where(item => string.Equals(NormalizePath(item.Source.Path), NormalizePath(path), StringComparison.OrdinalIgnoreCase))
            .OrderBy(item => line.HasValue && SourceContainsLine(item.Source, line.Value) ? 0 : 1)
            .ThenBy(item => line.HasValue && item.Source.Line.HasValue ? Math.Abs(item.Source.Line.Value - line.Value) : int.MaxValue)
            .ThenBy(item => item.DisplayName, StringComparer.Ordinal)
            .FirstOrDefault();
    }

    private static bool SourceContainsLine(SourceReference source, int line) {
        if (!source.Line.HasValue) {
            return false;
        }

        var endLine = source.EndLine ?? source.Line.Value;
        return line >= source.Line.Value && line <= endLine;
    }

    private static MemberFact? ChooseSeedMember(
        TypeFact type,
        IReadOnlyDictionary<string, IReadOnlyList<MemberFact>> membersByTypeId) {
        return ResolveSeedMemberIds(type, null, membersByTypeId)
            .Select(memberId => membersByTypeId[type.TypeId].FirstOrDefault(item => string.Equals(item.MemberId, memberId, StringComparison.Ordinal)))
            .FirstOrDefault(item => item is not null);
    }

    private sealed record SeedCandidate(
        TypeFact? Type,
        MemberFact? Member,
        ServiceRegistrationFact? Service,
        int Score,
        string Explanation);
}
