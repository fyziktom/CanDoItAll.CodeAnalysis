namespace CanDoItAll.CodeAnalytics.Tests.Support;

public static class FixturePaths {
    public static string GetFixtureSolutionPath() {
        return Path.Combine(
            RepositoryRootLocator.FindRepositoryRoot(),
            "tests",
            "fixtures",
            "Fixture.Shop",
            "Fixture.Shop.slnx");
    }

    public static string GetGoldenFilePath(string relativePath) {
        return Path.Combine(
            RepositoryRootLocator.FindRepositoryRoot(),
            "tests",
            "CanDoItAll.CodeAnalytics.Tests.Support",
            "Golden",
            relativePath.Replace('/', Path.DirectorySeparatorChar));
    }
}
