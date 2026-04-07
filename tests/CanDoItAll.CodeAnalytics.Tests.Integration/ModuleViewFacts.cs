using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Facts.Dependencies;
using CanDoItAll.CodeAnalytics.Facts.Documentation;
using CanDoItAll.CodeAnalytics.Facts.Symbols;
using CanDoItAll.CodeAnalytics.Tests.Support;
using CanDoItAll.CodeAnalytics.Workspace.Inventory;
using CanDoItAll.CodeAnalytics.Workspace.Loading;
using CanDoItAll.CodeAnalytics.Workspace.Normalization;

namespace CanDoItAll.CodeAnalytics.Tests.Integration;

public sealed class ModuleViewFacts {
    [Fact]
    public async Task ModuleView_captures_module_dependencies() {
        FixtureSolutionHost.EnsurePrepared();
        var loader = new MsBuildWorkspaceLoader(new AnalysisRequestNormalizer(), new ProjectFileInventoryReader());
        var symbolCollector = new SymbolFactsCollector(new XmlDocumentationNormalizer());
        var dependencyCollector = new DependencyFactCollector();

        using var workspace = await loader.LoadAsync(new AnalysisRequest(FixturePaths.GetFixtureSolutionPath(), [], [], true, true, true, true, true));
        var symbols = await symbolCollector.CollectAsync(workspace);
        var dependencies = await dependencyCollector.CollectAsync(workspace, symbols);

        Assert.Contains(dependencies.Modules, module => module.Name.Contains("Application.Orders", StringComparison.Ordinal));
        Assert.Contains(dependencies.Dependencies, edge => edge.Kind == CanDoItAll.CodeAnalytics.Domain.Facts.DependencyKind.ModuleDependency);
    }
}
