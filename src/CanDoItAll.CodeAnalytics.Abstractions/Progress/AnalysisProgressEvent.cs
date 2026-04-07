namespace CanDoItAll.CodeAnalytics.Abstractions.Progress;

public sealed record AnalysisProgressEvent(
    string Stage,
    AnalysisProgressState State,
    string Message,
    DateTimeOffset OccurredUtc);
