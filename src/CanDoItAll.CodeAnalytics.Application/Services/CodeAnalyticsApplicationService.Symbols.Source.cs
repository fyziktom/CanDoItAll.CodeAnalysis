using System.Text.RegularExpressions;
using CanDoItAll.CodeAnalytics.Abstractions;
using CanDoItAll.CodeAnalytics.Abstractions.Responses;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Domain.Sources;

namespace CanDoItAll.CodeAnalytics.Application.Services;

public sealed partial class CodeAnalyticsApplicationService {
    private const int MaxTypeDefinitionLines = 160;
    private const int MaxInterfaceDefinitionLines = 80;
    private const int MaxMemberDefinitionLines = 120;
    private const int SymbolContextPaddingLines = 1;
    private const int SymbolContextMaxLines = 5;
    private const int SymbolTypeHeaderLines = 10;
    private static readonly TimeSpan SymbolRegexTimeout = TimeSpan.FromMilliseconds(100);

    private static bool TryCreateSymbolMatcher(
        string searchText,
        SymbolSearchMode searchMode,
        out SymbolMatcher matcher,
        out string? validationError) {
        var trimmedSearchText = searchText.Trim();
        if (searchMode == SymbolSearchMode.Regex) {
            try {
                matcher = new SymbolMatcher(
                    trimmedSearchText,
                    NormalizeSearchToken(trimmedSearchText),
                    searchMode,
                    new Regex(
                        trimmedSearchText,
                        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
                        SymbolRegexTimeout));
                validationError = null;
                return true;
            }
            catch (ArgumentException exception) {
                matcher = null!;
                validationError = exception.Message;
                return false;
            }
        }

        matcher = new SymbolMatcher(
            trimmedSearchText,
            NormalizeSearchToken(trimmedSearchText),
            searchMode,
            null);
        validationError = null;
        return true;
    }

    private static IReadOnlyList<SymbolMatchFieldKind> CollectSymbolMatchFields(
        SymbolMatcher matcher,
        params (string? Value, SymbolMatchFieldKind Field)[] candidates) {
        return candidates
            .Where(candidate => MatchesSearch(matcher, candidate.Value))
            .Select(candidate => candidate.Field)
            .Distinct()
            .ToArray();
    }

    private static int ScoreSearchMatch(
        SymbolMatcher matcher,
        string? value,
        int exactWeight,
        int containsWeight) {
        if (string.IsNullOrWhiteSpace(value)) {
            return 0;
        }

        if (matcher.SearchMode == SymbolSearchMode.Exact) {
            return MatchesExactSearch(matcher, value)
                ? exactWeight
                : 0;
        }

        return MatchesSearch(matcher, value)
            ? containsWeight
            : 0;
    }

    private static bool MatchesSearch(SymbolMatcher matcher, string? value) {
        if (string.IsNullOrWhiteSpace(value)) {
            return false;
        }

        return matcher.SearchMode switch {
            SymbolSearchMode.Exact => MatchesExactSearch(matcher, value),
            SymbolSearchMode.Regex => matcher.IsRegexMatch(value),
            _ => MatchesContainsSearch(matcher, value),
        };
    }

    private static bool MatchesExactSearch(SymbolMatcher matcher, string value) {
        return string.Equals(value.Trim(), matcher.SearchText, StringComparison.OrdinalIgnoreCase)
            || string.Equals(NormalizeSearchToken(value), matcher.NormalizedSearchText, StringComparison.Ordinal)
            || string.Equals(NormalizeSearchToken(GetTrailingIdentifier(value)), matcher.NormalizedSearchText, StringComparison.Ordinal);
    }

    private static bool MatchesContainsSearch(SymbolMatcher matcher, string value) {
        return value.Contains(matcher.SearchText, StringComparison.OrdinalIgnoreCase)
            || NormalizeSearchToken(value).Contains(matcher.NormalizedSearchText, StringComparison.Ordinal);
    }

    private static bool ShouldIncludeDeclarationField(SymbolSearchMode searchMode, string? searchText) {
        if (searchMode != SymbolSearchMode.Exact || string.IsNullOrWhiteSpace(searchText)) {
            return true;
        }

        return searchText.IndexOfAny([' ', '(', ')', ':']) >= 0;
    }

