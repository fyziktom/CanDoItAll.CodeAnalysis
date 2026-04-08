using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record SymbolImplementationsResponse(
    string SnapshotId,
    string ProjectName,
    string ModuleName,
    string NamespaceName,
    TypeFact Type,
    IReadOnlyList<SymbolImplementationItem> Implementations);
