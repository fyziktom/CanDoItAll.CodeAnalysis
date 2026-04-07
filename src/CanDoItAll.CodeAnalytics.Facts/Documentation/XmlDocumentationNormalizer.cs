using System.Xml.Linq;
using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Sources;

namespace CanDoItAll.CodeAnalytics.Facts.Documentation;

public sealed class XmlDocumentationNormalizer {
    public XmlDocumentationResult Normalize(string? xml, string symbolDisplayName, SourceReference? source) {
        if (string.IsNullOrWhiteSpace(xml)) {
            return new XmlDocumentationResult(null, []);
        }

        try {
            var document = XDocument.Parse(xml, LoadOptions.None);
            var summary = document.Descendants("summary")
                .Select(node => NormalizeWhitespace(node.Value))
                .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));

            return new XmlDocumentationResult(summary, []);
        }
        catch (Exception exception) {
            var diagnostics = new[]
            {
                new AnalysisDiagnostic(
                    "XML0001",
                    AnalysisDiagnosticSeverity.Warning,
                    $"XML documentation could not be parsed for {symbolDisplayName}: {exception.Message}",
                    source),
            };

            return new XmlDocumentationResult(null, diagnostics);
        }
    }

    private static string NormalizeWhitespace(string value) {
        return string.Join(
            " ",
            value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
    }
}
