using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record SymbolImplementationItem(
    SymbolImplementationKind Kind,
    string ProjectName,
    string ModuleName,
    string NamespaceName,
    TypeFact Type);
