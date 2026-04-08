using CanDoItAll.CodeAnalytics.Abstractions;
using CanDoItAll.CodeAnalytics.Abstractions.Options;
using CanDoItAll.CodeAnalytics.Analysis.Graphs;
using CanDoItAll.CodeAnalytics.Analysis.Rules;
using CanDoItAll.CodeAnalytics.Application.Services;
using CanDoItAll.CodeAnalytics.Facts.Dependencies;
using CanDoItAll.CodeAnalytics.Facts.Documentation;
using CanDoItAll.CodeAnalytics.Facts.Members;
using CanDoItAll.CodeAnalytics.Facts.Persistence;
using CanDoItAll.CodeAnalytics.Facts.Services;
using CanDoItAll.CodeAnalytics.Facts.Symbols;
using CanDoItAll.CodeAnalytics.Rendering.Exports;
using CanDoItAll.CodeAnalytics.Rendering.Markdown;
using CanDoItAll.CodeAnalytics.Rendering.Mermaid;
using CanDoItAll.CodeAnalytics.Storage.Snapshots;
using CanDoItAll.CodeAnalytics.Workspace.Inventory;
using CanDoItAll.CodeAnalytics.Workspace.Loading;
using CanDoItAll.CodeAnalytics.Workspace.Normalization;

namespace CanDoItAll.CodeAnalytics.Tests.Support;

public static class ApplicationServiceFactory {
    public static ICodeAnalyticsApplicationService Create(string outputRootPath) {
        return new CodeAnalyticsApplicationService(
            new CodeAnalyticsApplicationOptions(outputRootPath, "0.1.0"),
            new MsBuildWorkspaceLoader(new AnalysisRequestNormalizer(), new ProjectFileInventoryReader()),
            new SymbolFactsCollector(new XmlDocumentationNormalizer()),
            new MemberRelationshipCollector(),
            new DependencyFactCollector(),
            new ServiceRegistrationCollector(),
            new PersistenceFactCollector(),
            new ArchitectureInsightBuilder(new StronglyConnectedComponentFinder()),
            new ExportBundleBuilder(
                new MarkdownSummaryWriter(),
                new ProjectGraphMermaidRenderer(),
                new ClassDiagramMermaidRenderer(),
                new ErDiagramMermaidRenderer()),
            new FileSnapshotRepository(new SnapshotJsonSerializer()));
    }
}
