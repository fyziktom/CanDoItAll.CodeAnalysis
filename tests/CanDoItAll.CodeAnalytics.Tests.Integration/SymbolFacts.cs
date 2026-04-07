using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Facts.Documentation;
using CanDoItAll.CodeAnalytics.Facts.Symbols;
using CanDoItAll.CodeAnalytics.Tests.Support;
using CanDoItAll.CodeAnalytics.Workspace.Inventory;
using CanDoItAll.CodeAnalytics.Workspace.Loading;
using CanDoItAll.CodeAnalytics.Workspace.Normalization;

namespace CanDoItAll.CodeAnalytics.Tests.Integration;

public sealed class SymbolFacts {
    [Fact]
    public async Task Symbol_collector_reads_types_members_and_xml_summaries() {
        FixtureSolutionHost.EnsurePrepared();
        var loader = new MsBuildWorkspaceLoader(new AnalysisRequestNormalizer(), new ProjectFileInventoryReader());
        var collector = new SymbolFactsCollector(new XmlDocumentationNormalizer());

        using var workspace = await loader.LoadAsync(new AnalysisRequest(FixturePaths.GetFixtureSolutionPath(), [], [], true, true, true, true, true));
        var symbols = await collector.CollectAsync(workspace);

        Assert.Contains(symbols.Types, type => type.DisplayName.Contains("OrderService", StringComparison.Ordinal));
        Assert.Contains(symbols.Members, member => member.DisplayName.Contains("PlaceOrderAsync", StringComparison.Ordinal));
        Assert.Contains(symbols.Types, type => type.XmlSummary is not null && type.DisplayName.Contains("ShopDbContext", StringComparison.Ordinal));
    }
}
