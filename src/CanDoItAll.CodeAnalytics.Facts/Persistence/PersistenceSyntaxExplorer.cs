using System.ComponentModel.DataAnnotations.Schema;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Domain.Sources;
using CanDoItAll.CodeAnalytics.Workspace.Loading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CanDoItAll.CodeAnalytics.Facts.Persistence;

internal static class PersistenceSyntaxExplorer {
    private const string TableAttributeTypeName = "System.ComponentModel.DataAnnotations.Schema.TableAttribute";

    public static DbContextModelDiscovery DiscoverDbContextModel(
        INamedTypeSymbol dbContextSymbol,
        Compilation compilation,
        AnalysisRequest request,
        CancellationToken cancellationToken) {
        var entityDisplayNames = new HashSet<string>(StringComparer.Ordinal);
        var tableMappings = new Dictionary<string, EntityStoreObjectMapping>(StringComparer.Ordinal);
        var diagnostics = new List<CanDoItAll.CodeAnalytics.Domain.Diagnostics.AnalysisDiagnostic>();
        var includesSameProjectConfigurations = false;
        var includesExternalConfigurations = false;
        string? defaultSchema = null;

        foreach (var syntaxReference in dbContextSymbol.DeclaringSyntaxReferences) {
            if (syntaxReference.GetSyntax(cancellationToken) is not ClassDeclarationSyntax classDeclaration) {
                continue;
            }

            var semanticModel = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
            var onModelCreatingMethods = classDeclaration.Members
                .OfType<MethodDeclarationSyntax>()
                .Where(method => string.Equals(method.Identifier.ValueText, "OnModelCreating", StringComparison.Ordinal));

            foreach (var method in onModelCreatingMethods) {
                foreach (var invocation in method.DescendantNodes().OfType<InvocationExpressionSyntax>()) {
                    var methodName = GetMethodName(invocation);
                    if (methodName is null) {
                        continue;
                    }

                    if (methodName == "HasDefaultSchema") {
                        defaultSchema = TryGetStringArgument(invocation.ArgumentList.Arguments, 0) ?? defaultSchema;
                        continue;
                    }

                    if (methodName is "OwnsOne" or "OwnsMany" or "HasConversion" or "ToJson") {
                        diagnostics.Add(
                            new CanDoItAll.CodeAnalytics.Domain.Diagnostics.AnalysisDiagnostic(
                                "EF0003",
                                CanDoItAll.CodeAnalytics.Domain.Diagnostics.AnalysisDiagnosticSeverity.Info,
                                $"Persistence pattern {methodName} is only partially interpreted.",
                                CreateSourceReference(invocation, request)));
                    }

                    if (methodName == "ApplyConfigurationsFromAssembly") {
                        if (IsCurrentAssemblyExpression(invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression, dbContextSymbol)) {
                            includesSameProjectConfigurations = true;
                        }
                        else {
                            includesExternalConfigurations = true;
                        }

                        continue;
                    }

                    if (methodName is not "ToTable" and not "ToView") {
                        continue;
                    }

                    var entityDisplayName = TryGetEntityTypeDisplayName(invocation, semanticModel);
                    if (entityDisplayName is null) {
                        continue;
                    }

                    entityDisplayNames.Add(entityDisplayName);
                    tableMappings[entityDisplayName] = new EntityStoreObjectMapping(
                        entityDisplayName,
                        TryGetStringArgument(invocation.ArgumentList.Arguments, 0),
                        TryGetStringArgument(invocation.ArgumentList.Arguments, 1),
                        CreateSourceReference(invocation, request));
                }

                foreach (var entityInvocation in method.DescendantNodes().OfType<InvocationExpressionSyntax>()) {
                    if (!string.Equals(GetMethodName(entityInvocation), "Entity", StringComparison.Ordinal)) {
                        continue;
                    }

                    var entityDisplayName = TryGetEntityTypeDisplayName(entityInvocation, semanticModel);
                    if (entityDisplayName is not null) {
                        entityDisplayNames.Add(entityDisplayName);
                    }
                }
            }
        }

        return new DbContextModelDiscovery(
            entityDisplayNames.OrderBy(value => value, StringComparer.Ordinal).ToArray(),
            tableMappings.Values.OrderBy(item => item.EntityDisplayName, StringComparer.Ordinal).ToArray(),
            defaultSchema,
            includesSameProjectConfigurations,
            includesExternalConfigurations,
            diagnostics.OrderBy(item => item.Code, StringComparer.Ordinal).ThenBy(item => item.Message, StringComparer.Ordinal).ToArray());
    }

