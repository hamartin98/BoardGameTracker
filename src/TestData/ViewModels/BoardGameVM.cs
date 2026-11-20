using BoardGameTracker.ViewModels;
using BoardGameTracker.ViewModels.Enums;
using Xunit;

namespace BoardGameTracker.TestData.ViewModels;

public partial class ViewModelsTestData
{
    public static BoardGameResponseVM ScytheVM
    {
        get =>
            new()
            {
                MetaData = ResponseMetaDataVM1,
                Name = "Scythe",
                MinPlayerCount = 1,
                MaxPlayerCount = 5,
                MinPlayTime = new TimeSpan(1, 55, 0),
                MaxPlayTime = new TimeSpan(2, 0, 0),
                BggUrl = "https://boardgamegeek.com/boardgame/169786/scythe",
                Categories =
                [
                    BoardGameCategoryVM.EXPLORATION,
                    BoardGameCategoryVM.FIGHTING,
                    BoardGameCategoryVM.ECONOMIC,
                    BoardGameCategoryVM.SCIENCE_FICTION,
                    BoardGameCategoryVM.CIVILIZATION,
                ],
                Type = BoardGameTypeVM.BASE_GAME,
            };
    }

    public static BoardGameResponseVM ScytheFenrisVM
    {
        get =>
            new()
            {
                MetaData = ResponseMetaDataVM2,
                Name = "Scythe: The Rise of Fenris",
                MinPlayerCount = 1,
                MaxPlayerCount = 5,
                MinPlayTime = new TimeSpan(1, 55, 0),
                MaxPlayTime = new TimeSpan(2, 0, 0),
                BggUrl =
                    "https://boardgamegeek.com/boardgameexpansion/242277/scythe-the-rise-of-fenris",
                Categories =
                [
                    BoardGameCategoryVM.EXPLORATION,
                    BoardGameCategoryVM.FIGHTING,
                    BoardGameCategoryVM.ECONOMIC,
                    BoardGameCategoryVM.SCIENCE_FICTION,
                    BoardGameCategoryVM.CIVILIZATION,
                ],
                Type = BoardGameTypeVM.EXPANSION,
                ExpansionOfId = ScytheVM.MetaData.Id,
            };
    }

    public static BoardGameUpdateVM ScytheFenrisUpdateVM
    {
        get =>
            new()
            {
                Name = "Scythe: The Rise of Fenris",
                MinPlayerCount = 1,
                MaxPlayerCount = 5,
                MinPlayTime = new TimeSpan(1, 55, 0),
                MaxPlayTime = new TimeSpan(2, 0, 0),
                BggUrl =
                    "https://boardgamegeek.com/boardgameexpansion/242277/scythe-the-rise-of-fenris",
                Categories =
                [
                    BoardGameCategoryVM.EXPLORATION,
                    BoardGameCategoryVM.FIGHTING,
                    BoardGameCategoryVM.ECONOMIC,
                    BoardGameCategoryVM.SCIENCE_FICTION,
                    BoardGameCategoryVM.CIVILIZATION,
                ],
                ExpansionOfId = CommonTestData.Guid1,
            };
    }

    public static void CheckBoardGameResponseVM(
        BoardGameResponseVM expected,
        BoardGameResponseVM actual
    )
    {
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.MinPlayerCount, actual.MinPlayerCount);
        Assert.Equal(expected.MaxPlayerCount, actual.MaxPlayerCount);
        Assert.Equal(expected.MinPlayTime, actual.MinPlayTime);
        Assert.Equal(expected.MaxPlayTime, actual.MaxPlayTime);
        Assert.Equal(expected.BggUrl, actual.BggUrl);
        Assert.Equal(expected.Categories.Count, actual.Categories.Count);
        foreach (var category in expected.Categories)
        {
            Assert.Contains(category, actual.Categories);
        }
        Assert.Equal(expected.Type, actual.Type);
        CommonTestData.CheckNullableProperty(expected.ExpansionOfId, actual.ExpansionOfId);
    }
}
