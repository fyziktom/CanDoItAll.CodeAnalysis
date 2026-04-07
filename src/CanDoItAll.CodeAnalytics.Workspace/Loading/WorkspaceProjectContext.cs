using CanDoItAll.CodeAnalytics.Domain.Facts;
using Microsoft.CodeAnalysis;

namespace CanDoItAll.CodeAnalytics.Workspace.Loading;

public sealed record WorkspaceProjectContext(
    Project Project,
    ProjectFact Fact,
    IReadOnlyList<WorkspaceDocumentContext> Documents);
