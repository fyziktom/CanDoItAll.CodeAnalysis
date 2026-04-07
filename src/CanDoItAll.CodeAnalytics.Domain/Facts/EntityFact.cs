using CanDoItAll.CodeAnalytics.Domain.Sources;

namespace CanDoItAll.CodeAnalytics.Domain.Facts;

public sealed record EntityFact(
    string EntityId,
    string TypeId,
    string ProjectId,
    string ModuleId,
    string DisplayName,
    string? TableName,
    string? Schema,
    IReadOnlyList<string> KeyPropertyNames,
    IReadOnlyList<string> RelationshipTargets,
    SourceReference Source);
