namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record ProjectLinkItem(
    string ProjectId,
    string ProjectName,
    string ProjectPath,
    ProjectRoleKind ProjectRole);
