using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Application.Services;

public sealed partial class CodeAnalyticsApplicationService {
    private static IReadOnlyList<ServiceRegistrationFact> SelectRelevantServices(
        IReadOnlyList<ServiceRegistrationFact> services,
        ISet<string> typeIdSet,
        IReadOnlyList<TypeFact> types,
        IReadOnlyDictionary<string, ProjectFact> projectsById,
        TypeFact? seedType) {
        return services
            .Where(service => ServiceTouchesTypes(service, typeIdSet, types, projectsById, seedType))
            .OrderBy(item => item.ServiceTypeDisplayName, StringComparer.Ordinal)
            .ThenBy(item => item.ImplementationTypeDisplayName, StringComparer.Ordinal)
            .Take(MaxRelatedServices)
            .ToArray();
    }

    private static bool ServiceTouchesTypes(
        ServiceRegistrationFact service,
        ISet<string> typeIds,
        IReadOnlyList<TypeFact> types,
        IReadOnlyDictionary<string, ProjectFact> projectsById,
        TypeFact? seedType) {
        var type = ResolveTypeForService(service, types);
        return type is not null
            && typeIds.Contains(type.TypeId)
            && !ShouldExcludeFromFocusedContext(type, projectsById, seedType);
    }

    private static bool ShouldExcludeFromFocusedContext(
        TypeFact type,
        IReadOnlyDictionary<string, ProjectFact> projectsById,
        TypeFact? seedType) {
        if (seedType is not null && string.Equals(type.TypeId, seedType.TypeId, StringComparison.Ordinal)) {
            return false;
        }

        var normalizedPath = NormalizePath(type.Source.Path);
        var seedPath = seedType is null ? string.Empty : NormalizePath(seedType.Source.Path);
        var seedIsTest = seedPath.Contains("/tests/", StringComparison.OrdinalIgnoreCase);
        var seedIsMigration = seedPath.Contains("/migrations/", StringComparison.OrdinalIgnoreCase);

        if (!seedIsTest && normalizedPath.Contains("/tests/", StringComparison.OrdinalIgnoreCase)) {
            return true;
        }

        if (!seedIsMigration && normalizedPath.Contains("/migrations/", StringComparison.OrdinalIgnoreCase)) {
            return true;
        }

        if (normalizedPath.Contains("/obj/", StringComparison.OrdinalIgnoreCase) || normalizedPath.EndsWith(".g.cs", StringComparison.OrdinalIgnoreCase)) {
            return true;
        }

        if (projectsById.TryGetValue(type.ProjectId, out var project)
            && !seedIsTest
            && project.Name.Contains("Tests", StringComparison.OrdinalIgnoreCase)) {
            return true;
        }

        return false;
    }

    private static string NormalizePath(string path) {
        return path.Replace('\\', '/');
    }
}
