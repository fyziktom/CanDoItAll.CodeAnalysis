using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Domain.Sources;
using CanDoItAll.CodeAnalytics.Workspace.Loading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CanDoItAll.CodeAnalytics.Facts.Persistence;

internal static partial class PersistenceSyntaxExplorer {
    public static IReadOnlyList<ConfiguredEntityRelationshipMapping> DiscoverEntityRelationships(
        WorkspaceProjectContext projectContext,
        Compilation compilation,
        AnalysisRequest request,
        ISet<string> projectDocumentPaths,
        CancellationToken cancellationToken) {
        var relationships = new List<ConfiguredEntityRelationshipMapping>();

        foreach (var symbol in EnumerateTypes(compilation.GlobalNamespace)) {
            if (!IsOwnedByProject(symbol, projectDocumentPaths)) {
                continue;
            }

            var entityType = TryGetConfiguredEntityType(symbol);
            if (entityType is null) {
                continue;
            }

            var entityDisplayName = entityType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
            relationships.AddRange(
                ReadConfigurationRelationships(
                    symbol,
                    compilation,
                    request,
                    projectContext.Fact.ProjectId,
                    entityDisplayName,
                    cancellationToken));
        }

        return relationships
            .GroupBy(
                item => (item.ProjectId, item.FromEntityDisplayName, item.ToEntityDisplayName, item.Kind),
                EqualityComparer<(string ProjectId, string FromEntityDisplayName, string ToEntityDisplayName, EntityRelationshipKind Kind)>.Default)
            .Select(
                group => {
                    var navigationNames = group
                        .SelectMany(item => item.NavigationPropertyNames)
                        .Distinct(StringComparer.Ordinal)
                        .OrderBy(value => value, StringComparer.Ordinal)
                        .ToArray();
                    var preferred = group
                        .OrderByDescending(item => item.NavigationPropertyNames.Count)
                        .ThenBy(item => item.Source?.Path, StringComparer.Ordinal)
                        .First();

                    return preferred with {
                        NavigationPropertyNames = navigationNames,
                    };
                })
            .OrderBy(item => item.FromEntityDisplayName, StringComparer.Ordinal)
            .ThenBy(item => item.ToEntityDisplayName, StringComparer.Ordinal)
            .ThenBy(item => item.Kind)
            .ToArray();
    }

