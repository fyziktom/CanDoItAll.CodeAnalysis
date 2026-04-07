using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record ServiceViewResponse(
    string SnapshotId,
    string? SearchText,
    IReadOnlyList<ServiceRegistrationFact> Services,
    IReadOnlyList<AnalysisDiagnostic> Diagnostics);
