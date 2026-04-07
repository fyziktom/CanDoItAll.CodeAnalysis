using CanDoItAll.CodeAnalytics.Domain.Snapshot;

namespace CanDoItAll.CodeAnalytics.Workspace.Normalization;

public sealed class AnalysisRequestNormalizer {
    public AnalysisRequest Normalize(AnalysisRequest request) {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.SolutionPath);

        var fullSolutionPath = Path.GetFullPath(request.SolutionPath);
        var extension = Path.GetExtension(fullSolutionPath);
        if (!string.Equals(extension, ".sln", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(extension, ".slnx", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(extension, ".csproj", StringComparison.OrdinalIgnoreCase)) {
            throw new ArgumentException(
                $"Only .sln, .slnx, and .csproj inputs are supported: {request.SolutionPath}",
                nameof(request));
        }

        var normalizedProjects = NormalizeList(request.ScopeProjectNames);
        var normalizedNamespaces = NormalizeList(request.ScopeNamespacePrefixes);

        return new AnalysisRequest(
            fullSolutionPath,
            normalizedProjects,
            normalizedNamespaces,
            request.IncludeDi,
            request.IncludePersistence,
            request.IncludeRisks,
            request.IncludeXmlDocs,
            request.IncludeMermaidExports);
    }

    private static IReadOnlyList<string> NormalizeList(IReadOnlyList<string> values) {
        return values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
