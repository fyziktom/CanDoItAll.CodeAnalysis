using CanDoItAll.CodeAnalytics.Abstractions;
using CanDoItAll.CodeAnalytics.Abstractions.Responses;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Domain.Sources;

namespace CanDoItAll.CodeAnalytics.Application.Services;

public sealed partial class CodeAnalyticsApplicationService {
    private const long MaxSourceReadBytes = 2 * 1024 * 1024;

    private async Task<FocusedContextFileBuildResult> BuildFocusedContextFilesAsync(
        ArchitectureSnapshot snapshot,
        TypeFact? seedType,
        MemberFact? seedMember,
        IReadOnlyList<TypeFact> selectedTypes,
        IReadOnlyList<FocusedContextSelectedMemberContext> selectedMemberContexts,
        IReadOnlyDictionary<string, IReadOnlyList<MemberFact>> membersByTypeId,
        IReadOnlyList<ServiceRegistrationFact> relatedServices,
        IReadOnlyCollection<string> focusTags,
        FocusedContextStrategy strategy,
        CancellationToken cancellationToken) {
        if (!strategy.EmitCodeExcerpts) {
            return new FocusedContextFileBuildResult([], []);
        }

        var fileCandidates = CreateExcerptCandidates(seedType, seedMember, selectedTypes, selectedMemberContexts, membersByTypeId, relatedServices, focusTags, strategy)
            .GroupBy(item => NormalizePath(item.Source.Path), StringComparer.OrdinalIgnoreCase)
            .OrderByDescending(group => group.Max(item => item.Priority))
            .ThenBy(group => group.Key, StringComparer.Ordinal)
            .Take(MaxFocusedFiles)
            .ToArray();
        if (fileCandidates.Length == 0) {
            return new FocusedContextFileBuildResult([], []);
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
        var selectionReasons = new List<FocusedContextSelectionReason>();

        foreach (var fileGroup in fileCandidates) {
            var absolutePath = TryResolveReadableSourcePath(workspaceRoot, fileGroup.Key);
            if (absolutePath is null) {
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
            selectionReasons.AddRange(
                fileGroup
                    .Select(
                        item => new FocusedContextSelectionReason(
                            FocusedContextSelectionTargetKind.File,
                            fileGroup.Key,
                            item.ReasonKind,
                            item.RoleKind))
                    .Distinct());
        }

        return new FocusedContextFileBuildResult(
            files
            .OrderByDescending(item => item.SelectedLineCount)
            .ThenBy(item => item.Path, StringComparer.Ordinal)
            .Take(MaxFocusedFiles)
            .ToArray(),
            selectionReasons
                .Distinct()
                .ToArray());
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
        IReadOnlyList<FocusedContextSelectedMemberContext> selectedMemberContexts,
        IReadOnlyDictionary<string, IReadOnlyList<MemberFact>> membersByTypeId,
        IReadOnlyList<ServiceRegistrationFact> relatedServices,
        IReadOnlyCollection<string> focusTags,
        FocusedContextStrategy strategy) {
        var candidates = new List<ExcerptCandidate>();
        var implementationTypeIds = selectedTypes
            .Where(item => !string.Equals(item.TypeId, seedType?.TypeId, StringComparison.Ordinal))
            .Select(item => item.TypeId)
            .ToHashSet(StringComparer.Ordinal);

        foreach (var memberContext in selectedMemberContexts.Take(MaxExcerptBlocks)) {
            candidates.Add(
                new ExcerptCandidate(
                    memberContext.ExcerptSource,
                    memberContext.Member.DisplayName,
                    memberContext.Member.Kind.ToString(),
                    memberContext.Priority,
                    memberContext.ReasonKind,
                    memberContext.RoleKind));
        }

        var representedPaths = selectedMemberContexts
            .Select(item => NormalizePath(item.Member.Source.Path))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var type in selectedTypes.Where(item => !representedPaths.Contains(NormalizePath(item.Source.Path))).Take(MaxExcerptBlocks)) {
            if (membersByTypeId.TryGetValue(type.TypeId, out var members)) {
                foreach (var member in RankRepresentativeMembers(type, members, focusTags).Take(MaxRepresentativeMembersPerType)) {
                    var reasonKind = ResolveTypeExcerptReasonKind(type, seedType, implementationTypeIds);
                    candidates.Add(
                        new ExcerptCandidate(
                            member.Source,
                            member.DisplayName,
                            member.Kind.ToString(),
                            GetRepresentativeExcerptPriority(type, member, seedType, seedMember),
                            reasonKind,
                            ClassifyReferenceRole(member, type, null)));
                }

                continue;
            }

            var typeReasonKind = ResolveTypeExcerptReasonKind(type, seedType, implementationTypeIds);
            candidates.Add(
                new ExcerptCandidate(
                    CreateTypeHeaderSource(type.Source),
                    type.DisplayName,
                    type.Kind.ToString(),
                    string.Equals(type.TypeId, seedType?.TypeId, StringComparison.Ordinal) ? 260 : 150,
                    typeReasonKind,
                    FocusedContextReferenceRoleKind.None));
        }

        if (strategy.ResolvedIntent == FocusedContextIntent.TroublePath) {
            foreach (var service in relatedServices.Take(2)) {
                candidates.Add(
                    new ExcerptCandidate(
                        service.Source,
                        CreateServiceExcerptTitle(service),
                        nameof(ServiceRegistrationFact),
                        250 + GetRoleScoreBonus(FocusedContextReferenceRoleKind.Registration),
                        FocusedContextSelectionReasonKind.ServiceRegistration,
                        FocusedContextReferenceRoleKind.Registration));
            }
        }

        return candidates
            .GroupBy(item => $"{NormalizePath(item.Source.Path)}::{item.Title}::{item.Source.Line}::{item.Kind}", StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .ToArray();
    }

    private static FocusedContextSelectionReasonKind ResolveTypeExcerptReasonKind(
        TypeFact type,
        TypeFact? seedType,
        ISet<string> implementationTypeIds) {
        if (string.Equals(type.TypeId, seedType?.TypeId, StringComparison.Ordinal)) {
            return FocusedContextSelectionReasonKind.SeedContext;
        }

        if (implementationTypeIds.Contains(type.TypeId)) {
            return FocusedContextSelectionReasonKind.Implementation;
        }

        return FocusedContextSelectionReasonKind.RelatedContext;
    }

    private static string CreateServiceExcerptTitle(ServiceRegistrationFact service) {
        return string.IsNullOrWhiteSpace(service.ImplementationTypeDisplayName)
            ? service.ServiceTypeDisplayName
            : $"{service.ServiceTypeDisplayName} -> {service.ImplementationTypeDisplayName}";
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

    private static string? TryResolveReadableSourcePath(string workspaceRoot, string relativePath) {
        var workspaceRootPath = Path.GetFullPath(workspaceRoot);
        var absolutePath = Path.GetFullPath(
            Path.Combine(workspaceRootPath, relativePath.Replace('/', Path.DirectorySeparatorChar)));
        if (!IsPathWithinDirectory(absolutePath, workspaceRootPath) || !File.Exists(absolutePath)) {
            return null;
        }

        return new FileInfo(absolutePath).Length <= MaxSourceReadBytes
            ? absolutePath
            : null;
    }

    private static bool IsPathWithinDirectory(string candidatePath, string directoryPath) {
        var comparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;
        var directory = Path.TrimEndingDirectorySeparator(Path.GetFullPath(directoryPath));
        var candidate = Path.GetFullPath(candidatePath);

        return string.Equals(candidate, directory, comparison)
            || candidate.StartsWith(directory + Path.DirectorySeparatorChar, comparison);
    }

    private sealed record ExcerptCandidate(
        SourceReference Source,
        string Title,
        string Kind,
        int Priority,
        FocusedContextSelectionReasonKind ReasonKind,
        FocusedContextReferenceRoleKind RoleKind);

    private sealed record FocusedContextFileBuildResult(
        IReadOnlyList<FocusedContextFileExcerpt> Files,
        IReadOnlyList<FocusedContextSelectionReason> SelectionReasons);
}
