using CanDoItAll.CodeAnalytics.Domain.Sources;

namespace CanDoItAll.CodeAnalytics.Domain.Diagnostics;

public sealed record AnalysisDiagnostic(
    string Code,
    AnalysisDiagnosticSeverity Severity,
    string Message,
    SourceReference? Source = null);
