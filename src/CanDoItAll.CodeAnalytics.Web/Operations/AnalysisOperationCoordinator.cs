using System.Collections.Concurrent;
using CanDoItAll.CodeAnalytics.Abstractions;
using CanDoItAll.CodeAnalytics.Abstractions.Commands;
using CanDoItAll.CodeAnalytics.Abstractions.Progress;
using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using Microsoft.Extensions.Logging;

namespace CanDoItAll.CodeAnalytics.Web.Operations;

public sealed class AnalysisOperationCoordinator {
    private readonly ConcurrentDictionary<string, TrackedOperation> _operations = new(StringComparer.Ordinal);
    private readonly ICodeAnalyticsApplicationService _applicationService;
    private readonly ILogger<AnalysisOperationCoordinator> _logger;

    public AnalysisOperationCoordinator(
        ICodeAnalyticsApplicationService applicationService,
        ILogger<AnalysisOperationCoordinator> logger) {
        _applicationService = applicationService;
        _logger = logger;
    }

    public string Start(BuildArchitectureSnapshotCommand command) {
        PruneCompletedOperations();

        var operation = TrackedOperation.Create(command);
        if (!_operations.TryAdd(operation.OperationId, operation)) {
            throw new InvalidOperationException($"Analysis operation {operation.OperationId} already exists.");
        }

        _ = Task.Run(() => RunAsync(operation));
        _logger.LogInformation("Started analysis operation {OperationId} for {WorkspacePath}", operation.OperationId, command.SolutionPath);
        return operation.OperationId;
    }

    public AnalysisOperationView? Get(string operationId) {
        return _operations.TryGetValue(operationId, out var operation)
            ? operation.ToView()
            : null;
    }

    private async Task RunAsync(TrackedOperation operation) {
        operation.MarkRunning();

        try {
            var response = await _applicationService.BuildSnapshotAsync(operation.Command, new OperationProgressReporter(operation));
            if (response.Diagnostics.Any(diagnostic => diagnostic.Severity == AnalysisDiagnosticSeverity.Error)) {
                operation.MarkCompletedWithErrors(response.FromCache, response.Snapshot.SnapshotId, response.Diagnostics);
                _logger.LogWarning(
                    "Analysis operation {OperationId} completed with errors for {WorkspacePath}. Snapshot {SnapshotId}",
                    operation.OperationId,
                    operation.Command.SolutionPath,
                    response.Snapshot.SnapshotId);
                return;
            }

            operation.MarkSucceeded(response.FromCache, response.Snapshot.SnapshotId, response.Diagnostics);
            _logger.LogInformation(
                "Analysis operation {OperationId} completed for {WorkspacePath}. Snapshot {SnapshotId}",
                operation.OperationId,
                operation.Command.SolutionPath,
                response.Snapshot.SnapshotId);
        }
        catch (OperationCanceledException exception) {
            operation.MarkFailed("Analysis was canceled.");
            _logger.LogWarning(exception, "Analysis operation {OperationId} was canceled.", operation.OperationId);
        }
        catch (Exception exception) {
            operation.MarkFailed(exception.Message);
            _logger.LogError(exception, "Analysis operation {OperationId} failed for {WorkspacePath}", operation.OperationId, operation.Command.SolutionPath);
        }
    }

    private void PruneCompletedOperations() {
        var cutoffUtc = DateTimeOffset.UtcNow.AddHours(-12);
        foreach (var candidate in _operations.ToArray()) {
            var operation = candidate.Value.ToView();
            if (operation.Status is AnalysisOperationStatus.Pending or AnalysisOperationStatus.Running) {
                continue;
            }

            if (operation.CompletedUtc is null || operation.CompletedUtc >= cutoffUtc) {
                continue;
            }

            _operations.TryRemove(candidate.Key, out _);
        }
    }

    private sealed class OperationProgressReporter : IAnalysisProgressReporter {
        private readonly TrackedOperation _operation;

        public OperationProgressReporter(TrackedOperation operation) {
            _operation = operation;
        }

        public void Report(AnalysisProgressEvent progressEvent) {
            _operation.AddProgress(progressEvent);
        }
    }

    private sealed class TrackedOperation {
        private readonly object _sync = new();
        private readonly List<AnalysisProgressEvent> _progressEvents = [];
        private readonly List<AnalysisDiagnostic> _diagnostics = [];

        private TrackedOperation(string operationId, BuildArchitectureSnapshotCommand command) {
            OperationId = operationId;
            Command = command;
            CreatedUtc = DateTimeOffset.UtcNow;
            Status = AnalysisOperationStatus.Pending;
        }

        public string OperationId { get; }

        public BuildArchitectureSnapshotCommand Command { get; }

        public DateTimeOffset CreatedUtc { get; }

        public AnalysisOperationStatus Status { get; private set; }

        public DateTimeOffset? StartedUtc { get; private set; }

        public DateTimeOffset? CompletedUtc { get; private set; }

        public string? SnapshotId { get; private set; }

        public string? ErrorMessage { get; private set; }

        public bool FromCache { get; private set; }

        public static TrackedOperation Create(BuildArchitectureSnapshotCommand command) {
            return new TrackedOperation(
                $"op-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..8]}",
                command);
        }

        public void MarkRunning() {
            lock (_sync) {
                Status = AnalysisOperationStatus.Running;
                StartedUtc ??= DateTimeOffset.UtcNow;
            }
        }

        public void AddProgress(AnalysisProgressEvent progressEvent) {
            lock (_sync) {
                _progressEvents.Add(progressEvent);
            }
        }

        public void MarkSucceeded(bool fromCache, string snapshotId, IReadOnlyList<AnalysisDiagnostic> diagnostics) {
            lock (_sync) {
                FromCache = fromCache;
                SnapshotId = snapshotId;
                Status = AnalysisOperationStatus.Succeeded;
                CompletedUtc = DateTimeOffset.UtcNow;
                ReplaceDiagnostics(diagnostics);
            }
        }

        public void MarkCompletedWithErrors(bool fromCache, string snapshotId, IReadOnlyList<AnalysisDiagnostic> diagnostics) {
            lock (_sync) {
                FromCache = fromCache;
                SnapshotId = snapshotId;
                Status = AnalysisOperationStatus.CompletedWithErrors;
                CompletedUtc = DateTimeOffset.UtcNow;
                ReplaceDiagnostics(diagnostics);
            }
        }

        public void MarkFailed(string errorMessage) {
            lock (_sync) {
                ErrorMessage = errorMessage;
                Status = AnalysisOperationStatus.Failed;
                CompletedUtc = DateTimeOffset.UtcNow;
            }
        }

        public AnalysisOperationView ToView() {
            lock (_sync) {
                return new AnalysisOperationView(
                    OperationId,
                    Command.SolutionPath,
                    Command.ScopeProjectNames ?? [],
                    Status,
                    CreatedUtc,
                    StartedUtc,
                    CompletedUtc,
                    SnapshotId,
                    FromCache,
                    _progressEvents.ToArray(),
                    _diagnostics.ToArray(),
                    ErrorMessage);
            }
        }

        private void ReplaceDiagnostics(IReadOnlyList<AnalysisDiagnostic> diagnostics) {
            _diagnostics.Clear();
            _diagnostics.AddRange(diagnostics);
        }
    }
}
