using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record ProjectInventoryItem(
    ProjectFact Project,
    ProjectRoleKind ProjectRole,
    IReadOnlyList<ProjectLinkItem> DirectProjectReferences,
    IReadOnlyList<ProjectLinkItem> SupportingDirectProjectReferences,
    IReadOnlyList<ProjectLinkItem> ReferencedByProjects,
    IReadOnlyList<ProjectLinkItem> SupportingReferencedByProjects,
    IReadOnlyList<DocumentFact> Documents);
