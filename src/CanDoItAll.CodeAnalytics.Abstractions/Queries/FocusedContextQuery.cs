using CanDoItAll.CodeAnalytics.Abstractions;

namespace CanDoItAll.CodeAnalytics.Abstractions.Queries;

public sealed record FocusedContextQuery(
    string SnapshotId,
    string? TypeId = null,
    string? MemberId = null,
    string? ServiceRegistrationId = null,
    int Depth = 2,
    string? QueryText = null,
    IReadOnlyList<string>? FocusTags = null,
    FocusedContextIntent Intent = FocusedContextIntent.Auto,
    FocusedContextPrecision Precision = FocusedContextPrecision.Auto,
    IReadOnlyList<string>? RelationHints = null);
