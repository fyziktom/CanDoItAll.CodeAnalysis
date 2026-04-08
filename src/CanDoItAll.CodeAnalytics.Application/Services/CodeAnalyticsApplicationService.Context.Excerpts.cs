using CanDoItAll.CodeAnalytics.Abstractions.Responses;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Domain.Sources;

namespace CanDoItAll.CodeAnalytics.Application.Services;

public sealed partial class CodeAnalyticsApplicationService {
    private async Task<IReadOnlyList<FocusedContextFileExcerpt>> BuildFocusedContextFilesAsync(
        ArchitectureSnapshot snapshot,
        TypeFact? seedType,
        MemberFact? seedMember,
        IReadOnlyList<TypeFact> selectedTypes,
        IReadOnlyList<MemberFact> selectedMembers,
        IReadOnlyDictionary<string, IReadOnlyList<MemberFact>> membersByTypeId,
        IReadOnlyCollection<string> focusTags,
        CancellationToken cancellationToken) {
        var fileCandidates = CreateExcerptCandidates(seedType, seedMember, selectedTypes, selectedMembers, membersByTypeId, focusTags)
            .GroupBy(item => NormalizePath(item.Source.Path), StringComparer.OrdinalIgnoreCase)
            .OrderByDescending(group => group.Max(item => item.Priority))
            .ThenBy(group => group.Key, StringComparer.Ordinal)
            .Take(MaxFocusedFiles)
            .ToArray();
        if (fileCandidates.Length == 0) {
            return [];
        }

        var workspaceRoot = Path.GetDirectoryName(snapshot.Request.SolutionPath)!;
        var documentLineCounts = snapshot.Facts.Documents
            .GroupBy(item => NormalizePath(item.Path), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => group.Max(item => item.LineCount),
                StringComparer.OrdinalIgnoreCase);
        var typesByPath = selectedTypes
            .GroupBy(item => NormalizePath(item.Source.Path), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => group
                    .OrderBy(item => item.DisplayName, StringComparer.Ordinal)
                    .Select(item => item.DisplayName)
                    .ToArray(),
                StringComparer.OrdinalIgnoreCase);
        var files = new List<FocusedContextFileExcerpt>();

        foreach (var fileGroup in fileCandidates) {
            var absolutePath = ResolveAbsoluteSourcePath(workspaceRoot, fileGroup.Key);
            if (!File.Exists(absolutePath)) {
                continue;
            }

            var lines = await File.ReadAllLinesAsync(absolutePath, cancellationToken);
            var blocks = fileGroup
                .OrderByDescending(item => item.Priority)
                .ThenBy(item => item.Source.Line ?? int.MaxValue)
                .Take(MaxExcerptBlocksPerFile)
                .Select(candidate => CreateExcerptBlock(candidate, lines))
                .Where(item => item is not null)
                .Cast<FocusedContextExcerptBlock>()
                .ToArray();
            if (blocks.Length == 0) {
                continue;
            }

            var totalLineCount = documentLineCounts.TryGetValue(fileGroup.Key, out var lineCount)
                ? lineCount
                : lines.Length;
            files.Add(
                new FocusedContextFileExcerpt(
                    fileGroup.Key,
                    totalLineCount,
                    CalculateSelectedLineCount(blocks),
                    typesByPath.TryGetValue(fileGroup.Key, out var typeDisplayNames)
                        ? typeDisplayNames
                        : [],
                    blocks));
        }

        return files
            .OrderByDescending(item => item.SelectedLineCount)
            .ThenBy(item => item.Path, StringComparer.Ordinal)
            .Take(MaxFocusedFiles)
            .ToArray();
    }

    private static FocusedContextStats BuildFocusedContextStats(IReadOnlyList<FocusedContextFileExcerpt> files) {
        return new FocusedContextStats(
            files.Count,
            files.Sum(item => item.Blocks.Count),
            files.Sum(item => item.SelectedLineCount),
            files.Sum(item => item.TotalLineCount));
    }

    private static IReadOnlyList<ExcerptCandidate> CreateExcerptCandidates(
        TypeFact? seedType,
        MemberFact? seedMember,
        IReadOnlyList<TypeFact> selectedTypes,
        IReadOnlyList<MemberFact> selectedMembers,
        IReadOnlyDictionary<string, IReadOnlyList<MemberFact>> membersByTypeId,
        IReadOnlyCollection<string> focusTags) {
        var candidates = new List<ExcerptCandidate>();

        foreach (var member in selectedMembers.Take(MaxExcerptBlocks)) {
            candidates.Add(
                new ExcerptCandidate(
                    member.Source,
                    member.DisplayName,
                    member.Kind.ToString(),
                    string.Equals(member.MemberId, seedMember?.MemberId, StringComparison.Ordinal)
                        ? 400
                        : string.Equals(member.TypeId, seedType?.TypeId, StringComparison.Ordinal)
                            ? 320
                            : 220));
        }

        var representedPaths = selectedMembers
            .Select(item => NormalizePath(item.Source.Path))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var type in selectedTypes.Where(item => !representedPaths.Contains(NormalizePath(item.Source.Path))).Take(MaxExcerptBlocks)) {
            if (membersByTypeId.TryGetValue(type.TypeId, out var members)) {
                foreach (var member in RankRepresentativeMembers(type, members, focusTags).Take(MaxRepresentativeMembersPerType)) {
                    candidates.Add(
                        new ExcerptCandidate(
                            member.Source,
                            member.DisplayName,
                            member.Kind.ToString(),
                            GetRepresentativeExcerptPriority(type, member, seedType, seedMember)));
                }

                continue;
            }

            candidates.Add(
                new ExcerptCandidate(
                    CreateTypeHeaderSource(type.Source),
                    type.DisplayName,
                    type.Kind.ToString(),
                    string.Equals(type.TypeId, seedType?.TypeId, StringComparison.Ordinal) ? 260 : 150));
        }

