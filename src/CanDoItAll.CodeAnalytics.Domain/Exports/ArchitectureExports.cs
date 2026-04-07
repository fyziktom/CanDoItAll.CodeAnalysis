namespace CanDoItAll.CodeAnalytics.Domain.Exports;

public sealed record ArchitectureExports(IReadOnlyList<ExportArtifact> Artifacts) {
    public static ArchitectureExports Empty { get; } = new([]);
}
