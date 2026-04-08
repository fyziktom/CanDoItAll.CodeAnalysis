using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Domain.Sources;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CanDoItAll.CodeAnalytics.Facts.Persistence;

internal static partial class PersistenceSyntaxExplorer {
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
