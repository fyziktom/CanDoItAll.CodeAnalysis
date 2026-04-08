using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Identifiers;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Domain.Sources;
using CanDoItAll.CodeAnalytics.Facts.Symbols;
using CanDoItAll.CodeAnalytics.Workspace.Loading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CanDoItAll.CodeAnalytics.Facts.Members;

public sealed class MemberRelationshipCollector {
    public async Task<MemberRelationshipCollectionResult> CollectAsync(
        WorkspaceLoadResult workspace,
        SymbolCollectionResult symbols,
        CancellationToken cancellationToken = default) {
        if (workspace.RoslynSolution is null) {
            return new MemberRelationshipCollectionResult([], []);
        }

        var diagnostics = new List<AnalysisDiagnostic>();
        var relationships = new Dictionary<(MemberRelationshipKind Kind, string FromMemberId, string ToMemberId), RelationshipAggregate>();
        var typesByDisplayName = symbols.Types
            .GroupBy(type => type.DisplayName, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.ToArray(), StringComparer.Ordinal);
        var membersByIdentity = symbols.Members
            .GroupBy(member => (member.TypeId, member.DisplayName), EqualityComparer<(string TypeId, string DisplayName)>.Default)
            .ToDictionary(group => group.Key, group => group.First(), EqualityComparer<(string TypeId, string DisplayName)>.Default);

        foreach (var projectContext in workspace.ProjectContexts.OrderBy(context => context.Fact.Name, StringComparer.OrdinalIgnoreCase)) {
            var compilation = await projectContext.Project.GetCompilationAsync(cancellationToken);
            if (compilation is null) {
                diagnostics.Add(
                    new AnalysisDiagnostic(
                        "MEM0001",
                        AnalysisDiagnosticSeverity.Warning,
                        $"Compilation was unavailable for project {projectContext.Fact.Name}."));
                continue;
            }

            foreach (var document in projectContext.Project.Documents.Where(item => item.SupportsSyntaxTree)) {
                var root = await document.GetSyntaxRootAsync(cancellationToken);
                if (root is null) {
                    continue;
                }

                var semanticModel = compilation.GetSemanticModel(root.SyntaxTree);
                foreach (var declaration in root.DescendantNodes().OfType<MemberDeclarationSyntax>()) {
                    if (!TryResolveSourceMember(
                            declaration,
                            semanticModel,
                            workspace.Request,
                            projectContext.Fact.ProjectId,
                            typesByDisplayName,
                            membersByIdentity,
                            out var sourceMember)) {
                        continue;
                    }

                    foreach (var candidate in CollectRelationshipCandidates(declaration, semanticModel, workspace.Request)) {
                        if (!TryResolveTargetMember(
                                candidate.Symbol,
                                projectContext.Fact.ProjectId,
                                typesByDisplayName,
                                membersByIdentity,
                                out var targetMember)) {
                            continue;
                        }

                        if (string.Equals(sourceMember.MemberId, targetMember.MemberId, StringComparison.Ordinal)) {
                            continue;
                        }

                        AddRelationship(relationships, candidate.Kind, sourceMember.MemberId, targetMember.MemberId, candidate.Source);
                    }
                }
            }
        }

        return new MemberRelationshipCollectionResult(
            relationships
                .OrderBy(item => item.Key.Kind)
                .ThenBy(item => item.Key.FromMemberId, StringComparer.Ordinal)
                .ThenBy(item => item.Key.ToMemberId, StringComparer.Ordinal)
                .Select(
                    item => new MemberRelationshipFact(
                        StableId.ForMemberRelationship($"{item.Key.Kind}:{item.Key.FromMemberId}:{item.Key.ToMemberId}"),
                        item.Key.FromMemberId,
                        item.Key.ToMemberId,
                        item.Key.Kind,
                        item.Value.Weight,
                        item.Value.Source))
                .ToArray(),
            diagnostics.OrderBy(item => item.Code, StringComparer.Ordinal).ThenBy(item => item.Message, StringComparer.Ordinal).ToArray());
    }

    private static bool TryResolveSourceMember(
        MemberDeclarationSyntax declaration,
        SemanticModel semanticModel,
        AnalysisRequest request,
        string projectId,
        IReadOnlyDictionary<string, TypeFact[]> typesByDisplayName,
        IReadOnlyDictionary<(string TypeId, string DisplayName), MemberFact> membersByIdentity,
        out MemberFact sourceMember) {
        var declaredSymbol = semanticModel.GetDeclaredSymbol(declaration);
        if (declaredSymbol is null || !TryMapMemberKind(declaredSymbol, out _)) {
            sourceMember = null!;
            return false;
        }

        return TryResolveMemberFact(declaredSymbol, request, projectId, typesByDisplayName, membersByIdentity, out sourceMember);
    }

