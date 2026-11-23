using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;
using BoardGameTracker.SeedData;
using Xunit;

namespace BoardGameTracker.TestData.Models;

public partial class ModelsTestData
{
    public static ScoringCategory ScytheStars
    {
        get => SeededEntities.ScytheStars;
    }

    public static ScoringCategory ScytheTerritories
    {
        get => SeededEntities.ScytheTerritories;
    }

    public static ScoringCategory FenrisBonus
    {
        get => SeededEntities.FenrisBonus;
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
