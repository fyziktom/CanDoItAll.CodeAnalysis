using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace CanDoItAll.CodeAnalytics.Facts.Persistence;

public sealed partial class PersistenceFactCollector {
    private static bool TryResolveEntitySymbol(
        IReadOnlyDictionary<string, ProjectAnalysisContext> projectAnalysesByProjectId,
        string projectId,
        string displayName,
        out INamedTypeSymbol entitySymbol) {
        if (!projectAnalysesByProjectId.TryGetValue(projectId, out var analysis) ||
            !analysis.SourceTypesByDisplayName.TryGetValue(displayName, out var candidates) ||
            candidates.Length == 0) {
            entitySymbol = null!;
            return false;
        }

        entitySymbol = candidates
            .OrderBy(candidate => candidate.Name, StringComparer.Ordinal)
            .First();
        return true;
    }

    private static IEnumerable<INamedTypeSymbol> EnumerateDbSetEntityTypes(INamedTypeSymbol dbContextSymbol) {
        var current = dbContextSymbol;
        while (current is not null) {
            foreach (var entitySymbol in current.GetMembers()
                .OfType<IPropertySymbol>()
                .Select(property => ResolveDbSetEntityType(property.Type))
                .Where(symbol => symbol is not null)
                .Cast<INamedTypeSymbol>()) {
                yield return entitySymbol;
            }

            current = current.BaseType;
        }
    }

    private static bool IsDbContext(INamedTypeSymbol symbol) {
        var current = symbol;
        while (current is not null) {
            if (string.Equals(current.ToDisplayString(), "Microsoft.EntityFrameworkCore.DbContext", StringComparison.Ordinal)) {
                return true;
            }

            current = current.BaseType;
        }

        return false;
    }

    private static bool ShouldIncludeProject(AnalysisRequest request, ProjectFact project) {
        if (request.ScopeProjectNames.Count == 0) {
            return true;
        }

        return request.ScopeProjectNames.Contains(project.Name, StringComparer.OrdinalIgnoreCase);
    }

    private static bool IsOwnedByProject(ISymbol symbol, ISet<string> projectDocumentPaths) {
        return symbol.Locations
            .Where(location => location.IsInSource && location.SourceTree?.FilePath is not null)
            .Select(location => Path.GetFullPath(location.SourceTree!.FilePath))
            .Any(projectDocumentPaths.Contains);
    }

    private bool TryResolveTypeFact(
        IReadOnlyDictionary<string, TypeFact[]> typesByDisplayName,
        string displayName,
        string projectId,
        ICollection<AnalysisDiagnostic> diagnostics,
        out TypeFact typeFact) {
        if (!typesByDisplayName.TryGetValue(displayName, out var candidates) || candidates.Length == 0) {
            typeFact = null!;
            return false;
        }

        var projectMatch = candidates.FirstOrDefault(candidate => string.Equals(candidate.ProjectId, projectId, StringComparison.Ordinal));
        if (projectMatch is not null) {
            typeFact = projectMatch;
            return true;
        }

        if (candidates.Length > 1) {
            var diagnostic = new AnalysisDiagnostic(
                "EF0004",
                AnalysisDiagnosticSeverity.Warning,
                $"Multiple collected types share the display name {displayName}. Falling back to the first candidate.");
            diagnostics.Add(diagnostic);
            _logger.LogWarning("Multiple collected types share the display name {DisplayName}. Falling back to the first candidate.", displayName);
        }

        typeFact = candidates
            .OrderBy(candidate => candidate.ProjectId, StringComparer.Ordinal)
            .ThenBy(candidate => candidate.TypeId, StringComparer.Ordinal)
            .First();
        return true;
    }
}
