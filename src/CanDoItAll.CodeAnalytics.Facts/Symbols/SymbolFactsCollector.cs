using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Identifiers;
using CanDoItAll.CodeAnalytics.Domain.Sources;
using CanDoItAll.CodeAnalytics.Facts.Documentation;
using CanDoItAll.CodeAnalytics.Workspace.Loading;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CanDoItAll.CodeAnalytics.Facts.Symbols;

public sealed partial class SymbolFactsCollector {
    private readonly XmlDocumentationNormalizer _xmlDocumentationNormalizer;
    private readonly ILogger<SymbolFactsCollector> _logger;

    public SymbolFactsCollector(
        XmlDocumentationNormalizer xmlDocumentationNormalizer,
        ILogger<SymbolFactsCollector>? logger = null) {
        _xmlDocumentationNormalizer = xmlDocumentationNormalizer;
        _logger = logger ?? NullLogger<SymbolFactsCollector>.Instance;
    }

    public async Task<SymbolCollectionResult> CollectAsync(
        WorkspaceLoadResult workspace,
        CancellationToken cancellationToken = default) {
        if (workspace.RoslynSolution is null) {
            return new SymbolCollectionResult([], [], [], []);
        }

        var diagnostics = new List<AnalysisDiagnostic>();
        var types = new List<TypeFact>();
        var members = new List<MemberFact>();
        var namespaceIndex = new Dictionary<(string ProjectId, string ModuleId, string NamespaceId), NamespaceBuilder>();

        foreach (var projectContext in workspace.ProjectContexts.OrderBy(context => context.Fact.Name, StringComparer.OrdinalIgnoreCase)) {
            if (!ShouldIncludeProject(workspace.Request, projectContext.Fact)) {
                continue;
            }

            var compilation = await projectContext.Project.GetCompilationAsync(cancellationToken);
            if (compilation is null) {
                diagnostics.Add(
                    new AnalysisDiagnostic(
                        "SYM0001",
                        AnalysisDiagnosticSeverity.Warning,
                        $"Compilation was unavailable for project {projectContext.Fact.Name}."));
                continue;
            }

            var projectDocumentPaths = projectContext.Project.Documents
                .Where(document => !string.IsNullOrWhiteSpace(document.FilePath))
                .Select(document => Path.GetFullPath(document.FilePath!))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var symbol in EnumerateTypes(compilation.GlobalNamespace)) {
                if (!IsOwnedByProject(symbol, projectDocumentPaths)) {
                    continue;
                }

                if (!ShouldIncludeType(workspace.Request, projectContext.Fact, symbol)) {
                    continue;
                }

                var namespaceName = symbol.ContainingNamespace.IsGlobalNamespace
                    ? projectContext.Fact.Name
                    : symbol.ContainingNamespace.ToDisplayString();
                var moduleName = ModuleNameClassifier.GetModuleName(projectContext.Fact.Name, namespaceName);
                var moduleId = StableId.ForModule($"{projectContext.Fact.ProjectId}:{moduleName}");
                var namespaceId = StableId.ForNamespace($"{projectContext.Fact.ProjectId}:{namespaceName}");
                var displayName = symbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
                var source = CreateSourceReference(symbol, projectContext.Project, workspace.Request);
                var xmlSummary = GetXmlSummary(workspace.Request, symbol, source, diagnostics, cancellationToken);
                var typeId = StableId.ForType($"{projectContext.Fact.ProjectId}:{displayName}");
                var typeMembers = CreateMembers(symbol, typeId, projectContext.Project, workspace.Request)
                    .OrderBy(member => member.DisplayName, StringComparer.Ordinal)
                    .ToArray();

                members.AddRange(typeMembers);

                types.Add(
                    new TypeFact(
                        typeId,
                        projectContext.Fact.ProjectId,
                        moduleId,
                        namespaceId,
                        displayName,
                        MapTypeKind(symbol),
                        GetBaseTypeDisplayName(symbol),
                        symbol.Interfaces
                            .Select(type => type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat))
                            .OrderBy(value => value, StringComparer.Ordinal)
                            .ToArray(),
                        typeMembers.Select(member => member.MemberId).ToArray(),
                        xmlSummary,
                        source ?? new SourceReference("unknown")));

                var namespaceKey = (projectContext.Fact.ProjectId, moduleId, namespaceId);
                if (!namespaceIndex.TryGetValue(namespaceKey, out var builder)) {
                    builder = new NamespaceBuilder(namespaceName);
                    namespaceIndex[namespaceKey] = builder;
                }

                builder.TypeIds.Add(typeId);
            }
        }

        var duplicateTypeGroups = types
            .GroupBy(type => type.TypeId, StringComparer.Ordinal)
            .Where(group => group.Count() > 1)
            .OrderBy(group => group.Key, StringComparer.Ordinal)
            .ToArray();
        foreach (var group in duplicateTypeGroups) {
            diagnostics.Add(
                new AnalysisDiagnostic(
                    "SYM0002",
                    AnalysisDiagnosticSeverity.Warning,
                    $"Duplicate type facts were collapsed for {group.First().DisplayName}."));
            _logger.LogWarning("Duplicate type facts were collapsed for {TypeId}", group.Key);
        }

        var orderedTypes = types
            .GroupBy(type => type.TypeId, StringComparer.Ordinal)
            .Select(
                group => group
                    .OrderBy(type => type.Source.Path, StringComparer.Ordinal)
                    .ThenBy(type => type.DisplayName, StringComparer.Ordinal)
                    .First())
            .OrderBy(type => type.DisplayName, StringComparer.Ordinal)
            .ToArray();
        var orderedMembers = members
            .GroupBy(member => member.MemberId, StringComparer.Ordinal)
            .Select(group => group.First())
            .OrderBy(member => member.DisplayName, StringComparer.Ordinal)
            .ToArray();
        var namespaces = namespaceIndex
            .OrderBy(item => item.Value.Name, StringComparer.OrdinalIgnoreCase)
            .Select(
                item => new NamespaceFact(
                    item.Key.NamespaceId,
                    item.Key.ProjectId,
                    item.Key.ModuleId,
                    item.Value.Name,
                    item.Value.TypeIds
                        .Distinct(StringComparer.Ordinal)
                        .OrderBy(value => value, StringComparer.Ordinal)
                        .ToArray()))
            .ToArray();

        return new SymbolCollectionResult(
            namespaces,
            orderedTypes,
            orderedMembers,
            diagnostics.OrderBy(diagnostic => diagnostic.Code, StringComparer.Ordinal).ThenBy(diagnostic => diagnostic.Message, StringComparer.Ordinal).ToArray());
    }
}
