using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Identifiers;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Domain.Sources;
using CanDoItAll.CodeAnalytics.Facts.Documentation;
using CanDoItAll.CodeAnalytics.Workspace.Loading;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CanDoItAll.CodeAnalytics.Facts.Symbols;

public sealed class SymbolFactsCollector {
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

    private static IEnumerable<INamedTypeSymbol> EnumerateTypes(INamespaceSymbol namespaceSymbol) {
        foreach (var type in namespaceSymbol.GetTypeMembers()) {
            if (type.Locations.Any(location => location.IsInSource)) {
                yield return type;
            }

            foreach (var nested in EnumerateNestedTypes(type)) {
                yield return nested;
            }
        }

        foreach (var childNamespace in namespaceSymbol.GetNamespaceMembers()) {
            foreach (var type in EnumerateTypes(childNamespace)) {
                yield return type;
            }
        }
    }

    private static IEnumerable<INamedTypeSymbol> EnumerateNestedTypes(INamedTypeSymbol typeSymbol) {
        foreach (var nestedType in typeSymbol.GetTypeMembers()) {
            if (nestedType.Locations.Any(location => location.IsInSource)) {
                yield return nestedType;
            }

            foreach (var child in EnumerateNestedTypes(nestedType)) {
                yield return child;
            }
        }
    }

    private static IReadOnlyList<MemberFact> CreateMembers(
        INamedTypeSymbol symbol,
        string typeId,
        Project project,
        AnalysisRequest request) {
        return symbol.GetMembers()
            .Where(member => !member.IsImplicitlyDeclared)
            .Select(member => CreateMemberFact(member, typeId, project, request))
            .Where(member => member is not null)
            .Cast<MemberFact>()
            .ToArray();
    }

