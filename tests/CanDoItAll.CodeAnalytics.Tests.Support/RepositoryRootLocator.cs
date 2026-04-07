namespace CanDoItAll.CodeAnalytics.Tests.Support;

public static class RepositoryRootLocator {
    public static string FindRepositoryRoot() {
        DirectoryInfo? current = new(AppContext.BaseDirectory);

        while (current is not null) {
            var solutionPath = Path.Combine(current.FullName, "CanDoItAll.CodeAnalsis.slnx");
            if (File.Exists(solutionPath)) {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("Could not locate the repository root from the current test execution directory.");
    }
}
