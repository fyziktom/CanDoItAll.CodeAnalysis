namespace CanDoItAll.CodeAnalytics.Workspace.Inventory;

public sealed record ProjectFileInventory(
    IReadOnlyList<string> TargetFrameworks,
    IReadOnlyList<string> ProjectReferencePaths,
    IReadOnlyList<string> PackageReferences);
