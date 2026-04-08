using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record DocumentSourceResponse(
    string SnapshotId,
    string ProjectName,
    DocumentFact Document,
    string SourceCode);
