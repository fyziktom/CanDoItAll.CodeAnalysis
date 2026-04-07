using CanDoItAll.CodeAnalytics.Domain.Facts;
using Microsoft.CodeAnalysis;

namespace CanDoItAll.CodeAnalytics.Workspace.Loading;

public sealed record WorkspaceDocumentContext(
    Document Document,
    DocumentFact Fact);
