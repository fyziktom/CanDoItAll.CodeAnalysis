using CanDoItAll.CodeAnalytics.Domain.Sources;

namespace CanDoItAll.CodeAnalytics.Domain.Facts;

public sealed record TypeFact(
    string TypeId,
    string ProjectId,
    string ModuleId,
    string NamespaceId,
    string DisplayName,
    TypeKind Kind,
    string? BaseTypeDisplayName,
    IReadOnlyList<string> InterfaceDisplayNames,
    IReadOnlyList<string> MemberIds,
    string? XmlSummary,
    SourceReference Source);