    private static string BuildTypeDeclaration(TypeFact type) {
        var keyword = type.Kind switch {
            TypeKind.Interface => "interface",
            TypeKind.Struct => "struct",
            TypeKind.Record => "record",
            TypeKind.Enum => "enum",
            TypeKind.Delegate => "delegate",
            _ => "class",
        };
        var dependencies = new List<string>();
        if (!string.IsNullOrWhiteSpace(type.BaseTypeDisplayName)) {
            dependencies.Add(type.BaseTypeDisplayName);
        }

        dependencies.AddRange(type.InterfaceDisplayNames);
        return dependencies.Count == 0
            ? $"{keyword} {type.DisplayName}"
            : $"{keyword} {type.DisplayName} : {string.Join(", ", dependencies)}";
    }

    private static string BuildMemberDeclaration(MemberFact member) {
        return member.DisplayName;
    }

    private static int BuildTypeDefinitionLineLimit(TypeFact type) {
        return type.Kind == TypeKind.Interface
            ? MaxInterfaceDefinitionLines
            : MaxTypeDefinitionLines;
    }

    private async Task<SymbolSourceExcerpt?> CreateSymbolDefinitionExcerptAsync(
        ArchitectureSnapshot snapshot,
        SourceReference source,
        int lineLimit,
        CancellationToken cancellationToken) {
        return await CreateSymbolExcerptAsync(
            snapshot,
            source,
            paddingBefore: 0,
            paddingAfter: 0,
            maxLines: lineLimit,
            cancellationToken);
    }

    private async Task<SymbolSourceExcerpt?> CreateContainingTypeHeaderAsync(
        ArchitectureSnapshot snapshot,
        SourceReference source,
        CancellationToken cancellationToken) {
        return await CreateSymbolExcerptAsync(
            snapshot,
            source with {
                EndLine = source.Line.HasValue
                    ? source.Line.Value + SymbolTypeHeaderLines - 1
                    : SymbolTypeHeaderLines,
            },
            paddingBefore: 0,
            paddingAfter: 0,
            maxLines: SymbolTypeHeaderLines,
            cancellationToken);
    }

    private async Task<SymbolSourceExcerpt?> CreateSymbolContextExcerptAsync(
        ArchitectureSnapshot snapshot,
        SourceReference source,
        CancellationToken cancellationToken) {
        return await CreateSymbolExcerptAsync(
            snapshot,
            source,
            paddingBefore: SymbolContextPaddingLines,
            paddingAfter: SymbolContextPaddingLines,
            maxLines: SymbolContextMaxLines,
            cancellationToken);
    }

    private async Task<SymbolSourceExcerpt?> CreateSymbolExcerptAsync(
        ArchitectureSnapshot snapshot,
        SourceReference source,
        int paddingBefore,
        int paddingAfter,
        int maxLines,
        CancellationToken cancellationToken) {
        var workspaceRoot = Path.GetDirectoryName(snapshot.Request.SolutionPath)!;
        var relativePath = NormalizePath(source.Path);
        var absolutePath = TryResolveReadableSourcePath(workspaceRoot, relativePath);
        if (absolutePath is null) {
            return null;
        }

        var lines = await File.ReadAllLinesAsync(absolutePath, cancellationToken);
        if (lines.Length == 0) {
            return null;
        }

        var startLine = Math.Clamp((source.Line ?? 1) - paddingBefore, 1, lines.Length);
        var endLine = Math.Clamp((source.EndLine ?? source.Line ?? startLine) + paddingAfter, startLine, lines.Length);
        var isTruncated = false;
        var selectedLineCount = endLine - startLine + 1;
        if (selectedLineCount > maxLines) {
            endLine = startLine + maxLines - 1;
            isTruncated = true;
        }

        return new SymbolSourceExcerpt(
            relativePath,
            startLine,
            endLine,
            string.Join(Environment.NewLine, lines.Skip(startLine - 1).Take(endLine - startLine + 1)),
            isTruncated);
    }

    private sealed class SymbolMatcher {
        public SymbolMatcher(
            string searchText,
            string normalizedSearchText,
            SymbolSearchMode searchMode,
            Regex? pattern) {
            SearchText = searchText;
            NormalizedSearchText = normalizedSearchText;
            SearchMode = searchMode;
            Pattern = pattern;
        }

        public string SearchText { get; }

        public string NormalizedSearchText { get; }

        public SymbolSearchMode SearchMode { get; }

        public Regex? Pattern { get; }

        private bool RegexTimedOut { get; set; }

        public bool IsRegexMatch(string value) {
            if (Pattern is null || RegexTimedOut) {
                return false;
            }

            try {
                return Pattern.IsMatch(value);
            }
            catch (RegexMatchTimeoutException) {
                RegexTimedOut = true;
                return false;
            }
        }
    }
}
