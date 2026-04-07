namespace CanDoItAll.CodeAnalytics.Domain.Exports;

public sealed record ExportArtifact(
    ExportArtifactKind Kind,
    string RelativePath,
    string Title,
    string Description,
    long? SizeInBytes = null);
