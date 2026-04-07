namespace CanDoItAll.CodeAnalytics.Tests.Support;

public sealed class TemporaryDirectoryScope : IDisposable {
    public TemporaryDirectoryScope() {
        Path = System.IO.Path.Combine(
            System.IO.Path.GetTempPath(),
            "CanDoItAll.CodeAnalytics.Tests",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path);
    }

    public string Path { get; }

    public void Dispose() {
        if (Directory.Exists(Path)) {
            Directory.Delete(Path, recursive: true);
        }
    }
}
