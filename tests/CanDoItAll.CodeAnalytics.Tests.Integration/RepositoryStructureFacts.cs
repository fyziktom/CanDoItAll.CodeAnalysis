using CanDoItAll.CodeAnalytics.Tests.Support;

namespace CanDoItAll.CodeAnalytics.Tests.Integration;

public sealed class RepositoryStructureFacts
{
    [Fact]
    public void Source_and_test_roots_exist()
    {
        var repoRoot = RepositoryRootLocator.FindRepositoryRoot();

        Assert.True(Directory.Exists(Path.Combine(repoRoot, "src")));
        Assert.True(Directory.Exists(Path.Combine(repoRoot, "tests")));
    }
}
