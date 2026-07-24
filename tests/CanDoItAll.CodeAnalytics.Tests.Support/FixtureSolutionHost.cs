using System.Diagnostics;

namespace CanDoItAll.CodeAnalytics.Tests.Support;

public static class FixtureSolutionHost {
    private static readonly Lock SyncLock = new();
    private static bool _isPrepared;

    public static void EnsurePrepared() {
        lock (SyncLock) {
            if (_isPrepared) {
                return;
            }

            var solutionPath = FixturePaths.GetFixtureSolutionPath();
            var startInfo = new ProcessStartInfo {
                FileName = "dotnet",
                Arguments = $"restore \"{solutionPath}\"",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using var process = Process.Start(startInfo)
                ?? throw new InvalidOperationException("Could not start dotnet restore for the fixture solution.");
            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();
            process.WaitForExit();
            Task.WaitAll(outputTask, errorTask);

            if (process.ExitCode != 0) {
                throw new InvalidOperationException(
                    $"Fixture restore failed.{Environment.NewLine}{outputTask.Result}{Environment.NewLine}{errorTask.Result}");
            }

            _isPrepared = true;
        }
    }
}
