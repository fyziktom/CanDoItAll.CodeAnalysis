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
            process.WaitForExit();

            if (process.ExitCode != 0) {
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                throw new InvalidOperationException($"Fixture restore failed.{Environment.NewLine}{output}{Environment.NewLine}{error}");
            }

            _isPrepared = true;
        }
    }
}
