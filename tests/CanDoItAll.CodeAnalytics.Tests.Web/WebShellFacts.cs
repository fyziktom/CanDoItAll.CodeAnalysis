using CanDoItAll.CodeAnalytics.Tests.Support;

namespace CanDoItAll.CodeAnalytics.Tests.Web;

public sealed class WebShellFacts
{
    [Fact]
    public void Home_page_exists_in_the_web_project()
    {
        var repoRoot = RepositoryRootLocator.FindRepositoryRoot();
        var homePagePath = Path.Combine(repoRoot, "src", "CanDoItAll.CodeAnalytics.Web", "Components", "Pages", "Home.razor");

        Assert.True(File.Exists(homePagePath));
    }
}
