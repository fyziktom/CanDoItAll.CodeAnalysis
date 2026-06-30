using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;

namespace CanDoItAll.CodeAnalytics.Application.Services;

public sealed partial class CodeAnalyticsApplicationService {
    private static SymbolQueryContext CreateSymbolQueryContext(ArchitectureSnapshot snapshot) {
        var typesById = snapshot.Facts.Types.ToDictionary(item => item.TypeId, StringComparer.Ordinal);
        var membersById = snapshot.Facts.Members.ToDictionary(item => item.MemberId, StringComparer.Ordinal);
        var membersByTypeId = snapshot.Facts.Members
            .GroupBy(item => item.TypeId, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<MemberFact>)group.ToArray(),
                StringComparer.Ordinal);
        var projectsById = snapshot.Facts.Projects.ToDictionary(item => item.ProjectId, StringComparer.Ordinal);
        var modulesById = snapshot.Facts.Modules.ToDictionary(item => item.ModuleId, StringComparer.Ordinal);
        var namespacesById = snapshot.Facts.Namespaces.ToDictionary(item => item.NamespaceId, StringComparer.Ordinal);
        var availableProjects = snapshot.Facts.Projects
            .Select(item => item.Name)
            .OrderBy(item => item, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new SymbolQueryContext(
            typesById,
            membersById,
            membersByTypeId,
            projectsById,
            modulesById,
            namespacesById,
            availableProjects);
    }

    private static bool TryResolveSymbolTarget(
        SymbolQueryContext context,
        string typeId,
        string? memberId,
        out TypeFact type,
        out MemberFact? member) {
        if (!context.TypesById.TryGetValue(typeId, out type!)) {
            member = null;
            return false;
        }

        if (string.IsNullOrWhiteSpace(memberId)) {
            member = null;
            return true;
        }

        if (!context.MembersById.TryGetValue(memberId, out var resolvedMember)
            || !string.Equals(resolvedMember.TypeId, type.TypeId, StringComparison.Ordinal)) {
            member = null;
            return false;
        }

        member = resolvedMember;
        return true;
    }

    private static bool MatchesProjectFilter(
        SymbolQueryContext context,
        string projectId,
        string? projectName) {
        if (string.IsNullOrWhiteSpace(projectName)) {
            return true;
        }

        return context.ProjectsById.TryGetValue(projectId, out var project)
            && string.Equals(project.Name, projectName, StringComparison.OrdinalIgnoreCase);
    }

    private static SymbolNames ResolveSymbolNames(SymbolQueryContext context, TypeFact type) {
        return new SymbolNames(
            context.ProjectsById.TryGetValue(type.ProjectId, out var project) ? project.Name : type.ProjectId,
            context.ModulesById.TryGetValue(type.ModuleId, out var module) ? module.Name : type.ModuleId,
            context.NamespacesById.TryGetValue(type.NamespaceId, out var @namespace) ? @namespace.Name : type.NamespaceId);
    }
}
