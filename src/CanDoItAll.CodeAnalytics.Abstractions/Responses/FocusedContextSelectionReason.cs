using CanDoItAll.CodeAnalytics.Abstractions;

namespace CanDoItAll.CodeAnalytics.Abstractions.Responses;

public sealed record FocusedContextSelectionReason(
    FocusedContextSelectionTargetKind TargetKind,
    string TargetId,
    FocusedContextSelectionReasonKind ReasonKind,
    FocusedContextReferenceRoleKind RoleKind);
