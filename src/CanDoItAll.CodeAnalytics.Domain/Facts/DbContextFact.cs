using CanDoItAll.CodeAnalytics.Domain.Sources;

namespace CanDoItAll.CodeAnalytics.Domain.Facts;

public sealed record DbContextFact(
    string DbContextId,
    string TypeId,
    string ProjectId,
    string ModuleId,
    string DisplayName,
    IReadOnlyList<string> EntityTypeIds,
    SourceReference Source);
