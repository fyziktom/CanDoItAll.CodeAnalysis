using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record SymbolReferencesResponse(
    string SnapshotId,
    SymbolTargetKind TargetKind,
    TypeFact Type,
    MemberFact? Member,
    int TotalCount,
    IReadOnlyList<SymbolReferenceItem> References);
