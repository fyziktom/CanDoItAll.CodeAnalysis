using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Identifiers;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Domain.Sources;
using CanDoItAll.CodeAnalytics.Facts.Symbols;
using CanDoItAll.CodeAnalytics.Workspace.Loading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CanDoItAll.CodeAnalytics.Facts.Persistence;

public sealed class PersistenceFactCollector {
    public async Task<PersistenceCollectionResult> CollectAsync(
        WorkspaceLoadResult workspace,
        SymbolCollectionResult symbols,
        CancellationToken cancellationToken = default) {
        if (!workspace.Request.IncludePersistence || workspace.RoslynSolution is null) {
            return new PersistenceCollectionResult([], [], []);
        }

        var diagnostics = new List<AnalysisDiagnostic>();
        var dbContexts = new List<DbContextFact>();
        var entities = new List<EntityFact>();
        var typeByDisplayName = symbols.Types.ToDictionary(type => type.DisplayName, StringComparer.Ordinal);
        var entityIdsByDisplayName = new Dictionary<string, string>(StringComparer.Ordinal);
        var tableMappings = new Dictionary<string, (string? Table, string? Schema)>(StringComparer.Ordinal);

        foreach (var projectContext in workspace.ProjectContexts.OrderBy(context => context.Fact.Name, StringComparer.OrdinalIgnoreCase)) {
            var compilation = await projectContext.Project.GetCompilationAsync(cancellationToken);
            if (compilation is null) {
                diagnostics.Add(
                    new AnalysisDiagnostic(
                        "EF0001",
                    AnalysisDiagnosticSeverity.Warning,
                    $"Compilation was unavailable for project {projectContext.Fact.Name}."));
                continue;
            }

            var projectDocumentPaths = projectContext.Project.Documents
                .Where(document => !string.IsNullOrWhiteSpace(document.FilePath))
                .Select(document => Path.GetFullPath(document.FilePath!))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var dbContextSymbol in EnumerateTypes(compilation.GlobalNamespace)
                .Where(symbol => IsOwnedByProject(symbol, projectDocumentPaths))
                .Where(IsDbContext)) {
                var dbContextDisplayName = dbContextSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
                if (!typeByDisplayName.TryGetValue(dbContextDisplayName, out var dbContextType)) {
                    continue;
                }

                var entitySymbols = dbContextSymbol.GetMembers()
                    .OfType<IPropertySymbol>()
                    .Select(property => ResolveDbSetEntityType(property.Type))
                    .Where(symbol => symbol is not null)
                    .Cast<INamedTypeSymbol>()
                    .GroupBy(symbol => symbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat), StringComparer.Ordinal)
                    .Select(group => group.First())
                    .OrderBy(symbol => symbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat), StringComparer.Ordinal)
                    .ToArray();
                var knownEntityDisplayNames = entitySymbols
                    .Select(symbol => symbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat))
                    .ToHashSet(StringComparer.Ordinal);

                ReadModelBuilderMappings(
                    dbContextSymbol,
                    compilation,
                    workspace.Request,
                    tableMappings,
                    diagnostics,
                    cancellationToken);

                var entityIds = new List<string>();
                foreach (var entitySymbol in entitySymbols) {
                    var entityDisplayName = entitySymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
                    if (!typeByDisplayName.TryGetValue(entityDisplayName, out var entityType)) {
                        diagnostics.Add(
                            new AnalysisDiagnostic(
                                "EF0002",
                                AnalysisDiagnosticSeverity.Info,
                                $"Entity type {entityDisplayName} was not part of the collected source symbol set."));
                        continue;
                    }

                    if (!entityIdsByDisplayName.TryGetValue(entityDisplayName, out var entityId)) {
                        entityId = StableId.ForEntity(entityDisplayName);
                        entityIdsByDisplayName[entityDisplayName] = entityId;
                        entities.Add(
                            CreateEntityFact(
                                entityId,
                                entitySymbol,
                                entityType,
                                typeByDisplayName,
                                knownEntityDisplayNames,
                                tableMappings));
                    }

                    entityIds.Add(entityId);
                }

                dbContexts.Add(
                    new DbContextFact(
                        StableId.ForDbContext(dbContextDisplayName),
                        dbContextType.TypeId,
                        dbContextType.ProjectId,
                        dbContextType.ModuleId,
                        dbContextSymbol.Name,
                        entityIds.OrderBy(value => value, StringComparer.Ordinal).ToArray(),
                        dbContextType.Source));
            }
        }

        return new PersistenceCollectionResult(
            dbContexts.OrderBy(item => item.DisplayName, StringComparer.Ordinal).ToArray(),
            entities.OrderBy(item => item.DisplayName, StringComparer.Ordinal).ToArray(),
            diagnostics.OrderBy(item => item.Code, StringComparer.Ordinal).ThenBy(item => item.Message, StringComparer.Ordinal).ToArray());
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

    private static bool IsOwnedByProject(ISymbol symbol, ISet<string> projectDocumentPaths) {
        return symbol.Locations
            .Where(location => location.IsInSource && location.SourceTree?.FilePath is not null)
            .Select(location => Path.GetFullPath(location.SourceTree!.FilePath))
            .Any(projectDocumentPaths.Contains);
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

        foreach (var child in namespaceSymbol.GetNamespaceMembers()) {
            foreach (var type in EnumerateTypes(child)) {
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

    private static INamedTypeSymbol? ResolveDbSetEntityType(ITypeSymbol typeSymbol) {
        if (typeSymbol is not INamedTypeSymbol namedType) {
            return null;
        }

        if (!string.Equals(namedType.Name, "DbSet", StringComparison.Ordinal) ||
            !string.Equals(namedType.ContainingNamespace.ToDisplayString(), "Microsoft.EntityFrameworkCore", StringComparison.Ordinal)) {
            return null;
        }

        return namedType.TypeArguments[0] as INamedTypeSymbol;
    }

    private static void ReadModelBuilderMappings(
        INamedTypeSymbol dbContextSymbol,
        Compilation compilation,
        AnalysisRequest request,
        IDictionary<string, (string? Table, string? Schema)> tableMappings,
        ICollection<AnalysisDiagnostic> diagnostics,
        CancellationToken cancellationToken) {
        foreach (var syntaxReference in dbContextSymbol.DeclaringSyntaxReferences) {
            if (syntaxReference.GetSyntax(cancellationToken) is not ClassDeclarationSyntax classDeclaration) {
                continue;
            }

            var semanticModel = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
            var onModelCreatingMethods = classDeclaration.Members
                .OfType<MethodDeclarationSyntax>()
                .Where(method => string.Equals(method.Identifier.ValueText, "OnModelCreating", StringComparison.Ordinal));

            foreach (var method in onModelCreatingMethods) {
                var invocations = method.DescendantNodes().OfType<InvocationExpressionSyntax>();
                foreach (var invocation in invocations) {
                    var methodName = GetMethodName(invocation);
                    if (methodName is null) {
                        continue;
                    }

                    if (methodName is "OwnsOne" or "OwnsMany" or "HasConversion" or "ToJson") {
                        diagnostics.Add(
                            new AnalysisDiagnostic(
                                "EF0003",
                                AnalysisDiagnosticSeverity.Info,
                                $"Persistence pattern {methodName} is only partially interpreted.",
                                CreateSourceReference(invocation, request)));
                    }

                    if (methodName != "ToTable") {
                        continue;
                    }

                    var entityDisplayName = TryGetEntityTypeDisplayName(invocation, semanticModel);
                    if (entityDisplayName is null) {
                        continue;
                    }

                    var tableName = TryGetStringArgument(invocation.ArgumentList.Arguments, 0);
                    var schemaName = TryGetStringArgument(invocation.ArgumentList.Arguments, 1);
                    tableMappings[entityDisplayName] = (tableName, schemaName);
                }
            }
        }
    }

    private static EntityFact CreateEntityFact(
        string entityId,
        INamedTypeSymbol entitySymbol,
        TypeFact entityType,
        IReadOnlyDictionary<string, TypeFact> typeByDisplayName,
        ISet<string> knownEntityDisplayNames,
        IReadOnlyDictionary<string, (string? Table, string? Schema)> tableMappings) {
        var relationshipTargets = entitySymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .SelectMany(property => ExpandEntityPropertyTypes(property.Type))
            .Select(candidate => candidate.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat))
            .Where(typeByDisplayName.ContainsKey)
            .Where(knownEntityDisplayNames.Contains)
            .Select(candidate => StableId.ForEntity(candidate))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();

        var keyProperties = entitySymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(property => string.Equals(property.Name, "Id", StringComparison.Ordinal) ||
                string.Equals(property.Name, $"{entitySymbol.Name}Id", StringComparison.Ordinal))
            .Select(property => property.Name)
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();

        tableMappings.TryGetValue(entitySymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat), out var mapping);

        return new EntityFact(
            entityId,
            entityType.TypeId,
            entityType.ProjectId,
            entityType.ModuleId,
            entitySymbol.Name,
            mapping.Table,
            mapping.Schema,
            keyProperties,
            relationshipTargets,
            entityType.Source);
    }

    private static IEnumerable<INamedTypeSymbol> ExpandEntityPropertyTypes(ITypeSymbol? typeSymbol) {
        if (typeSymbol is null) {
            yield break;
        }

        switch (typeSymbol) {
            case INamedTypeSymbol namedType when namedType.SpecialType != SpecialType.None:
                yield break;
            case INamedTypeSymbol namedType when string.Equals(namedType.Name, "String", StringComparison.Ordinal):
                yield break;
            case INamedTypeSymbol namedType when namedType.IsGenericType:
                foreach (var typeArgument in namedType.TypeArguments.OfType<INamedTypeSymbol>()) {
                    yield return typeArgument;
                }

                break;
            case INamedTypeSymbol namedType:
                yield return namedType;
                break;
        }
    }

    private static string? GetMethodName(InvocationExpressionSyntax invocation) {
        return invocation.Expression switch {
            MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier.ValueText,
            GenericNameSyntax genericName => genericName.Identifier.ValueText,
            IdentifierNameSyntax identifierName => identifierName.Identifier.ValueText,
            _ => null,
        };
    }

    private static string? TryGetEntityTypeDisplayName(InvocationExpressionSyntax invocation, SemanticModel semanticModel) {
        SyntaxNode? current = invocation;

        while (current is InvocationExpressionSyntax currentInvocation) {
            if (currentInvocation.Expression is not MemberAccessExpressionSyntax memberAccess) {
                return null;
            }

            if (memberAccess.Name is GenericNameSyntax genericName &&
                string.Equals(genericName.Identifier.ValueText, "Entity", StringComparison.Ordinal) &&
                genericName.TypeArgumentList.Arguments.Count == 1) {
                var entityType = semanticModel.GetTypeInfo(genericName.TypeArgumentList.Arguments[0]).Type;
                return entityType?.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
            }

            current = memberAccess.Expression;
        }

        return null;
    }

    private static string? TryGetStringArgument(SeparatedSyntaxList<ArgumentSyntax> arguments, int index) {
        if (arguments.Count <= index) {
            return null;
        }

        return arguments[index].Expression switch {
            LiteralExpressionSyntax literalExpression when literalExpression.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.StringLiteralExpression)
                => literalExpression.Token.ValueText,
            _ => null,
        };
    }

    private static SourceReference? CreateSourceReference(SyntaxNode syntaxNode, AnalysisRequest request) {
        var location = syntaxNode.GetLocation().GetLineSpan();
        if (string.IsNullOrWhiteSpace(location.Path)) {
            return null;
        }

        var solutionDirectory = Path.GetDirectoryName(request.SolutionPath)!;
        return new SourceReference(
            Path.GetRelativePath(solutionDirectory, location.Path).Replace('\\', '/'),
            location.StartLinePosition.Line + 1,
            location.StartLinePosition.Character + 1);
    }
}