        return candidates
            .GroupBy(item => $"{NormalizePath(item.Source.Path)}::{item.Title}::{item.Source.Line}::{item.Kind}", StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .ToArray();
    }

    private static IReadOnlyList<MemberFact> RankRepresentativeMembers(
        TypeFact type,
        IReadOnlyList<MemberFact> members,
        IReadOnlyCollection<string> focusTags) {
        return RankSeedMembers(type, members, type.DisplayName, focusTags)
            .Where(item => item.Kind is MemberKind.Method or MemberKind.Property or MemberKind.Constructor)
            .ToArray();
    }

    private static int GetRepresentativeExcerptPriority(
        TypeFact type,
        MemberFact member,
        TypeFact? seedType,
        MemberFact? seedMember) {
        if (string.Equals(member.MemberId, seedMember?.MemberId, StringComparison.Ordinal)) {
            return 400;
        }

        if (string.Equals(type.TypeId, seedType?.TypeId, StringComparison.Ordinal)) {
            return 280;
        }

        return 170;
    }

    private static SourceReference CreateTypeHeaderSource(SourceReference source) {
        var startLine = source.Line ?? 1;
        var endLine = source.EndLine ?? (startLine + DefaultTypeHeaderLength - 1);
        return source with {
            EndLine = Math.Max(startLine, Math.Min(endLine, startLine + DefaultTypeHeaderLength - 1)),
        };
    }

    private static FocusedContextExcerptBlock? CreateExcerptBlock(ExcerptCandidate candidate, string[] lines) {
        var (startLine, endLine) = CalculateExcerptRange(candidate.Source, lines.Length);
        if (startLine <= 0 || endLine <= 0 || startLine > endLine || startLine > lines.Length) {
            return null;
        }

        var excerpt = string.Join(Environment.NewLine, lines.Skip(startLine - 1).Take(endLine - startLine + 1));
        return new FocusedContextExcerptBlock(candidate.Title, candidate.Kind, startLine, endLine, excerpt);
    }

    private static (int StartLine, int EndLine) CalculateExcerptRange(SourceReference source, int totalLineCount) {
        if (totalLineCount <= 0) {
            return (0, 0);
        }

        var startLine = source.Line ?? 1;
        var endLine = source.EndLine ?? Math.Min(totalLineCount, startLine + DefaultExcerptLength - 1);
        if (endLine < startLine) {
            endLine = startLine;
        }

        startLine = Math.Clamp(startLine, 1, totalLineCount);
        endLine = Math.Clamp(endLine, startLine, totalLineCount);
        return (
            Math.Max(1, startLine - DefaultExcerptPaddingLines),
            Math.Min(totalLineCount, endLine + DefaultExcerptPaddingLines));
    }

    private static int CalculateSelectedLineCount(IReadOnlyList<FocusedContextExcerptBlock> blocks) {
        if (blocks.Count == 0) {
            return 0;
        }

        var orderedBlocks = blocks
            .OrderBy(item => item.StartLine)
            .ThenBy(item => item.EndLine)
            .ToArray();
        var total = 0;
        var currentStart = orderedBlocks[0].StartLine;
        var currentEnd = orderedBlocks[0].EndLine;

        for (var index = 1; index < orderedBlocks.Length; index++) {
            var block = orderedBlocks[index];
            if (block.StartLine <= currentEnd + 1) {
                currentEnd = Math.Max(currentEnd, block.EndLine);
                continue;
            }

            total += currentEnd - currentStart + 1;
            currentStart = block.StartLine;
            currentEnd = block.EndLine;
        }

        total += currentEnd - currentStart + 1;
        return total;
    }

    private static string ResolveAbsoluteSourcePath(string workspaceRoot, string relativePath) {
        return Path.GetFullPath(Path.Combine(workspaceRoot, relativePath.Replace('/', Path.DirectorySeparatorChar)));
    }

    private sealed record ExcerptCandidate(
        SourceReference Source,
        string Title,
        string Kind,
        int Priority);
}
