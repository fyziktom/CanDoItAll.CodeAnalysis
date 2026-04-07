using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Identifiers;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Domain.Sources;
using CanDoItAll.CodeAnalytics.Workspace.Loading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CanDoItAll.CodeAnalytics.Facts.Services;

public sealed class ServiceRegistrationCollector {
    private readonly ILogger<ServiceRegistrationCollector> _logger;

    public ServiceRegistrationCollector(ILogger<ServiceRegistrationCollector>? logger = null) {
        _logger = logger ?? NullLogger<ServiceRegistrationCollector>.Instance;
    }

    public async Task<ServiceRegistrationCollectionResult> CollectAsync(
        WorkspaceLoadResult workspace,
        CancellationToken cancellationToken = default) {
        if (!workspace.Request.IncludeDi) {
            return new ServiceRegistrationCollectionResult([], []);
        }

        var services = new List<ServiceRegistrationFact>();
        var diagnostics = new List<AnalysisDiagnostic>();

        foreach (var projectContext in workspace.ProjectContexts.OrderBy(context => context.Fact.Name, StringComparer.OrdinalIgnoreCase)) {
            if (!ShouldIncludeProject(workspace.Request, projectContext.Fact)) {
                continue;
            }

            var moduleId = StableId.ForModule($"{projectContext.Fact.ProjectId}:{projectContext.Fact.Name}");

            foreach (var documentContext in projectContext.Documents.OrderBy(context => context.Fact.Path, StringComparer.OrdinalIgnoreCase)) {
                var syntaxRoot = await documentContext.Document.GetSyntaxRootAsync(cancellationToken);
                if (syntaxRoot is null) {
                    continue;
                }

                var semanticModel = await documentContext.Document.GetSemanticModelAsync(cancellationToken);
                if (semanticModel is null) {
                    continue;
                }

                var invocations = syntaxRoot.DescendantNodes()
                    .OfType<InvocationExpressionSyntax>()
                    .Where(IsRegistrationInvocation);

                foreach (var invocation in invocations) {
                    var methodName = GetMethodName(invocation);
                    if (methodName is null) {
                        continue;
                    }

                    var lifetime = MapLifetime(methodName);
                    var registration = TryCreateRegistration(
                        workspace.Request,
                        projectContext.Fact.ProjectId,
                        moduleId,
                        invocation,
                        semanticModel,
                        methodName,
                        lifetime,
                        diagnostics);
                    if (registration is not null) {
                        services.Add(registration);
                    }
                }
            }
        }

        return new ServiceRegistrationCollectionResult(
            services.OrderBy(service => service.ServiceTypeDisplayName, StringComparer.Ordinal).ThenBy(service => service.Source.Path, StringComparer.Ordinal).ToArray(),
            diagnostics.OrderBy(diagnostic => diagnostic.Code, StringComparer.Ordinal).ThenBy(diagnostic => diagnostic.Message, StringComparer.Ordinal).ToArray());
    }

    private static bool IsRegistrationInvocation(InvocationExpressionSyntax invocation) {
        var methodName = GetMethodName(invocation);
        return methodName is "AddSingleton" or "AddScoped" or "AddTransient";
    }

    private static string? GetMethodName(InvocationExpressionSyntax invocation) {
        return invocation.Expression switch {
            MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier.ValueText,
            GenericNameSyntax genericName => genericName.Identifier.ValueText,
            IdentifierNameSyntax identifierName => identifierName.Identifier.ValueText,
            _ => null,
        };
    }

    private static ServiceRegistrationFact? TryCreateRegistration(
        AnalysisRequest request,
        string projectId,
        string moduleId,
        InvocationExpressionSyntax invocation,
        SemanticModel semanticModel,
        string methodName,
        ServiceLifetimeKind lifetime,
        ICollection<AnalysisDiagnostic> diagnostics) {
        string? serviceType = null;
        string? implementationType = null;
        var usesFactory = false;
        var source = CreateSourceReference(invocation, request);

        if (invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
            memberAccess.Name is GenericNameSyntax genericName) {
            var typeArguments = genericName.TypeArgumentList.Arguments;
            if (typeArguments.Count >= 1) {
                serviceType = ResolveTypeDisplayName(semanticModel, typeArguments[0]);
            }

            if (typeArguments.Count >= 2) {
                implementationType = ResolveTypeDisplayName(semanticModel, typeArguments[1]);
            }
        }

        var arguments = invocation.ArgumentList.Arguments;
        if (serviceType is null && arguments.Count >= 1) {
            serviceType = ResolveTypeDisplayName(semanticModel, arguments[0].Expression);
        }

        if (implementationType is null && arguments.Count >= 2) {
            implementationType = ResolveTypeDisplayName(semanticModel, arguments[1].Expression);
        }

        if (arguments.Any(argument => IsFactoryArgument(argument.Expression))) {
            usesFactory = true;
            diagnostics.Add(
                new AnalysisDiagnostic(
                    "DI0001",
                    AnalysisDiagnosticSeverity.Info,
                    $"Factory-based registration at {source?.Path ?? "unknown"} is only partially interpreted.",
                    source));
        }

        if (string.IsNullOrWhiteSpace(serviceType)) {
            diagnostics.Add(
                new AnalysisDiagnostic(
                    "DI0002",
                    AnalysisDiagnosticSeverity.Warning,
                    $"Service type could not be resolved for {methodName}.",
                    source));
            return null;
        }

        var idSeed = $"{methodName}:{serviceType}:{implementationType}:{source?.Path}:{source?.Line}";
        return new ServiceRegistrationFact(
            StableId.ForServiceRegistration(idSeed),
            projectId,
            moduleId,
            lifetime,
            serviceType,
            implementationType,
            methodName,
            usesFactory,
            source ?? new SourceReference("unknown"));
    }

    private static bool IsFactoryArgument(ExpressionSyntax expression) {
        return expression is SimpleLambdaExpressionSyntax
            or ParenthesizedLambdaExpressionSyntax
            or AnonymousMethodExpressionSyntax;
    }

    private static bool ShouldIncludeProject(AnalysisRequest request, ProjectFact project) {
        if (request.ScopeProjectNames.Count == 0) {
            return true;
        }

        return request.ScopeProjectNames.Contains(project.Name, StringComparer.OrdinalIgnoreCase);
    }

    private static string? ResolveTypeDisplayName(SemanticModel semanticModel, ExpressionSyntax expression) {
        return expression switch {
            TypeOfExpressionSyntax typeOfExpression => ResolveTypeDisplayName(semanticModel, typeOfExpression.Type),
            _ when IsFactoryArgument(expression) => null,
            _ => null,
        };
    }

    private static string? ResolveTypeDisplayName(SemanticModel semanticModel, TypeSyntax typeSyntax) {
        var type = semanticModel.GetTypeInfo(typeSyntax).Type as ITypeSymbol;
        return type?.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
    }

    private static ServiceLifetimeKind MapLifetime(string methodName) {
        return methodName switch {
            "AddSingleton" => ServiceLifetimeKind.Singleton,
            "AddScoped" => ServiceLifetimeKind.Scoped,
            "AddTransient" => ServiceLifetimeKind.Transient,
            _ => ServiceLifetimeKind.Unknown,
        };
    }

    private static SourceReference? CreateSourceReference(SyntaxNode syntaxNode, AnalysisRequest request) {
        var location = syntaxNode.GetLocation();
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
