namespace CanDoItAll.CodeAnalytics.Domain.Snapshot;

public sealed record AnalysisRequest(
    string SolutionPath,
    IReadOnlyList<string> ScopeProjectNames,
    IReadOnlyList<string> ScopeNamespacePrefixes,
    bool IncludeDi,
    bool IncludePersistence,
    bool IncludeRisks,
    bool IncludeXmlDocs,
    bool IncludeMermaidExports);