    private static MemberFact? CreateMemberFact(
        ISymbol member,
        string typeId,
        Project project,
        AnalysisRequest request) {
        var source = CreateSourceReference(member, project, request);
        if (source is null) {
            return null;
        }

        return member switch {
            IMethodSymbol method when method.MethodKind == MethodKind.Constructor => new MemberFact(
                StableId.ForMember($"{typeId}:{method.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)}"),
                typeId,
                method.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
                MemberKind.Constructor,
                method.ContainingType.Name,
                method.Parameters.Select(parameter => parameter.Type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)).ToArray(),
                source),
            IMethodSymbol method when method.MethodKind == MethodKind.Ordinary => new MemberFact(
                StableId.ForMember($"{typeId}:{method.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)}"),
                typeId,
                method.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
                MemberKind.Method,
                method.ReturnType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
                method.Parameters.Select(parameter => parameter.Type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)).ToArray(),
                source),
            IPropertySymbol property => new MemberFact(
                StableId.ForMember($"{typeId}:{property.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)}"),
                typeId,
                property.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
                MemberKind.Property,
                property.Type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
                [],
                source),
            IFieldSymbol field => new MemberFact(
                StableId.ForMember($"{typeId}:{field.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)}"),
                typeId,
                field.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
                MemberKind.Field,
                field.Type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
                [],
                source),
            IEventSymbol eventSymbol => new MemberFact(
                StableId.ForMember($"{typeId}:{eventSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)}"),
                typeId,
                eventSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
                MemberKind.Event,
                eventSymbol.Type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
                [],
                source),
            _ => null,
        };
    }

    private string? GetXmlSummary(
        AnalysisRequest request,
        INamedTypeSymbol symbol,
        SourceReference? source,
        ICollection<AnalysisDiagnostic> diagnostics,
        CancellationToken cancellationToken) {
        if (!request.IncludeXmlDocs) {
            return null;
        }

        var xml = symbol.GetDocumentationCommentXml(expandIncludes: true, cancellationToken: cancellationToken);
        var result = _xmlDocumentationNormalizer.Normalize(
            xml,
            symbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
            source);
        foreach (var diagnostic in result.Diagnostics) {
            diagnostics.Add(diagnostic);
            _logger.LogInformation("XML documentation diagnostic {Code}: {Message}", diagnostic.Code, diagnostic.Message);
        }

        return result.Summary;
    }

    private static string? GetBaseTypeDisplayName(INamedTypeSymbol symbol) {
        if (symbol.TypeKind == Microsoft.CodeAnalysis.TypeKind.Interface) {
            return null;
        }

        var baseType = symbol.BaseType;
        if (baseType is null || baseType.SpecialType == SpecialType.System_Object) {
            return null;
        }

        return baseType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
    }

    private static bool ShouldIncludeProject(AnalysisRequest request, ProjectFact project) {
        if (request.ScopeProjectNames.Count == 0) {
            return true;
        }

        return request.ScopeProjectNames.Contains(project.Name, StringComparer.OrdinalIgnoreCase);
    }

    private static bool ShouldIncludeType(AnalysisRequest request, ProjectFact project, INamedTypeSymbol symbol) {
        if (!ShouldIncludeProject(request, project)) {
            return false;
        }

        if (request.ScopeNamespacePrefixes.Count == 0) {
            return true;
        }

        var namespaceName = symbol.ContainingNamespace.IsGlobalNamespace
            ? project.Name
            : symbol.ContainingNamespace.ToDisplayString();
        return request.ScopeNamespacePrefixes.Any(
            prefix => namespaceName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsOwnedByProject(ISymbol symbol, ISet<string> projectDocumentPaths) {
        return symbol.Locations
            .Where(location => location.IsInSource && location.SourceTree?.FilePath is not null)
            .Select(location => Path.GetFullPath(location.SourceTree!.FilePath))
            .Any(projectDocumentPaths.Contains);
    }

    private static CanDoItAll.CodeAnalytics.Domain.Facts.TypeKind MapTypeKind(INamedTypeSymbol symbol) {
        return symbol.TypeKind switch {
            Microsoft.CodeAnalysis.TypeKind.Class when symbol.IsRecord => CanDoItAll.CodeAnalytics.Domain.Facts.TypeKind.Record,
            Microsoft.CodeAnalysis.TypeKind.Class => CanDoItAll.CodeAnalytics.Domain.Facts.TypeKind.Class,
            Microsoft.CodeAnalysis.TypeKind.Interface => CanDoItAll.CodeAnalytics.Domain.Facts.TypeKind.Interface,
            Microsoft.CodeAnalysis.TypeKind.Struct when symbol.IsRecord => CanDoItAll.CodeAnalytics.Domain.Facts.TypeKind.Record,
            Microsoft.CodeAnalysis.TypeKind.Struct => CanDoItAll.CodeAnalytics.Domain.Facts.TypeKind.Struct,
            Microsoft.CodeAnalysis.TypeKind.Enum => CanDoItAll.CodeAnalytics.Domain.Facts.TypeKind.Enum,
            Microsoft.CodeAnalysis.TypeKind.Delegate => CanDoItAll.CodeAnalytics.Domain.Facts.TypeKind.Delegate,
            _ => CanDoItAll.CodeAnalytics.Domain.Facts.TypeKind.Class,
        };
    }

    private static SourceReference? CreateSourceReference(ISymbol symbol, Project project, AnalysisRequest request) {
        var location = symbol.Locations.FirstOrDefault(candidate => candidate.IsInSource && candidate.SourceTree is not null);
        if (location is null || location.SourceTree?.FilePath is null) {
            return null;
        }

        var lineSpan = location.GetLineSpan();
        var solutionDirectory = Path.GetDirectoryName(request.SolutionPath)!;
        return new SourceReference(
            Path.GetRelativePath(solutionDirectory, lineSpan.Path).Replace('\\', '/'),
            lineSpan.StartLinePosition.Line + 1,
            lineSpan.StartLinePosition.Character + 1);
    }

    private sealed class NamespaceBuilder {
        public NamespaceBuilder(string name) {
            Name = name;
        }

        public string Name { get; }

        public List<string> TypeIds { get; } = [];
    }
}
