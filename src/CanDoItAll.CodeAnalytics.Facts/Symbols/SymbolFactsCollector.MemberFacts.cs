using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Identifiers;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Domain.Sources;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace CanDoItAll.CodeAnalytics.Facts.Symbols;

public sealed partial class SymbolFactsCollector {
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
