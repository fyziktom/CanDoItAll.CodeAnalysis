using CanDoItAll.CodeAnalytics.Facts.Documentation;

namespace CanDoItAll.CodeAnalytics.Tests.Unit;

public sealed class XmlDocumentationFacts {
    [Fact]
    public void XmlDocumentation_extracts_the_summary_text() {
        var normalizer = new XmlDocumentationNormalizer();

        var result = normalizer.Normalize("<member><summary>  Coordinates  order   work. </summary></member>", "OrderService", null);

        Assert.Equal("Coordinates order work.", result.Summary);
        Assert.Empty(result.Diagnostics);
    }

    [Fact]
    public void XmlDocumentation_reports_malformed_xml() {
        var normalizer = new XmlDocumentationNormalizer();

        var result = normalizer.Normalize("<member><summary>broken", "OrderService", null);

        Assert.Null(result.Summary);
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Code == "XML0001");
    }
}
