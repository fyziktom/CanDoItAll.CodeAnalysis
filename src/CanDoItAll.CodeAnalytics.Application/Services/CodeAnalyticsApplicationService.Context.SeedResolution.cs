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
        var exactTypeIds = snapshot.Facts.Types
            .Where(type => IsTypeIdentityQuery(queryText, type))
            .Select(type => type.TypeId)
            .ToHashSet(StringComparer.Ordinal);

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

            if (exactTypeIds.Count > 0 && !exactTypeIds.Contains(type.TypeId)) {
                continue;
            }

            if (member.Kind == MemberKind.Constructor && IsTypeIdentityQuery(queryText, type) && !LooksLikeConstructorQuery(queryText)) {
                continue;
            }

            var score = ScoreSearchText(queryText, member.DisplayName, member.ReturnTypeDisplayName, string.Join(' ', member.ParameterDisplayNames), member.Source.Path);
            if (!IsTypeIdentityQuery(queryText, type)) {
                score += ScoreSearchText(queryText, type.DisplayName, type.XmlSummary);
            }

            if (score <= 0) {
                continue;
            }

            best = SelectHigherScore(
                best,
                new SeedCandidate(
                    type,
                    member,
                    null,
                    score + ScoreSeedMember(type, member, queryText, focusTags),
                    $"Resolved from prompt text to member {member.DisplayName}."));
        }

        foreach (var type in snapshot.Facts.Types) {
            if (ShouldExcludeFromFocusedContext(type, projectsById, null)) {
                continue;
            }

            if (exactTypeIds.Count > 0 && !exactTypeIds.Contains(type.TypeId)) {
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
                    ChooseSeedMember(type, membersByTypeId, queryText, focusTags),
                    null,
                    score + 140 + GetTypeIdentityBoost(queryText, type) + GetFocusTagScore(focusTags, type.DisplayName, type.XmlSummary, type.Source.Path),
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

            if (exactTypeIds.Count > 0 && (type is null || !exactTypeIds.Contains(type.TypeId))) {
                continue;
            }

            best = SelectHigherScore(
                best,
                new SeedCandidate(
                    type,
                    type is null ? null : ChooseSeedMember(type, membersByTypeId, queryText, focusTags),
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
        IReadOnlyDictionary<string, IReadOnlyList<MemberFact>> membersByTypeId,
        string? queryText,
        IReadOnlyCollection<string> focusTags) {
        return ResolveSeedMemberIds(type, null, membersByTypeId, queryText, focusTags)
            .Select(memberId => membersByTypeId[type.TypeId].FirstOrDefault(item => string.Equals(item.MemberId, memberId, StringComparison.Ordinal)))
            .FirstOrDefault(item => item is not null);
    }

    private static int ScoreSeedMember(
        TypeFact type,
        MemberFact member,
        string? queryText,
        IReadOnlyCollection<string> focusTags) {
        var score = member.Kind switch {
            MemberKind.Method => 120,
            MemberKind.Property => 85,
            MemberKind.Constructor => 40,
            MemberKind.Field => 15,
            _ => 0,
        };

        if (type.Kind == TypeKind.Interface && member.Kind == MemberKind.Method && !string.IsNullOrWhiteSpace(queryText) && IsTypeIdentityQuery(queryText, type)) {
            score += 40;
        }

        if (!string.IsNullOrWhiteSpace(queryText)) {
            score += ScoreSearchText(
                queryText,
                member.DisplayName,
                member.ReturnTypeDisplayName,
                string.Join(' ', member.ParameterDisplayNames));

            if (member.Kind == MemberKind.Constructor && IsTypeIdentityQuery(queryText, type)) {
                score -= 220;
            }
            else if (member.Kind == MemberKind.Constructor && !LooksLikeConstructorQuery(queryText)) {
                score -= 50;
            }
        }

        if (type.Kind == TypeKind.Interface && member.Kind == MemberKind.Method && !string.IsNullOrWhiteSpace(queryText) && !IsTypeIdentityQuery(queryText, type)) {
            score -= 60;
        }

        if (IsLowSignalMember(member)) {
            score -= 20;
        }

        score += GetFocusTagScore(
            focusTags,
            GetTrailingIdentifier(member.DisplayName),
            member.ReturnTypeDisplayName,
            string.Join(' ', member.ParameterDisplayNames));

        return score;
    }

    private static int GetTypeIdentityBoost(string queryText, TypeFact type) {
        return IsTypeIdentityQuery(queryText, type)
            ? 180
            : 0;
    }

    private static bool IsTypeIdentityQuery(string queryText, TypeFact type) {
        var normalizedQuery = NormalizeSearchToken(queryText);
        if (string.IsNullOrWhiteSpace(normalizedQuery)) {
            return false;
        }

        return string.Equals(normalizedQuery, NormalizeSearchToken(type.DisplayName), StringComparison.Ordinal)
            || string.Equals(normalizedQuery, NormalizeSearchToken(GetTrailingIdentifier(type.DisplayName)), StringComparison.Ordinal);
    }

    private static bool LooksLikeConstructorQuery(string queryText) {
        return queryText.Contains('(', StringComparison.Ordinal)
            || queryText.Contains(".ctor", StringComparison.OrdinalIgnoreCase)
            || queryText.Contains("constructor", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsLowSignalMember(MemberFact member) {
        return member.DisplayName.Contains("Dispose", StringComparison.Ordinal)
            || member.DisplayName.Contains("Reset", StringComparison.Ordinal);
    }

    private static bool MemberNameMatchesQuery(MemberFact member, string queryText) {
        var normalizedQuery = NormalizeSearchToken(queryText);
        if (string.IsNullOrWhiteSpace(normalizedQuery)) {
            return false;
        }

        return string.Equals(normalizedQuery, NormalizeSearchToken(GetTrailingIdentifier(member.DisplayName)), StringComparison.Ordinal);
    }

    private static string GetTrailingIdentifier(string displayName) {
        var trimmed = displayName.Trim();
        var genericStart = trimmed.IndexOf('<');
        if (genericStart >= 0) {
            trimmed = trimmed[..genericStart];
        }

        var methodStart = trimmed.IndexOf('(');
        if (methodStart >= 0) {
            trimmed = trimmed[..methodStart];
        }

        var lastDot = trimmed.LastIndexOf('.');
        return lastDot >= 0
            ? trimmed[(lastDot + 1)..]
            : trimmed;
    }

    private sealed record SeedCandidate(
        TypeFact? Type,
        MemberFact? Member,
        ServiceRegistrationFact? Service,
        int Score,
        string Explanation);
}
