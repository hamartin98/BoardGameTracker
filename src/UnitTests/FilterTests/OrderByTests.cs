using BoardGameTracker.Models;
using BoardGameTracker.Models.Enums;
using BoardGameTracker.Models.Filters;
using BoardGameTracker.TestData.Models;

namespace BoardGameTracker.UnitTests.FilterTests;

public class OrderByTests
{
    private static IQueryable<BoardGameTracker.Models.Entities.Player> Players =>
        new[] { ModelsTestData.Alice, ModelsTestData.Bob }.AsQueryable();

    [Theory]
    [InlineData("name")]
    [InlineData("Name")]
    [InlineData("NAME")]
    public void OrderByIsCaseInsensitiveTest(string property)
    {
        var actual = Players.OrderBy(property, OrderByDirection.DESC).Select(p => p.Name).ToList();

        Assert.Equal([ModelsTestData.Bob.Name, ModelsTestData.Alice.Name], actual);
    }

    [Theory]
    [InlineData("nope")]
    [InlineData("name.")]
    [InlineData(".name")]
    [InlineData("name.nope")]
    public void OrderByUnknownPropertyThrowsTest(string property)
    {
        Assert.Throws<InvalidOrderByException>(() => Players.OrderBy(property).ToList());
    }

    [Fact]
    public void InvalidOrderByIsClientErrorTest()
    {
        Assert.IsAssignableFrom<ApplicationException>(new InvalidOrderByException("nope"));
    }
}
