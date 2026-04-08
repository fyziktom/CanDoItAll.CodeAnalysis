using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CanDoItAll.CodeAnalytics.Facts.Persistence;

internal static partial class PersistenceSyntaxExplorer {
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
}