    public static IReadOnlyList<EntityConfigurationMapping> DiscoverEntityConfigurations(
        WorkspaceProjectContext projectContext,
        Compilation compilation,
        AnalysisRequest request,
        ISet<string> projectDocumentPaths,
        CancellationToken cancellationToken) {
        var mappings = new List<EntityConfigurationMapping>();

        foreach (var symbol in EnumerateTypes(compilation.GlobalNamespace)) {
            if (!IsOwnedByProject(symbol, projectDocumentPaths)) {
                continue;
            }

            var entityType = TryGetConfiguredEntityType(symbol);
            if (entityType is null) {
                continue;
            }

            var entityDisplayName = entityType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
            var explicitMappings = ReadConfigurationMappings(symbol, compilation, request, entityDisplayName, cancellationToken);
            if (explicitMappings.Count == 0) {
                mappings.Add(
                    new EntityConfigurationMapping(
                        projectContext.Fact.ProjectId,
                        entityDisplayName,
                        null,
                        null,
                        CreateSourceReference(symbol, request)));
                continue;
            }

            mappings.AddRange(
                explicitMappings.Select(
                    mapping => new EntityConfigurationMapping(
                        projectContext.Fact.ProjectId,
                        mapping.EntityDisplayName,
                        mapping.TableName,
                        mapping.Schema,
                        mapping.Source)));
        }

        return mappings
            .GroupBy(mapping => (mapping.ProjectId, mapping.EntityDisplayName), EqualityComparer<(string ProjectId, string EntityDisplayName)>.Default)
            .Select(
                group => group
                    .OrderByDescending(item => !string.IsNullOrWhiteSpace(item.Schema))
                    .ThenByDescending(item => !string.IsNullOrWhiteSpace(item.TableName))
                    .ThenBy(item => item.Source?.Path, StringComparer.Ordinal)
                    .First())
            .OrderBy(item => item.EntityDisplayName, StringComparer.Ordinal)
            .ToArray();
    }

    public static EntityStoreObjectMapping? TryReadTableAttribute(INamedTypeSymbol entitySymbol, AnalysisRequest request) {
        var attribute = entitySymbol.GetAttributes()
            .FirstOrDefault(
                candidate => string.Equals(
                    candidate.AttributeClass?.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
                    TableAttributeTypeName,
                    StringComparison.Ordinal));
        if (attribute is null) {
            return null;
        }

        var tableName = attribute.ConstructorArguments.Length > 0
            ? attribute.ConstructorArguments[0].Value as string
            : null;
        var schema = attribute.NamedArguments
            .FirstOrDefault(argument => string.Equals(argument.Key, nameof(TableAttribute.Schema), StringComparison.Ordinal))
            .Value.Value as string;
        return new EntityStoreObjectMapping(
            entitySymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
            tableName,
            schema,
            CreateSourceReference(entitySymbol, request));
    }

    private static IReadOnlyList<EntityStoreObjectMapping> ReadConfigurationMappings(
        INamedTypeSymbol configurationSymbol,
        Compilation compilation,
        AnalysisRequest request,
        string entityDisplayName,
        CancellationToken cancellationToken) {
        var mappings = new List<EntityStoreObjectMapping>();

        foreach (var syntaxReference in configurationSymbol.DeclaringSyntaxReferences) {
            if (syntaxReference.GetSyntax(cancellationToken) is not ClassDeclarationSyntax classDeclaration) {
                continue;
            }

            var semanticModel = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
            var configureMethods = classDeclaration.Members
                .OfType<MethodDeclarationSyntax>()
                .Where(method => string.Equals(method.Identifier.ValueText, "Configure", StringComparison.Ordinal));

            foreach (var method in configureMethods) {
                foreach (var invocation in method.DescendantNodes().OfType<InvocationExpressionSyntax>()) {
                    var methodName = GetMethodName(invocation);
                    if (methodName is not "ToTable" and not "ToView") {
                        continue;
                    }

                    var resolvedDisplayName = TryGetConfiguredEntityTypeDisplayName(invocation, semanticModel) ?? entityDisplayName;
                    mappings.Add(
                        new EntityStoreObjectMapping(
                            resolvedDisplayName,
                            TryGetStringArgument(invocation.ArgumentList.Arguments, 0),
                            TryGetStringArgument(invocation.ArgumentList.Arguments, 1),
                            CreateSourceReference(invocation, request)));
                }
            }
        }

        return mappings;
    }

    private static INamedTypeSymbol? TryGetConfiguredEntityType(INamedTypeSymbol symbol) {
        return symbol.AllInterfaces
            .FirstOrDefault(
                @interface => @interface.IsGenericType &&
                    string.Equals(@interface.OriginalDefinition.Name, "IEntityTypeConfiguration", StringComparison.Ordinal) &&
                    @interface.OriginalDefinition.Arity == 1 &&
                    string.Equals(@interface.OriginalDefinition.ContainingNamespace.ToDisplayString(), "Microsoft.EntityFrameworkCore", StringComparison.Ordinal))
            ?.TypeArguments[0] as INamedTypeSymbol;
    }

