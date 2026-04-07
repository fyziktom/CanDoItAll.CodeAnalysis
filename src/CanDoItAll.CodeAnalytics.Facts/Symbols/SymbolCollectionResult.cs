using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Facts.Symbols;

public sealed record SymbolCollectionResult(
    IReadOnlyList<NamespaceFact> Namespaces,
    IReadOnlyList<TypeFact> Types,
    IReadOnlyList<MemberFact> Members,
    IReadOnlyList<AnalysisDiagnostic> Diagnostics);
