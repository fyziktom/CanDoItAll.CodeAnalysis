using System.Diagnostics;
using CanDoItAll.CodeAnalytics.Abstractions;
using CanDoItAll.CodeAnalytics.Abstractions.Progress;
using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Exports;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Storage.Paths;
using CanDoItAll.CodeAnalytics.Workspace.Loading;
using Microsoft.Extensions.Logging;

namespace CanDoItAll.CodeAnalytics.Application.Services;

public sealed partial class CodeAnalyticsApplicationService {
    private async Task<WorkspaceLoadResult> LoadWorkspaceAsync(
        AnalysisRequest request,
        IList<AnalysisProgressEvent> progressEvents,
        IAnalysisProgressReporter? progressReporter,
        CancellationToken cancellationToken) {
        ReportProgress(progressEvents, progressReporter, "workspace", AnalysisProgressState.Started, "Loading the MSBuild workspace.");

        try {
            var workspace = await _workspaceLoader.LoadAsync(request, cancellationToken);
            var workspaceState = workspace.HasBlockingErrors
                ? AnalysisProgressState.Failed
                : AnalysisProgressState.Completed;
            var workspaceMessage = workspace.RoslynSolution is null
                ? "Workspace load completed without a Roslyn solution."
                : $"Workspace load completed with {workspace.Projects.Count} source projects.";
            ReportProgress(progressEvents, progressReporter, "workspace", workspaceState, workspaceMessage);
            return workspace;
        }
        catch (OperationCanceledException) {
            ReportProgress(progressEvents, progressReporter, "workspace", AnalysisProgressState.Failed, "Workspace loading was canceled.");
            throw;
        }
        catch (Exception exception) {
            _logger.LogError(exception, "Workspace loading failed for {WorkspacePath}", request.SolutionPath);
            ReportProgress(progressEvents, progressReporter, "workspace", AnalysisProgressState.Failed, exception.Message);
            throw;
        }
    }

    private async Task<T> ExecuteStageAsync<T>(
        string stage,
        string description,
        Func<Task<T>> action,
        T fallback,
        ICollection<AnalysisDiagnostic> diagnostics,
        string diagnosticCode,
        IList<AnalysisProgressEvent> progressEvents,
        IAnalysisProgressReporter? progressReporter,
        CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ReportProgress(progressEvents, progressReporter, stage, AnalysisProgressState.Started, description);
        var stopwatch = Stopwatch.StartNew();

        try {
            var result = await action();
            stopwatch.Stop();
            ReportProgress(progressEvents, progressReporter, stage, AnalysisProgressState.Completed, $"{description} Completed in {stopwatch.ElapsedMilliseconds} ms.");
            return result;
        }
        catch (OperationCanceledException) {
            ReportProgress(progressEvents, progressReporter, stage, AnalysisProgressState.Failed, $"{description} Canceled.");
            throw;
        }
        catch (Exception exception) {
            stopwatch.Stop();
            _logger.LogError(exception, "{Stage} failed after {ElapsedMilliseconds} ms", stage, stopwatch.ElapsedMilliseconds);
            diagnostics.Add(
                new AnalysisDiagnostic(
                    diagnosticCode,
                    AnalysisDiagnosticSeverity.Error,
                    $"{description} {exception.Message}"));
            ReportProgress(progressEvents, progressReporter, stage, AnalysisProgressState.Failed, $"{description} Failed after {stopwatch.ElapsedMilliseconds} ms.");
            return fallback;
        }
    }

    private async Task PersistSnapshotAsync(
        SnapshotPathResolver pathResolver,
        ArchitectureSnapshot snapshot,
        string requestHash,
        IReadOnlyList<PreparedExport> exports,
        IList<AnalysisProgressEvent> progressEvents,
        IAnalysisProgressReporter? progressReporter,
        CancellationToken cancellationToken) {
        ReportProgress(progressEvents, progressReporter, "persist", AnalysisProgressState.Started, $"Persisting snapshot {snapshot.SnapshotId}.");

        try {
            await _snapshotRepository.StoreAsync(pathResolver, snapshot, requestHash, exports, cancellationToken);
            ReportProgress(progressEvents, progressReporter, "persist", AnalysisProgressState.Completed, $"Persisted snapshot {snapshot.SnapshotId}.");
        }
        catch (OperationCanceledException) {
            ReportProgress(progressEvents, progressReporter, "persist", AnalysisProgressState.Failed, $"Persisting snapshot {snapshot.SnapshotId} was canceled.");
            throw;
        }
        catch (Exception exception) {
            _logger.LogError(exception, "Persisting snapshot {SnapshotId} failed", snapshot.SnapshotId);
            ReportProgress(progressEvents, progressReporter, "persist", AnalysisProgressState.Failed, exception.Message);
            throw;
        }
    }

    private void ReportProgress(
        IList<AnalysisProgressEvent> progressEvents,
        IAnalysisProgressReporter? progressReporter,
        string stage,
        AnalysisProgressState state,
        string message) {
        var progressEvent = new AnalysisProgressEvent(stage, state, message, DateTimeOffset.UtcNow);
        progressEvents.Add(progressEvent);
        progressReporter?.Report(progressEvent);

        switch (state) {
            case AnalysisProgressState.Failed:
                _logger.LogError("{Stage}: {Message}", stage, message);
                break;
            default:
                _logger.LogInformation("{Stage}: {Message}", stage, message);
                break;
        }
    }
}
