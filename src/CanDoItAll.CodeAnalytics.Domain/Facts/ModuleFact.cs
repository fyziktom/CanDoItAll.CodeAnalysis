namespace CanDoItAll.CodeAnalytics.Domain.Facts;

public sealed record ModuleFact(
    string ModuleId,
    string ProjectId,
    string Name,
    string NamespacePrefix,
    IReadOnlyList<string> NamespaceIds,
    IReadOnlyList<string> TypeIds);
