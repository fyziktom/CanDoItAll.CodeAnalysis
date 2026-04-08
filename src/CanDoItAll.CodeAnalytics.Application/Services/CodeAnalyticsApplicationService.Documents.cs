using CanDoItAll.CodeAnalytics.Abstractions.Queries;
using CanDoItAll.CodeAnalytics.Abstractions.Responses;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;

namespace CanDoItAll.CodeAnalytics.Application.Services;

public sealed partial class CodeAnalyticsApplicationService {
    public async Task<DocumentSourceResponse?> GetDocumentSourceAsync(
        DocumentQuery query,
        CancellationToken cancellationToken = default) {
        var snapshot = await GetSnapshotAsync(query.SnapshotId, cancellationToken);
        if (snapshot is null) {
            return null;
        }

        var resolvedDocument = ResolveDocument(snapshot, query);
        if (resolvedDocument is null) {
            return null;
        }

        var sourceCode = await File.ReadAllTextAsync(resolvedDocument.AbsolutePath, cancellationToken);
        return new DocumentSourceResponse(
            snapshot.SnapshotId,
            resolvedDocument.Project.Name,
            resolvedDocument.Document,
            sourceCode);
    }

    public async Task<DocumentSymbolsResponse?> GetDocumentSymbolsAsync(
        DocumentQuery query,
        CancellationToken cancellationToken = default) {
        var snapshot = await GetSnapshotAsync(query.SnapshotId, cancellationToken);
        if (snapshot is null) {
            return null;
        }

        var resolvedDocument = ResolveDocument(snapshot, query);
        if (resolvedDocument is null) {
            return null;
        }

        var modulesById = snapshot.Facts.Modules.ToDictionary(module => module.ModuleId, StringComparer.Ordinal);
        var namespacesById = snapshot.Facts.Namespaces.ToDictionary(@namespace => @namespace.NamespaceId, StringComparer.Ordinal);
        var membersByTypeId = snapshot.Facts.Members
            .GroupBy(member => member.TypeId, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<MemberFact>)group
                    .OrderBy(member => member.Kind)
                    .ThenBy(member => member.DisplayName, StringComparer.Ordinal)
                    .ToArray(),
                StringComparer.Ordinal);
        var documentPath = NormalizePath(resolvedDocument.Document.Path);
        var types = snapshot.Facts.Types
            .Where(type => string.Equals(NormalizePath(type.Source.Path), documentPath, StringComparison.OrdinalIgnoreCase))
            .OrderBy(type => namespacesById[type.NamespaceId].Name, StringComparer.Ordinal)
            .ThenBy(type => type.DisplayName, StringComparer.Ordinal)
            .Select(
                type => new TypeSearchResultItem(
                    resolvedDocument.Project.Name,
                    modulesById[type.ModuleId].Name,
                    namespacesById[type.NamespaceId].Name,
                    type,
                    membersByTypeId.TryGetValue(type.TypeId, out var members)
                        ? members
                        : []))
            .ToArray();

        return new DocumentSymbolsResponse(
            snapshot.SnapshotId,
            resolvedDocument.Project.Name,
            resolvedDocument.Document,
            types);
    }

    private static ResolvedDocument? ResolveDocument(
        ArchitectureSnapshot snapshot,
        DocumentQuery query) {
        var projectsById = snapshot.Facts.Projects.ToDictionary(project => project.ProjectId, StringComparer.Ordinal);
        var workspaceRoot = Path.GetDirectoryName(snapshot.Request.SolutionPath)!;

        DocumentFact? document = null;
        if (!string.IsNullOrWhiteSpace(query.DocumentId)) {
            var documentId = query.DocumentId.Trim();
            document = snapshot.Facts.Documents.FirstOrDefault(
                item => string.Equals(item.DocumentId, documentId, StringComparison.Ordinal));
        }

        if (document is null && !string.IsNullOrWhiteSpace(query.DocumentPath)) {
            var lookupPath = NormalizeDocumentLookupPath(query.DocumentPath.Trim(), workspaceRoot);
            document = snapshot.Facts.Documents.FirstOrDefault(
                item => string.Equals(NormalizePath(item.Path), lookupPath, StringComparison.OrdinalIgnoreCase));
        }

        if (document is null
            || !projectsById.TryGetValue(document.ProjectId, out var project)) {
            return null;
        }

        var absolutePath = ResolveAbsoluteSourcePath(workspaceRoot, NormalizePath(document.Path));
        return File.Exists(absolutePath)
            ? new ResolvedDocument(document, project, absolutePath)
            : null;
    }

    private static string NormalizeDocumentLookupPath(string documentPath, string workspaceRoot) {
        var candidatePath = Path.IsPathRooted(documentPath)
            ? documentPath
            : Path.GetFullPath(Path.Combine(workspaceRoot, documentPath));
        var relativePath = Path.GetRelativePath(workspaceRoot, candidatePath);
        return NormalizePath(relativePath);
    }

    private sealed record ResolvedDocument(
        DocumentFact Document,
        ProjectFact Project,
        string AbsolutePath);
}