    private static string? TryGetConfiguredEntityTypeDisplayName(
        InvocationExpressionSyntax invocation,
        SemanticModel semanticModel) {
        if (semanticModel.GetSymbolInfo(invocation).Symbol is not IMethodSymbol methodSymbol) {
            return null;
        }

        var containingType = methodSymbol.ContainingType;
        if (!string.Equals(containingType.Name, "EntityTypeBuilder", StringComparison.Ordinal) || !containingType.IsGenericType) {
            return null;
        }

        return containingType.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
    }

    private static bool IsCurrentAssemblyExpression(ExpressionSyntax? expression, INamedTypeSymbol dbContextSymbol) {
        if (expression is MemberAccessExpressionSyntax memberAccess &&
            string.Equals(memberAccess.Name.Identifier.ValueText, "Assembly", StringComparison.Ordinal)) {
            if (memberAccess.Expression is TypeOfExpressionSyntax typeOfExpression) {
                var typeName = typeOfExpression.Type.ToString();
                return string.Equals(typeName, dbContextSymbol.Name, StringComparison.Ordinal) ||
                    string.Equals(typeName, dbContextSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat), StringComparison.Ordinal) ||
                    string.Equals(typeName, dbContextSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat), StringComparison.Ordinal);
            }

            if (memberAccess.Expression is InvocationExpressionSyntax invocation &&
                string.Equals(GetMethodName(invocation), "GetType", StringComparison.Ordinal)) {
                return true;
            }
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

            if (string.Equals(memberAccess.Name.Identifier.ValueText, "Entity", StringComparison.Ordinal)) {
                return TryGetNonGenericEntityTypeDisplayName(currentInvocation, semanticModel);
            }

            current = memberAccess.Expression;
        }

        return null;
    }

    private static string? TryGetNonGenericEntityTypeDisplayName(
        InvocationExpressionSyntax invocation,
        SemanticModel semanticModel) {
        if (!string.Equals(GetMethodName(invocation), "Entity", StringComparison.Ordinal)) {
            return null;
        }

        if (invocation.ArgumentList.Arguments.Count == 0) {
            return null;
        }

        var argumentExpression = invocation.ArgumentList.Arguments[0].Expression;
        if (argumentExpression is TypeOfExpressionSyntax typeOfExpression) {
            return semanticModel.GetTypeInfo(typeOfExpression.Type).Type?.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
        }

        var constantValue = semanticModel.GetConstantValue(argumentExpression);
        return constantValue.Value is ITypeSymbol typeSymbol
            ? typeSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)
            : null;
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

    private static SourceReference? CreateSourceReference(ISymbol symbol, AnalysisRequest request) {
        var location = symbol.Locations.FirstOrDefault(candidate => candidate.IsInSource && candidate.SourceTree?.FilePath is not null);
        return location is null
            ? null
            : CreateSourceReference(location, request);
    }

    private static SourceReference? CreateSourceReference(SyntaxNode syntaxNode, AnalysisRequest request) {
        return CreateSourceReference(syntaxNode.GetLocation(), request);
    }

    private static SourceReference? CreateSourceReference(Location location, AnalysisRequest request) {
        var lineSpan = location.GetLineSpan();
        if (string.IsNullOrWhiteSpace(lineSpan.Path)) {
            return null;
        }

        var solutionDirectory = Path.GetDirectoryName(request.SolutionPath)!;
        return new SourceReference(
            Path.GetRelativePath(solutionDirectory, lineSpan.Path).Replace('\\', '/'),
            lineSpan.StartLinePosition.Line + 1,
            lineSpan.StartLinePosition.Character + 1);
    }
}

internal sealed record EntityConfigurationMapping(
    string ProjectId,
    string EntityDisplayName,
    string? TableName,
    string? Schema,
    SourceReference? Source);

internal sealed record EntityStoreObjectMapping(
    string EntityDisplayName,
    string? TableName,
    string? Schema,
    SourceReference? Source);

internal sealed record DbContextModelDiscovery(
    IReadOnlyList<string> EntityDisplayNames,
    IReadOnlyList<EntityStoreObjectMapping> StoreObjectMappings,
    string? DefaultSchema,
    bool IncludesSameProjectConfigurations,
    bool IncludesExternalConfigurations,
    IReadOnlyList<CanDoItAll.CodeAnalytics.Domain.Diagnostics.AnalysisDiagnostic> Diagnostics);
