using CanDoItAll.CodeAnalytics.Abstractions;
using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Application.Services;

public sealed partial class CodeAnalyticsApplicationService {
    private static IReadOnlyList<ServiceRegistrationFact> SelectRelevantServices(
        IReadOnlyList<ServiceRegistrationFact> services,
        ISet<string> typeIdSet,
        IReadOnlyList<TypeFact> types,
        IReadOnlyDictionary<string, ProjectFact> projectsById,
        TypeFact? seedType,
        IReadOnlyCollection<string> focusTags) {
        var selectedTypes = types
            .Where(type => typeIdSet.Contains(type.TypeId))
            .ToArray();
        return services
            .Where(service => ServiceTouchesTypes(service, typeIdSet, types, projectsById, seedType) || ServiceMentionsSelectedTypes(service, selectedTypes, seedType))
            .OrderByDescending(service => ScoreRelevantService(service, selectedTypes, focusTags, seedType))
            .ThenBy(item => item.ServiceTypeDisplayName, StringComparer.Ordinal)
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

    private static bool ServiceMentionsSelectedTypes(
        ServiceRegistrationFact service,
        IReadOnlyList<TypeFact> selectedTypes,
        TypeFact? seedType) {
        if (selectedTypes.Count == 0) {
            return false;
        }

        var haystack = string.Join(
            ' ',
            service.ServiceTypeDisplayName,
            service.ImplementationTypeDisplayName,
            service.RegistrationMethod,
            service.Source.Path);
        foreach (var type in selectedTypes) {
            if (seedType is not null
                && !string.Equals(type.ProjectId, seedType.ProjectId, StringComparison.Ordinal)
                && !string.Equals(type.ModuleId, seedType.ModuleId, StringComparison.Ordinal)) {
                continue;
            }

            if (haystack.Contains(type.DisplayName, StringComparison.Ordinal)
                || haystack.Contains(GetTrailingIdentifier(type.DisplayName), StringComparison.Ordinal)) {
                return true;
            }
        }

        return false;
    }

    private static int ScoreRelevantService(
        ServiceRegistrationFact service,
        IReadOnlyList<TypeFact> selectedTypes,
        IReadOnlyCollection<string> focusTags,
        TypeFact? seedType) {
        var score = GetFocusTagScore(focusTags, service.ServiceTypeDisplayName, service.ImplementationTypeDisplayName, service.Source.Path);
        var haystack = string.Join(
            ' ',
            service.ServiceTypeDisplayName,
            service.ImplementationTypeDisplayName,
            service.RegistrationMethod,
            service.Source.Path);
        foreach (var type in selectedTypes) {
            if (haystack.Contains(type.DisplayName, StringComparison.Ordinal)) {
                score += 48;
                continue;
            }

            if (haystack.Contains(GetTrailingIdentifier(type.DisplayName), StringComparison.Ordinal)) {
                score += 36;
            }
        }

        if (!string.IsNullOrWhiteSpace(service.ImplementationTypeDisplayName)
            && service.ImplementationTypeDisplayName.Contains("Factory", StringComparison.Ordinal)) {
            score += GetRoleScoreBonus(FocusedContextReferenceRoleKind.Factory);
        }

        if (service.UsesFactory) {
            score += 18;
        }

        if (seedType is not null && string.Equals(service.ProjectId, seedType.ProjectId, StringComparison.Ordinal)) {
            score += 12;
        }

        score += GetRoleScoreBonus(FocusedContextReferenceRoleKind.Registration);
        return score;
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
