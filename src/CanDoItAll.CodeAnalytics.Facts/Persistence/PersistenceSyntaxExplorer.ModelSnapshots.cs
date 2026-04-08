using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Domain.Sources;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CanDoItAll.CodeAnalytics.Facts.Persistence;

internal static partial class PersistenceSyntaxExplorer {
    public static bool TryDiscoverModelSnapshot(
        INamedTypeSymbol symbol,
        Compilation compilation,
        AnalysisRequest request,
        CancellationToken cancellationToken,
        out ModelSnapshotDiscovery discovery) {
        if (!IsModelSnapshot(symbol) || !TryGetDbContextDisplayName(symbol, out var dbContextDisplayName)) {
            discovery = null!;
            return false;
        }

        var entityDisplayNames = new HashSet<string>(StringComparer.Ordinal);
        var storeObjectMappings = new List<EntityStoreObjectMapping>();
        var relationshipMappings = new List<ModelSnapshotRelationshipMapping>();

        foreach (var syntaxReference in symbol.DeclaringSyntaxReferences) {
            if (syntaxReference.GetSyntax(cancellationToken) is not ClassDeclarationSyntax classDeclaration) {
                continue;
            }

            var semanticModel = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
            foreach (var buildModelMethod in classDeclaration.Members
                .OfType<MethodDeclarationSyntax>()
                .Where(method => string.Equals(method.Identifier.ValueText, "BuildModel", StringComparison.Ordinal))) {
                foreach (var entityInvocation in buildModelMethod.DescendantNodes().OfType<InvocationExpressionSyntax>()) {
                    if (!string.Equals(GetMethodName(entityInvocation), "Entity", StringComparison.Ordinal) ||
                        !TryGetSnapshotEntityDisplayName(entityInvocation, semanticModel, out var entityDisplayName)) {
                        continue;
                    }

                    entityDisplayNames.Add(entityDisplayName);
                    if (TryGetEntityLambda(entityInvocation) is not { } lambda) {
                        continue;
                    }

                    storeObjectMappings.AddRange(ReadSnapshotStoreObjectMappings(entityDisplayName, lambda, request));
                    relationshipMappings.AddRange(ReadSnapshotRelationshipMappings(entityDisplayName, lambda, request, semanticModel));
                }
            }
        }

        discovery = new ModelSnapshotDiscovery(
            dbContextDisplayName,
            entityDisplayNames.OrderBy(item => item, StringComparer.Ordinal).ToArray(),
            storeObjectMappings
                .GroupBy(item => item.EntityDisplayName, StringComparer.Ordinal)
                .Select(group => group.First())
                .OrderBy(item => item.EntityDisplayName, StringComparer.Ordinal)
                .ToArray(),
            relationshipMappings
                .GroupBy(item => (item.FromEntityDisplayName, item.ToEntityDisplayName, item.Kind), EqualityComparer<(string FromEntityDisplayName, string ToEntityDisplayName, EntityRelationshipKind Kind)>.Default)
                .Select(group => group.First())
                .OrderBy(item => item.FromEntityDisplayName, StringComparer.Ordinal)
                .ThenBy(item => item.ToEntityDisplayName, StringComparer.Ordinal)
                .ThenBy(item => item.Kind)
                .ToArray(),
            CreateSourceReference(symbol, request));
        return true;
    }

    private static IEnumerable<EntityStoreObjectMapping> ReadSnapshotStoreObjectMappings(
        string entityDisplayName,
        LambdaExpressionSyntax lambda,
        AnalysisRequest request) {
        foreach (var invocation in lambda.Body.DescendantNodes().OfType<InvocationExpressionSyntax>()) {
            var methodName = GetMethodName(invocation);
            if (methodName is not "ToTable" and not "ToView") {
                continue;
            }

            yield return new EntityStoreObjectMapping(
                entityDisplayName,
                TryGetStringArgument(invocation.ArgumentList.Arguments, 0),
                TryGetStringArgument(invocation.ArgumentList.Arguments, 1),
                CreateSourceReference(invocation, request));
        }
    }

    private static IEnumerable<ModelSnapshotRelationshipMapping> ReadSnapshotRelationshipMappings(
        string entityDisplayName,
        LambdaExpressionSyntax lambda,
        AnalysisRequest request,
        SemanticModel semanticModel) {
        foreach (var invocation in lambda.Body.DescendantNodes().OfType<InvocationExpressionSyntax>()) {
            var startMethodName = GetMethodName(invocation);
            if (startMethodName is not "HasOne" and not "HasMany") {
                continue;
            }

            var targetEntityDisplayName = TryGetSnapshotRelationshipTargetEntityDisplayName(invocation, semanticModel);
            if (string.IsNullOrWhiteSpace(targetEntityDisplayName)) {
                continue;
            }

            var chain = GetInvocationChain(invocation);
            var relationship = CreateSnapshotRelationship(
                entityDisplayName,
                targetEntityDisplayName,
                startMethodName,
                chain,
                request,
                invocation);
            if (relationship is not null) {
                yield return relationship;
            }
        }
    }

