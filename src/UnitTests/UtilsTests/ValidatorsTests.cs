using BoardGameTracker.Utils;

namespace BoardGameTracker.UnitTests.UtilsTests;

public class ValidatorsTests
{
    [Theory]
    [InlineData("https://a.com", true)]
    [InlineData("https://boardgamegeek.com", true)]
    [InlineData("https://boardgamegeek.com/boardgame/169786/scythe", true)]
    [InlineData("  https://boardgamegeek.com", false)]
    [InlineData("https://boardgamegeek.com  ", false)]
    [InlineData("", false)]
    [InlineData("http", false)]
    [InlineData("https://a", false)]
    [InlineData("bgg.com", false)]
    public void IsUrlValidTest(string url, bool expected)
    {
        Assert.Equal(expected, Validators.IsUrlValid(url));
    }
}
