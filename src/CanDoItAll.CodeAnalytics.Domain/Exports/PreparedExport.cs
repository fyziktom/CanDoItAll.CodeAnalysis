namespace CanDoItAll.CodeAnalytics.Domain.Exports;

public sealed record PreparedExport(
    ExportArtifactKind Kind,
    string RelativePath,
    string Title,
    string Description,
    string Content);
