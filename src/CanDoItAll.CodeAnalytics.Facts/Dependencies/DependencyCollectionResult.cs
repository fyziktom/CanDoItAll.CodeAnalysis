using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Facts.Dependencies;

public sealed record DependencyCollectionResult(
    IReadOnlyList<ModuleFact> Modules,
    IReadOnlyList<DependencyEdgeFact> Dependencies,
    IReadOnlyList<AnalysisDiagnostic> Diagnostics);
