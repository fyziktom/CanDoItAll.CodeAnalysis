using CanDoItAll.CodeAnalytics.Domain.Sources;

namespace CanDoItAll.CodeAnalytics.Domain.Facts;

public sealed record DependencyEdgeFact(
    string EdgeId,
    DependencyKind Kind,
    string FromId,
    string ToId,
    int Weight,
    SourceReference? Source = null);
