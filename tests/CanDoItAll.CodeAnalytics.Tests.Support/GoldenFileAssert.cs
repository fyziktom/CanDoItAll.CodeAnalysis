namespace CanDoItAll.CodeAnalytics.Tests.Support;

public static class GoldenFileAssert {
    public static void EqualToFile(string relativePath, string actual) {
        var goldenPath = FixturePaths.GetGoldenFilePath(relativePath);
        if (!File.Exists(goldenPath)) {
            throw new InvalidOperationException($"Missing golden file: {goldenPath}");
        }

        var expected = Normalize(File.ReadAllText(goldenPath));
        var normalizedActual = Normalize(actual);
        if (!string.Equals(expected, normalizedActual, StringComparison.Ordinal)) {
            throw new InvalidOperationException($"Golden file mismatch for {relativePath}.");
        }
    }

    private static string Normalize(string value) {
        return value.Replace("\r\n", "\n").Trim();
    }
}
