using CanDoItAll.CodeAnalytics.Abstractions;
using CanDoItAll.CodeAnalytics.Abstractions.Responses;
using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Application.Services;

public sealed partial class CodeAnalyticsApplicationService {
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
}
