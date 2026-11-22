using BoardGameTracker.ViewModels;
using BoardGameTracker.ViewModels.Filters;
using Xunit;

namespace BoardGameTracker.TestData.ViewModels;

public partial class ViewModelsTestData
{
    public static ScoringCategoryResponseVM ScytheStarsVM
    {
        get =>
            new()
            {
                MetaData = ResponseMetaDataVM1,
                BoardGameId = CommonTestData.Guid1,
                Name = "Stars",
                Order = 0,
            };
    }

    public static ScoringCategoryResponseVM ScytheTerritoriesVM
    {
        get =>
            new()
            {
                MetaData = ResponseMetaDataVM2,
                BoardGameId = CommonTestData.Guid1,
                Name = "Territories",
                Order = 1,
            };
    }

    public static ScoringCategoryResponseVM FenrisBonusVM
    {
        get =>
            new()
            {
                MetaData = ResponseMetaDataVM3,
                BoardGameId = CommonTestData.Guid2,
                Name = "Fenris Bonus",
                Order = 0,
            };
    }

    public static ScoringCategoryCreateVM ScytheResourcesCreateVM
    {
        get => new() { BoardGameId = CommonTestData.Guid1, Name = "Resources" };
    }

    public static ScoringCategoryCreateVM ScytheStarsUppercaseCreateVM
    {
        get => new() { BoardGameId = CommonTestData.Guid1, Name = "STARS" };
    }

    public static ScoringCategoryCreateVM DeletedBoardGameCategoryCreateVM
    {
        get => new() { BoardGameId = CommonTestData.DeletedGuid, Name = "Stars" };
    }

    public static ScoringCategoryCreateVM EmptyNameCategoryCreateVM
    {
        get => new() { BoardGameId = CommonTestData.Guid1, Name = "" };
    }

    public static ScoringCategoryCreateVM AzulTilesCategoryCreateVM
    {
        get => new() { BoardGameId = Guid.Empty, Name = "Tiles" };
    }

    public static ScoringCategoryUpdateVM StarTokensUpdateVM
    {
        get => new() { Name = "Star Tokens" };
    }

    public static ScoringCategoryUpdateVM TerritoriesUppercaseUpdateVM
    {
        get => new() { Name = "TERRITORIES" };
    }

    public static ScoringCategoryResponseVM ScytheStarTokensVM
    {
        get
        {
            var category = ScytheStarsVM;
            category.MetaData.Version = 2;
            category.Name = "Star Tokens";
            return category;
        }
    }

    public static ScoringCategoryFilterVM ScytheStarsFilterVM
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

    public static void CheckScoringCategoryVM(
        ScoringCategoryCreateVM expected,
        ScoringCategoryCreateVM actual
    )
    {
        Assert.Equal(expected.BoardGameId, actual.BoardGameId);
        Assert.Equal(expected.Name, actual.Name);
    }

    public static void CheckScoringCategoryResponseVM(
        ScoringCategoryResponseVM expected,
        ScoringCategoryResponseVM actual
    )
    {
        Assert.Equal(expected.MetaData.Id, actual.MetaData.Id);
        Assert.Equal(expected.MetaData.Version, actual.MetaData.Version);
        Assert.Equal(expected.Order, actual.Order);
        CheckScoringCategoryVM(expected, actual);
    }

    public static void CheckScoringCategoryResponseVMs(
        List<ScoringCategoryResponseVM> expected,
        List<ScoringCategoryResponseVM> actual
    )
    {
        Assert.Equal(expected.Count, actual.Count);
        for (int i = 0; i < expected.Count; i++)
        {
            CheckScoringCategoryResponseVM(expected[i], actual[i]);
        }
    }
}
