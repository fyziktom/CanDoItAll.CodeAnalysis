using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace CanDoItAll.CodeAnalytics.Workspace.Loading;

public sealed class WorkspaceLoadResult : IDisposable {
    private readonly MSBuildWorkspace? _workspace;

    public WorkspaceLoadResult(
        AnalysisRequest request,
        SolutionFact? solution,
        IReadOnlyList<ProjectFact> projects,
        IReadOnlyList<DocumentFact> documents,
        IReadOnlyList<WorkspaceProjectContext> projectContexts,
        IReadOnlyList<AnalysisDiagnostic> diagnostics,
        Solution? roslynSolution,
        MSBuildWorkspace? workspace) {
        Request = request;
        Solution = solution;
        Projects = projects;
        Documents = documents;
        ProjectContexts = projectContexts;
        Diagnostics = diagnostics;
        RoslynSolution = roslynSolution;
        _workspace = workspace;
    }

    public AnalysisRequest Request { get; }

    public SolutionFact? Solution { get; }

    public IReadOnlyList<ProjectFact> Projects { get; }

    public IReadOnlyList<DocumentFact> Documents { get; }

    public IReadOnlyList<WorkspaceProjectContext> ProjectContexts { get; }

    public IReadOnlyList<AnalysisDiagnostic> Diagnostics { get; }

    public Solution? RoslynSolution { get; }

    public bool HasBlockingErrors {
        get {
            return RoslynSolution is null || Diagnostics.Any(diagnostic => diagnostic.Severity == AnalysisDiagnosticSeverity.Error);
        }
    }

    public void Dispose() {
        _workspace?.Dispose();
    }
}
