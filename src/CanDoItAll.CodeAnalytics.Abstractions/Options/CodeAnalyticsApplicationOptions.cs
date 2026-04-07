namespace CanDoItAll.CodeAnalytics.Abstractions.Options;

public sealed record CodeAnalyticsApplicationOptions(
    string OutputRootPath,
    string GeneratorVersion,
    int MaxRecentSnapshots = 20,
    int MaxDiagramNodes = 80);
