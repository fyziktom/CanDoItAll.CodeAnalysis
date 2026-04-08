using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Application.Services;

public sealed partial class CodeAnalyticsApplicationService {
    private static readonly IReadOnlyDictionary<string, string[]> FocusTagKeywords =
        new Dictionary<string, string[]>(StringComparer.Ordinal) {
            ["db"] = ["db", "database", "sql", "entity", "entities", "table", "persistence", "repository", "context", "migration", "ef"],
            ["ui"] = ["ui", "page", "component", "razor", "view", "render", "layout", "web"],
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
