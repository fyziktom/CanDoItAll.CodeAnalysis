using CanDoItAll.CodeAnalytics.Rendering.Markdown;
using CanDoItAll.CodeAnalytics.Tests.Support;

namespace CanDoItAll.CodeAnalytics.Tests.Unit;

public sealed class SummaryWriterFacts {
    [Fact]
    public void SummaryWriter_renders_the_expected_markdown_summary() {
        var snapshot = SampleSnapshotFactory.Create();
        var writer = new MarkdownSummaryWriter();

        var content = writer.Write(snapshot);

        GoldenFileAssert.EqualToFile("exports/summary.md", content);
    }
}