    private static ModelSnapshotRelationshipMapping? CreateSnapshotRelationship(
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
            .SelectMany(invocation => TryGetSnapshotNavigationNames(invocation.ArgumentList.Arguments))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();

        if (startMethodName == "HasOne" && methodNames.Contains("WithMany", StringComparer.Ordinal)) {
            return new ModelSnapshotRelationshipMapping(
                targetEntityDisplayName,
                entityDisplayName,
                EntityRelationshipKind.OneToMany,
                navigationNames,
                CreateSourceReference(sourceInvocation, request));
        }

        if (startMethodName == "HasMany" && methodNames.Contains("WithOne", StringComparer.Ordinal)) {
            return new ModelSnapshotRelationshipMapping(
                entityDisplayName,
                targetEntityDisplayName,
                EntityRelationshipKind.OneToMany,
                navigationNames,
                CreateSourceReference(sourceInvocation, request));
        }

        if (methodNames.Contains("WithOne", StringComparer.Ordinal)) {
            return new ModelSnapshotRelationshipMapping(
                entityDisplayName,
                targetEntityDisplayName,
                EntityRelationshipKind.OneToOne,
                navigationNames,
                CreateSourceReference(sourceInvocation, request));
        }

        if (methodNames.Contains("UsingEntity", StringComparer.Ordinal) ||
            (startMethodName == "HasMany" && methodNames.Contains("WithMany", StringComparer.Ordinal))) {
            return new ModelSnapshotRelationshipMapping(
                entityDisplayName,
                targetEntityDisplayName,
                EntityRelationshipKind.ManyToMany,
                navigationNames,
                CreateSourceReference(sourceInvocation, request));
        }

        return new ModelSnapshotRelationshipMapping(
            entityDisplayName,
            targetEntityDisplayName,
            EntityRelationshipKind.Reference,
            navigationNames,
            CreateSourceReference(sourceInvocation, request));
    }

    private static bool IsModelSnapshot(INamedTypeSymbol symbol) {
        var current = symbol;
        while (current is not null) {
            if (string.Equals(current.ToDisplayString(), "Microsoft.EntityFrameworkCore.Infrastructure.ModelSnapshot", StringComparison.Ordinal)) {
                return true;
            }

            current = current.BaseType;
        }

        return false;
    }

    private static bool TryGetDbContextDisplayName(INamedTypeSymbol symbol, out string dbContextDisplayName) {
        var attribute = symbol.GetAttributes()
            .FirstOrDefault(
                candidate => string.Equals(candidate.AttributeClass?.Name, "DbContextAttribute", StringComparison.Ordinal) &&
                    candidate.ConstructorArguments.Length > 0);
        if (attribute?.ConstructorArguments[0].Value is not INamedTypeSymbol dbContextSymbol) {
            dbContextDisplayName = string.Empty;
            return false;
        }

        dbContextDisplayName = dbContextSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
        return true;
    }

    private static bool TryGetSnapshotEntityDisplayName(
        InvocationExpressionSyntax invocation,
        SemanticModel semanticModel,
        out string entityDisplayName) {
        var stringName = TryGetStringArgument(invocation.ArgumentList.Arguments, 0);
        if (!string.IsNullOrWhiteSpace(stringName)) {
            entityDisplayName = stringName;
            return true;
        }

        var typeName = TryGetEntityTypeDisplayName(invocation, semanticModel);
        if (!string.IsNullOrWhiteSpace(typeName)) {
            entityDisplayName = typeName;
            return true;
        }

        entityDisplayName = string.Empty;
        return false;
    }

    private static string? TryGetSnapshotRelationshipTargetEntityDisplayName(
        InvocationExpressionSyntax invocation,
        SemanticModel semanticModel) {
        return TryGetStringArgument(invocation.ArgumentList.Arguments, 0) ?? TryGetEntityTypeDisplayName(invocation, semanticModel);
    }

    private static LambdaExpressionSyntax? TryGetEntityLambda(InvocationExpressionSyntax invocation) {
        return invocation.ArgumentList.Arguments
            .Select(argument => argument.Expression)
            .OfType<LambdaExpressionSyntax>()
            .FirstOrDefault();
    }

    private static IReadOnlyList<string> TryGetSnapshotNavigationNames(SeparatedSyntaxList<ArgumentSyntax> arguments) {
        return arguments
            .Select(argument => argument.Expression switch {
                LiteralExpressionSyntax literal when literal.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.StringLiteralExpression) => literal.Token.ValueText,
                _ => TryGetNavigationName(argument.Expression),
            })
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Cast<string>()
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();
    }
}

internal sealed record ModelSnapshotDiscovery(
    string DbContextDisplayName,
    IReadOnlyList<string> EntityDisplayNames,
    IReadOnlyList<EntityStoreObjectMapping> StoreObjectMappings,
    IReadOnlyList<ModelSnapshotRelationshipMapping> RelationshipMappings,
    SourceReference? Source);

internal sealed record ModelSnapshotRelationshipMapping(
    string FromEntityDisplayName,
    string ToEntityDisplayName,
    EntityRelationshipKind Kind,
    IReadOnlyList<string> NavigationPropertyNames,
    SourceReference? Source);