    private static IReadOnlyList<ConfiguredEntityRelationshipMapping> ReadConfigurationRelationships(
        INamedTypeSymbol configurationSymbol,
        Compilation compilation,
        AnalysisRequest request,
        string projectId,
        string entityDisplayName,
        CancellationToken cancellationToken) {
        var relationships = new List<ConfiguredEntityRelationshipMapping>();

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
                    var startMethodName = GetMethodName(invocation);
                    if (startMethodName is not "HasOne" and not "HasMany") {
                        continue;
                    }

                    var targetEntityDisplayName = TryGetRelationshipTargetEntityDisplayName(invocation, semanticModel);
                    if (string.IsNullOrWhiteSpace(targetEntityDisplayName)) {
                        continue;
                    }

                    var chain = GetInvocationChain(invocation);
                    var relationship = CreateConfiguredRelationship(
                        projectId,
                        entityDisplayName,
                        targetEntityDisplayName,
                        startMethodName,
                        chain,
                        request,
                        invocation);
                    if (relationship is not null) {
                        relationships.Add(relationship);
                    }
                }
            }
        }

        return relationships;
    }

    private static ConfiguredEntityRelationshipMapping? CreateConfiguredRelationship(
        string projectId,
        string entityDisplayName,
        string targetEntityDisplayName,
        string startMethodName,
        IReadOnlyList<InvocationExpressionSyntax> chain,
        AnalysisRequest request,
        InvocationExpressionSyntax sourceInvocation) {
        var methodNames = chain
            .Select(GetMethodName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Cast<string>()
            .ToArray();
        var navigationNames = chain
            .SelectMany(invocation => TryGetNavigationNames(invocation.ArgumentList.Arguments))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();

        if (startMethodName == "HasOne" && methodNames.Contains("WithMany", StringComparer.Ordinal)) {
            return new ConfiguredEntityRelationshipMapping(
                projectId,
                targetEntityDisplayName,
                entityDisplayName,
                EntityRelationshipKind.OneToMany,
                navigationNames,
                CreateSourceReference(sourceInvocation, request));
        }

        if (startMethodName == "HasMany" && methodNames.Contains("WithOne", StringComparer.Ordinal)) {
            return new ConfiguredEntityRelationshipMapping(
                projectId,
                entityDisplayName,
                targetEntityDisplayName,
                EntityRelationshipKind.OneToMany,
                navigationNames,
                CreateSourceReference(sourceInvocation, request));
        }

        if (methodNames.Contains("WithOne", StringComparer.Ordinal)) {
            return new ConfiguredEntityRelationshipMapping(
                projectId,
                entityDisplayName,
                targetEntityDisplayName,
                EntityRelationshipKind.OneToOne,
                navigationNames,
                CreateSourceReference(sourceInvocation, request));
        }

        if (methodNames.Contains("UsingEntity", StringComparer.Ordinal) ||
            (startMethodName == "HasMany" && methodNames.Contains("WithMany", StringComparer.Ordinal))) {
            return new ConfiguredEntityRelationshipMapping(
                projectId,
                entityDisplayName,
                targetEntityDisplayName,
                EntityRelationshipKind.ManyToMany,
                navigationNames,
                CreateSourceReference(sourceInvocation, request));
        }

        return null;
    }

    private static IReadOnlyList<InvocationExpressionSyntax> GetInvocationChain(InvocationExpressionSyntax invocation) {
        var chain = new List<InvocationExpressionSyntax>();
        SyntaxNode? current = invocation;

        while (current is InvocationExpressionSyntax currentInvocation) {
            chain.Add(currentInvocation);

            if (currentInvocation.Parent is not MemberAccessExpressionSyntax memberAccess ||
                memberAccess.Parent is not InvocationExpressionSyntax parentInvocation) {
                break;
            }

            current = parentInvocation;
        }

        return chain;
    }

    private static string? TryGetRelationshipTargetEntityDisplayName(
        InvocationExpressionSyntax invocation,
        SemanticModel semanticModel) {
        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess) {
            return null;
        }

        if (memberAccess.Name is GenericNameSyntax genericName &&
            genericName.TypeArgumentList.Arguments.Count == 1) {
            return semanticModel.GetTypeInfo(genericName.TypeArgumentList.Arguments[0]).Type?.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
        }

        if (invocation.ArgumentList.Arguments.Count == 0) {
            return null;
        }

        var candidateType = semanticModel.GetTypeInfo(invocation.ArgumentList.Arguments[0].Expression).Type;
        return candidateType?.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
    }

    private static IReadOnlyList<string> TryGetNavigationNames(SeparatedSyntaxList<ArgumentSyntax> arguments) {
        return arguments
            .Select(argument => TryGetNavigationName(argument.Expression))
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Cast<string>()
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();
    }

    private static string? TryGetNavigationName(ExpressionSyntax expression) {
        return expression switch {
            SimpleLambdaExpressionSyntax lambda => TryGetMemberAccessName(lambda.Body),
            ParenthesizedLambdaExpressionSyntax lambda => TryGetMemberAccessName(lambda.Body),
            _ => null,
        };
    }

    private static string? TryGetMemberAccessName(SyntaxNode syntax) {
        return syntax switch {
            MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier.ValueText,
            ConditionalAccessExpressionSyntax conditionalAccess when conditionalAccess.WhenNotNull is MemberBindingExpressionSyntax memberBinding
                => memberBinding.Name.Identifier.ValueText,
            IdentifierNameSyntax identifierName => identifierName.Identifier.ValueText,
            _ => null,
        };
    }
}

internal sealed record ConfiguredEntityRelationshipMapping(
    string ProjectId,
    string FromEntityDisplayName,
    string ToEntityDisplayName,
    EntityRelationshipKind Kind,
    IReadOnlyList<string> NavigationPropertyNames,
    SourceReference? Source);
