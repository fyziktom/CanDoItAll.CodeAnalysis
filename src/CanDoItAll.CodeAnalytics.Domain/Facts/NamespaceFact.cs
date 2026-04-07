namespace CanDoItAll.CodeAnalytics.Domain.Facts;

public sealed record NamespaceFact(
    string NamespaceId,
    string ProjectId,
    string ModuleId,
    string Name,
    IReadOnlyList<string> TypeIds);
