using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record SymbolReferenceItem(
    SymbolReferenceKind Kind,
    string ProjectName,
    string ModuleName,
    string NamespaceName,
    TypeFact SourceType,
    MemberFact? SourceMember,
    string Path,
    int? Line,
    SymbolSourceExcerpt ContextExcerpt);
