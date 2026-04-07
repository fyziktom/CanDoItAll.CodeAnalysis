namespace CanDoItAll.CodeAnalytics.Web.WorkspacePicker;

public sealed record WorkspacePickerResult(
    bool IsSuccess,
    bool IsCanceled,
    string? WorkspacePath,
    string? ErrorMessage);
