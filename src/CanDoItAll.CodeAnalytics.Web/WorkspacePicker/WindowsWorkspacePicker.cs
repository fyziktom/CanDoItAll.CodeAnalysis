using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging;

namespace CanDoItAll.CodeAnalytics.Web.WorkspacePicker;

public sealed class WindowsWorkspacePicker : IWorkspacePicker {
    private const string PickerScript = """
        Add-Type -AssemblyName System.Windows.Forms
        [Console]::OutputEncoding = [System.Text.Encoding]::UTF8
        $dialog = New-Object System.Windows.Forms.OpenFileDialog
        $dialog.Filter = 'Solutions and projects (*.sln;*.slnx;*.csproj)|*.sln;*.slnx;*.csproj|Solutions (*.sln;*.slnx)|*.sln;*.slnx|C# projects (*.csproj)|*.csproj|All files (*.*)|*.*'
        $dialog.Multiselect = $false
        $dialog.CheckFileExists = $true
        $dialog.Title = 'Select solution or project'
        if ($env:CODE_ANALYTICS_PICKER_INITIAL_PATH) {
            if (Test-Path -LiteralPath $env:CODE_ANALYTICS_PICKER_INITIAL_PATH) {
                if ((Get-Item -LiteralPath $env:CODE_ANALYTICS_PICKER_INITIAL_PATH) -is [System.IO.DirectoryInfo]) {
                    $dialog.InitialDirectory = $env:CODE_ANALYTICS_PICKER_INITIAL_PATH
                }
                else {
                    $dialog.InitialDirectory = Split-Path -LiteralPath $env:CODE_ANALYTICS_PICKER_INITIAL_PATH -Parent
                    $dialog.FileName = Split-Path -LiteralPath $env:CODE_ANALYTICS_PICKER_INITIAL_PATH -Leaf
                }
            }
        }
        if ($dialog.ShowDialog() -eq [System.Windows.Forms.DialogResult]::OK) {
            Write-Output $dialog.FileName
        }
        """;
    private readonly ILogger<WindowsWorkspacePicker> _logger;

    public WindowsWorkspacePicker(ILogger<WindowsWorkspacePicker> logger) {
        _logger = logger;
    }

    public async Task<WorkspacePickerResult> PickAsync(string? currentPath, CancellationToken cancellationToken = default) {
        if (!OperatingSystem.IsWindows()) {
            return new WorkspacePickerResult(false, false, null, "The local workspace picker is only available on Windows.");
        }

        using var process = CreateProcess(currentPath);

        try {
            process.Start();
            var stdoutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
            var stderrTask = process.StandardError.ReadToEndAsync(cancellationToken);

            await process.WaitForExitAsync(cancellationToken);

            var stdout = (await stdoutTask).Trim();
            var stderr = (await stderrTask).Trim();
            if (process.ExitCode != 0) {
                var message = string.IsNullOrWhiteSpace(stderr)
                    ? $"Workspace picker failed with exit code {process.ExitCode}."
                    : stderr;
                _logger.LogError("Workspace picker failed: {Message}", message);
                return new WorkspacePickerResult(false, false, null, message);
            }

            if (string.IsNullOrWhiteSpace(stdout)) {
                return new WorkspacePickerResult(false, true, null, null);
            }

            _logger.LogInformation("Workspace picker selected {WorkspacePath}", stdout);
            return new WorkspacePickerResult(true, false, stdout, null);
        }
        catch (Exception exception) {
            _logger.LogError(exception, "Workspace picker failed to open.");
            return new WorkspacePickerResult(false, false, null, exception.Message);
        }
    }

    private static Process CreateProcess(string? currentPath) {
        var startInfo = new ProcessStartInfo {
            FileName = "pwsh",
            Arguments = $"-NoProfile -STA -EncodedCommand {BuildEncodedCommand()}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        if (!string.IsNullOrWhiteSpace(currentPath)) {
            startInfo.Environment["CODE_ANALYTICS_PICKER_INITIAL_PATH"] = currentPath;
        }

        return new Process {
            StartInfo = startInfo,
        };
    }

    private static string BuildEncodedCommand() {
        return Convert.ToBase64String(Encoding.Unicode.GetBytes(PickerScript));
    }
}