    private static bool TryResolveTargetMember(
        ISymbol symbol,
        string projectId,
        IReadOnlyDictionary<string, TypeFact[]> typesByDisplayName,
        IReadOnlyDictionary<(string TypeId, string DisplayName), MemberFact> membersByIdentity,
        out MemberFact targetMember) {
        var normalizedSymbol = NormalizeTargetSymbol(symbol);
        if (normalizedSymbol is null) {
            targetMember = null!;
            return false;
        }

        return TryResolveMemberFact(normalizedSymbol, null, projectId, typesByDisplayName, membersByIdentity, out targetMember);
    }

    private static bool TryResolveMemberFact(
        ISymbol memberSymbol,
        AnalysisRequest? request,
        string projectId,
        IReadOnlyDictionary<string, TypeFact[]> typesByDisplayName,
        IReadOnlyDictionary<(string TypeId, string DisplayName), MemberFact> membersByIdentity,
        out MemberFact memberFact) {
        var containingType = memberSymbol.ContainingType;
        if (containingType is null) {
            memberFact = null!;
            return false;
        }

        var typeDisplayName = containingType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
        if (!TryResolveTypeFact(typeDisplayName, projectId, typesByDisplayName, out var typeFact)) {
            memberFact = null!;
            return false;
        }

        var memberDisplayName = memberSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
        if (!membersByIdentity.TryGetValue((typeFact.TypeId, memberDisplayName), out memberFact!)) {
            return false;
        }

        return true;
    }

    private static bool TryResolveTypeFact(
        string displayName,
        string projectId,
        IReadOnlyDictionary<string, TypeFact[]> typesByDisplayName,
        out TypeFact typeFact) {
        if (!typesByDisplayName.TryGetValue(displayName, out var candidates) || candidates.Length == 0) {
            typeFact = null!;
            return false;
        }

        typeFact = candidates.FirstOrDefault(candidate => string.Equals(candidate.ProjectId, projectId, StringComparison.Ordinal))
            ?? candidates[0];
        return true;
    }

    private static IEnumerable<RelationshipCandidate> CollectRelationshipCandidates(
        MemberDeclarationSyntax declaration,
        SemanticModel semanticModel,
        AnalysisRequest request) {
        foreach (var invocation in declaration.DescendantNodes().OfType<InvocationExpressionSyntax>()) {
            var symbol = semanticModel.GetSymbolInfo(invocation).Symbol;
            if (symbol is null) {
                continue;
            }

            yield return new RelationshipCandidate(MemberRelationshipKind.Invocation, symbol, CreateSourceReference(invocation.GetLocation(), request));
        }

        foreach (var creation in declaration.DescendantNodes().OfType<ObjectCreationExpressionSyntax>()) {
            var symbol = semanticModel.GetSymbolInfo(creation).Symbol;
            if (symbol is null) {
                continue;
            }

            yield return new RelationshipCandidate(MemberRelationshipKind.ObjectCreation, symbol, CreateSourceReference(creation.GetLocation(), request));
        }
    }

    private static ISymbol? NormalizeTargetSymbol(ISymbol symbol) {
        return symbol switch {
            IMethodSymbol method when method.MethodKind is MethodKind.PropertyGet or MethodKind.PropertySet => method.AssociatedSymbol,
            IMethodSymbol method when method.ReducedFrom is not null => method.ReducedFrom.OriginalDefinition,
            IMethodSymbol method => method.OriginalDefinition,
            IPropertySymbol property => property,
            IFieldSymbol field => field,
            _ when TryMapMemberKind(symbol, out _) => symbol,
            _ => null,
        };
    }

    private static bool TryMapMemberKind(ISymbol symbol, out MemberKind kind) {
        switch (symbol) {
            case IMethodSymbol method when method.MethodKind == MethodKind.Constructor:
                kind = MemberKind.Constructor;
                return true;
            case IMethodSymbol method when method.MethodKind == MethodKind.Ordinary:
                kind = MemberKind.Method;
                return true;
            case IPropertySymbol:
                kind = MemberKind.Property;
                return true;
            case IFieldSymbol:
                kind = MemberKind.Field;
                return true;
            case IEventSymbol:
                kind = MemberKind.Event;
                return true;
            default:
                kind = default;
                return false;
        }
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

    private static void AddRelationship(
        IDictionary<(MemberRelationshipKind Kind, string FromMemberId, string ToMemberId), RelationshipAggregate> relationships,
        MemberRelationshipKind kind,
        string fromMemberId,
        string toMemberId,
        SourceReference? source) {
        var key = (kind, fromMemberId, toMemberId);
        if (relationships.TryGetValue(key, out var aggregate)) {
            relationships[key] = aggregate with {
                Weight = aggregate.Weight + 1,
            };
            return;
        }

        relationships[key] = new RelationshipAggregate(1, source);
    }

    private sealed record RelationshipCandidate(MemberRelationshipKind Kind, ISymbol Symbol, SourceReference? Source);

    private sealed record RelationshipAggregate(int Weight, SourceReference? Source);
}
