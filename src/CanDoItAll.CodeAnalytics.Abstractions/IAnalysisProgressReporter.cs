using CanDoItAll.CodeAnalytics.Abstractions.Progress;

namespace CanDoItAll.CodeAnalytics.Abstractions;

public interface IAnalysisProgressReporter {
    void Report(AnalysisProgressEvent progressEvent);
}
