using CanDoItAll.CodeAnalytics.Domain.Diagnostics;

namespace CanDoItAll.CodeAnalytics.Facts.Documentation;

public sealed record XmlDocumentationResult(
    string? Summary,
    IReadOnlyList<AnalysisDiagnostic> Diagnostics);
