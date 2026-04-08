namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record FocusedContextUsageCluster(
    string ProjectId,
    string ProjectName,
    string? ModuleId,
    string? ModuleName,
    int CallerCount,
    IReadOnlyList<FocusedContextUsageSample> Samples);
