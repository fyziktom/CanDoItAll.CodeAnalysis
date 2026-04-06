using CanDoItAll.CodeAnalytics.Tests.Support;

namespace CanDoItAll.CodeAnalytics.Tests.Unit;

public sealed class RepositoryBootstrapFacts
{
    [Fact]
    public void DirectoryBuildProps_exists_at_the_repository_root()
    {
        var repoRoot = RepositoryRootLocator.FindRepositoryRoot();
        var path = Path.Combine(repoRoot, "Directory.Build.props");

        Assert.True(File.Exists(path));
    }
}
