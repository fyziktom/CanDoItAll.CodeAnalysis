using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record SymbolDefinitionResponse(
    string SnapshotId,
    SymbolTargetKind TargetKind,
    string ProjectName,
    string ModuleName,
    string NamespaceName,
    TypeFact Type,
    MemberFact? Member,
    string Declaration,
    string? XmlSummary,
    SymbolSourceExcerpt Definition,
    SymbolSourceExcerpt? ContainingTypeHeader);
