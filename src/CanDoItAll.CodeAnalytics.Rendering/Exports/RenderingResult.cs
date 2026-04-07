using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Exports;

namespace CanDoItAll.CodeAnalytics.Rendering.Exports;

public sealed record RenderingResult(
    IReadOnlyList<PreparedExport> Exports,
    IReadOnlyList<AnalysisDiagnostic> Diagnostics);
