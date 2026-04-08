using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record ProjectInventoryItem(
    ProjectFact Project,
    IReadOnlyList<ProjectLinkItem> DirectProjectReferences,
    IReadOnlyList<ProjectLinkItem> ReferencedByProjects,
    IReadOnlyList<DocumentFact> Documents);
