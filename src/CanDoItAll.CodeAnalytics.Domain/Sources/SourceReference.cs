using System.Text.Json.Serialization;

namespace CanDoItAll.CodeAnalytics.Domain.Sources;

public sealed record SourceReference(
    string Path,
    int? Line = null,
    int? Column = null,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] int? EndLine = null,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] int? EndColumn = null);
