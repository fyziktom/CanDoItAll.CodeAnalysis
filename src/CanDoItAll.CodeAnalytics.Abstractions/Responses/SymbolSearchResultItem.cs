namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record SymbolSearchResultItem(
    SymbolTargetKind TargetKind,
    string ProjectName,
    string ModuleName,
    string NamespaceName,
    string Path,
    int? Line,
    string DisplayName,
    string Declaration,
    string? ContainerTypeDisplayName,
    string TypeId,
    string? MemberId,
    IReadOnlyList<SymbolMatchFieldKind> MatchFields);
