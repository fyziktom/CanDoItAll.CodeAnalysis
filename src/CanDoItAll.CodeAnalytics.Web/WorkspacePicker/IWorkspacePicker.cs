namespace CanDoItAll.CodeAnalytics.Web.WorkspacePicker;

public interface IWorkspacePicker {
    Task<WorkspacePickerResult> PickAsync(string? currentPath, CancellationToken cancellationToken = default);
}
