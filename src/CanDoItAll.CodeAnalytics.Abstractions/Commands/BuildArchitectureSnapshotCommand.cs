namespace CanDoItAll.CodeAnalytics.Abstractions.Commands;

public sealed record BuildArchitectureSnapshotCommand(
    string SolutionPath,
    IReadOnlyList<string>? ScopeProjectNames = null,
    IReadOnlyList<string>? ScopeNamespacePrefixes = null,
    bool IncludeDi = true,
    bool IncludePersistence = true,
    bool IncludeRisks = true,
    bool IncludeXmlDocs = true,
    bool IncludeMermaidExports = true,
    bool ForceRefresh = false);
