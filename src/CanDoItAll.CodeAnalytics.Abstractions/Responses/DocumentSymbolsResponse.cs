using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record DocumentSymbolsResponse(
    string SnapshotId,
    string ProjectName,
    DocumentFact Document,
    IReadOnlyList<TypeSearchResultItem> Types);
