using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Facts.Services;

public sealed record ServiceRegistrationCollectionResult(
    IReadOnlyList<ServiceRegistrationFact> Services,
    IReadOnlyList<AnalysisDiagnostic> Diagnostics);
