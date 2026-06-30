using CanDoItAll.CodeAnalytics.Abstractions.Responses;
using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Application.Services;

public sealed partial class CodeAnalyticsApplicationService {
    private sealed record SymbolNames(
        string ProjectName,
        string ModuleName,
        string NamespaceName);

    private sealed record SymbolQueryContext(
        IReadOnlyDictionary<string, TypeFact> TypesById,
        IReadOnlyDictionary<string, MemberFact> MembersById,
        IReadOnlyDictionary<string, IReadOnlyList<MemberFact>> MembersByTypeId,
        IReadOnlyDictionary<string, ProjectFact> ProjectsById,
        IReadOnlyDictionary<string, ModuleFact> ModulesById,
        IReadOnlyDictionary<string, NamespaceFact> NamespacesById,
        IReadOnlyList<string> AvailableProjects);

    private sealed record ScoredSymbolSearchResult(SymbolSearchResultItem Result, int Score);

    private sealed record ScoredSymbolReference(SymbolReferenceItem Reference, int Score);
}
