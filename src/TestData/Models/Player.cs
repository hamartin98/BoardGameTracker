using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Enums;
using BoardGameTracker.Models.Filters;
using Xunit;

namespace BoardGameTracker.TestData.Models;

public partial class ModelsTestData
{
    public static Player Alice
    {
        get =>
            new()
            {
                Id = CommonTestData.Guid1,
                Name = "Alice",
                CreatedAt = CommonTestData.DateTime,
                UpdatedAt = CommonTestData.DateTime,
            };
    }

    public static Player Bob
    {
        get =>
            new()
            {
                Id = CommonTestData.Guid2,
                Name = "Bob",
                CreatedAt = CommonTestData.DateTime,
                UpdatedAt = CommonTestData.DateTime,
            };
    }

    public static Player Carol
    {
        get => new() { Name = "Carol" };
    }

    public static Player AliceRenamed
    {
        get => new() { Name = "Alice Renamed" };
    }

    public static Player AliceRenamedAgain
    {
        get => new() { Name = "Alice Renamed Again" };
    }

    public static PlayerFilter AliFilter
    {
        get =>
            new()
            {
                Name = "ali",
                Page = 2,
                PageSize = 20,
                OrderBy = "name",
                OrderDirection = OrderByDirection.DESC,
            };
    }

    public static PaginatedResponse<Player> PlayersPage2
    {
        get => new([Alice, Bob], count: 5, page: 2, pageSize: 2);
    }

    public static void CheckPlayer(Player expected, Player actual)
    {
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.NormalizedName, actual.NormalizedName);
    }

    public static void CheckPlayerFilter(PlayerFilter expected, PlayerFilter actual)
    {
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Page, actual.Page);
        Assert.Equal(expected.PageSize, actual.PageSize);
        Assert.Equal(expected.CreatedAtFrom, actual.CreatedAtFrom);
        Assert.Equal(expected.CreatedAtTo, actual.CreatedAtTo);
        Assert.Equal(expected.OrderBy, actual.OrderBy);
        Assert.Equal(expected.OrderDirection, actual.OrderDirection);
    }
}
