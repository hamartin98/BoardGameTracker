using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;
using Xunit;

namespace BoardGameTracker.TestData.Models;

public partial class ModelsTestData
{
    public static ScoringCategory ScytheStars
    {
        get =>
            new()
            {
                Id = CommonTestData.Guid1,
                CreatedAt = CommonTestData.DateTime,
                UpdatedAt = CommonTestData.DateTime,
                BoardGameId = CommonTestData.Guid1,
                Name = "Stars",
                Order = 0,
            };
    }

    public static ScoringCategory ScytheTerritories
    {
        get =>
            new()
            {
                Id = CommonTestData.Guid2,
                CreatedAt = CommonTestData.DateTime,
                UpdatedAt = CommonTestData.DateTime,
                BoardGameId = CommonTestData.Guid1,
                Name = "Territories",
                Order = 1,
            };
    }

    public static ScoringCategory FenrisBonus
    {
        get =>
            new()
            {
                Id = CommonTestData.Guid3,
                CreatedAt = CommonTestData.DateTime,
                UpdatedAt = CommonTestData.DateTime,
                BoardGameId = CommonTestData.Guid2,
                Name = "Fenris Bonus",
                Order = 0,
            };
    }

    public static ScoringCategory ScytheResourcesCreate
    {
        get => new() { BoardGameId = CommonTestData.Guid1, Name = "Resources" };
    }

    public static ScoringCategory FenrisStarsCreate
    {
        get => new() { BoardGameId = CommonTestData.Guid2, Name = "Stars" };
    }

    public static ScoringCategory ScytheStarsUppercaseCreate
    {
        get => new() { BoardGameId = CommonTestData.Guid1, Name = "STARS" };
    }

    public static ScoringCategory UnknownBoardGameCategoryCreate
    {
        get => new() { BoardGameId = CommonTestData.DeletedGuid, Name = "Stars" };
    }

    public static ScoringCategory StarTokensUpdate
    {
        get => new() { BoardGameId = CommonTestData.Guid2, Name = "Star Tokens" };
    }

    public static ScoringCategory TerritoriesUppercaseUpdate
    {
        get => new() { BoardGameId = CommonTestData.Guid1, Name = "TERRITORIES" };
    }

    public static ScoringCategoryFilter ScytheStarsFilter
    {
        get =>
            new()
            {
                BoardGameId = CommonTestData.Guid1,
                Name = "sta",
                Page = 2,
                PageSize = 5,
            };
    }

    public static void CheckScoringCategory(ScoringCategory expected, ScoringCategory actual)
    {
        Assert.Equal(expected.BoardGameId, actual.BoardGameId);
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.NormalizedName, actual.NormalizedName);
        Assert.Equal(expected.Order, actual.Order);
    }

    public static void CheckScoringCategoryFilter(
        ScoringCategoryFilter expected,
        ScoringCategoryFilter actual
    )
    {
        Assert.Equal(expected.BoardGameId, actual.BoardGameId);
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Page, actual.Page);
        Assert.Equal(expected.PageSize, actual.PageSize);
    }
}
