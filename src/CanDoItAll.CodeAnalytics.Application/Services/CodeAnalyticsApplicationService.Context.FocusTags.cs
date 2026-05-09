using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Application.Services;

public sealed partial class CodeAnalyticsApplicationService {
    private static readonly IReadOnlyDictionary<string, string[]> FocusTagKeywords =
        new Dictionary<string, string[]>(StringComparer.Ordinal) {
            ["db"] = ["db", "database", "sql", "entity", "entities", "table", "persistence", "repository", "context", "migration", "ef"],
            ["database"] = ["db", "database", "sql", "entity", "entities", "table", "persistence", "repository", "context", "migration", "ef"],
            ["entityframework"] = ["db", "database", "sql", "entity", "entities", "table", "persistence", "repository", "context", "migration", "ef", "efcore", "entityframework"],
            ["efcore"] = ["db", "database", "sql", "entity", "entities", "table", "persistence", "repository", "context", "migration", "ef", "efcore", "entityframework"],
            ["ui"] = ["ui", "page", "component", "razor", "view", "render", "layout", "web"],
            ["razor"] = ["ui", "page", "component", "razor", "view", "render", "layout", "web"],
            ["component"] = ["ui", "page", "component", "razor", "view", "render", "layout", "web"],
            ["service"] = ["service", "handler", "command", "query", "application"],
            ["domain"] = ["domain", "aggregate", "entity", "value", "model"],
            ["infra"] = ["infra", "infrastructure", "storage", "persistence", "hosting", "adapter"],
            ["test"] = ["test", "spec", "fixture", "mock"],
        };

    private static IReadOnlyList<string> NormalizeFocusTags(IReadOnlyList<string>? focusTags) {
        if (focusTags is null || focusTags.Count == 0) {
            return [];
        }

        return focusTags
            .SelectMany(
                tag => tag.Split(
                    [',', ';', '\r', '\n'],
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Select(NormalizeSearchToken)
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Distinct(StringComparer.Ordinal)
            .ToArray();
    }

    private static IReadOnlyList<string> NormalizeRelationHints(IReadOnlyList<string>? relationHints) {
        if (relationHints is null || relationHints.Count == 0) {
            return [];
        }

        return relationHints
            .SelectMany(
                hint => hint.Split(
                    [',', ';', '\r', '\n'],
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Select(NormalizeSearchToken)
            .Where(hint => !string.IsNullOrWhiteSpace(hint))
            .Distinct(StringComparer.Ordinal)
            .ToArray();
    }

    private static int GetFocusTagScore(IReadOnlyCollection<string> focusTags, params string?[] texts) {
        if (focusTags.Count == 0) {
            return 0;
        }

        var haystack = string.Join(' ', texts.Where(text => !string.IsNullOrWhiteSpace(text)))
            .ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(haystack)) {
            return 0;
        }

        var score = 0;
        foreach (var focusTag in focusTags) {
            if (haystack.Contains(focusTag, StringComparison.Ordinal)) {
                score += 20;
            }

            foreach (var keyword in ExpandFocusTag(focusTag)) {
                if (string.Equals(keyword, focusTag, StringComparison.Ordinal)) {
                    continue;
                }

                if (haystack.Contains(keyword, StringComparison.Ordinal)) {
                    score += 8;
                    break;
                }
            }
        }

        return score;
    }

    private static IEnumerable<string> ExpandFocusTag(string focusTag) {
        return FocusTagKeywords.TryGetValue(focusTag, out var keywords)
            ? keywords
            : [focusTag];
    }

    private static int GetRelationHintScore(IReadOnlyCollection<string> relationHints, params string?[] texts) {
        if (relationHints.Count == 0) {
            return 0;
        }

        var haystack = string.Join(' ', texts.Where(text => !string.IsNullOrWhiteSpace(text)))
            .ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(haystack)) {
            return 0;
        }

        var score = 0;
        foreach (var relationHint in relationHints) {
            if (haystack.Contains(relationHint, StringComparison.Ordinal)) {
                score += 120;
                continue;
            }

            var tokens = ExtractSearchTokens(relationHint).ToArray();
            if (tokens.Length == 0) {
                continue;
            }

            var matchedTokenCount = tokens.Count(token => haystack.Contains(token, StringComparison.Ordinal));
            if (matchedTokenCount == tokens.Length) {
                score += 80 + matchedTokenCount * 12;
            }
            else if (matchedTokenCount > 0) {
                score += matchedTokenCount * 16;
            }
        }

        return score;
    }

    private static string NormalizeSearchToken(string value) {
        return value.Trim().ToLowerInvariant();
    }

    private static string CreateFocusTagText(TypeFact type) {
        return string.Join(
            ' ',
            type.DisplayName,
            type.XmlSummary ?? string.Empty,
            type.Source.Path);
    }
}
