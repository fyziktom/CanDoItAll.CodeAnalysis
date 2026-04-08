using System.ComponentModel.DataAnnotations.Schema;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Domain.Sources;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CanDoItAll.CodeAnalytics.Facts.Persistence;

internal static partial class PersistenceSyntaxExplorer {
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
        Workspace.Loading.WorkspaceProjectContext projectContext,
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
