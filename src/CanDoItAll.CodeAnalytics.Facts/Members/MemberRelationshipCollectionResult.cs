using CanDoItAll.CodeAnalytics.Domain.Diagnostics;
using CanDoItAll.CodeAnalytics.Domain.Facts;

namespace CanDoItAll.CodeAnalytics.Facts.Members;

public sealed record MemberRelationshipCollectionResult(
    IReadOnlyList<MemberRelationshipFact> Relationships,
    IReadOnlyList<AnalysisDiagnostic> Diagnostics);
