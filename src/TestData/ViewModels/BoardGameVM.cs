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

    public static BoardGameCreateVM AzulCreateVM
    {
        get =>
            new()
            {
                Name = "Azul",
                MinPlayerCount = 2,
                MaxPlayerCount = 4,
                MinPlayTime = new TimeSpan(0, 30, 0),
                MaxPlayTime = new TimeSpan(0, 45, 0),
                BggUrl = "https://boardgamegeek.com/boardgame/230802/azul",
                Categories = [BoardGameCategoryVM.ABSTRACT],
                Type = BoardGameTypeVM.BASE_GAME,
            };
    }

    public static BoardGameResponseVM AzulVM
    {
        get =>
            new()
            {
                MetaData = ResponseMetaDataVM1,
                Name = "Azul",
                MinPlayerCount = 2,
                MaxPlayerCount = 4,
                MinPlayTime = new TimeSpan(0, 30, 0),
                MaxPlayTime = new TimeSpan(0, 45, 0),
                BggUrl = "https://boardgamegeek.com/boardgame/230802/azul",
                Categories = [BoardGameCategoryVM.ABSTRACT],
                Type = BoardGameTypeVM.BASE_GAME,
            };
    }

    public static BoardGameCreateVM AzulInvalidPlayTimeCreateVM
    {
        get
        {
            var boardGame = AzulCreateVM;
            boardGame.MinPlayTime = new TimeSpan(0, 1, 0);
            return boardGame;
        }
    }

    public static BoardGameCreateVM AzulWithParentCreateVM
    {
        get
        {
            var boardGame = AzulCreateVM;
            boardGame.ExpansionOfId = CommonTestData.Guid1;
            return boardGame;
        }
    }

    public static BoardGameCreateVM ScytheUppercaseCreateVM
    {
        get
        {
            var boardGame = AzulCreateVM;
            boardGame.Name = "SCYTHE";
            return boardGame;
        }
    }

    public static BoardGameCreateVM ScytheInvadersCreateVM
    {
        get =>
            new()
            {
                Name = "Scythe: Invaders from Afar",
                MinPlayerCount = 1,
                MaxPlayerCount = 7,
                MinPlayTime = new TimeSpan(1, 55, 0),
                MaxPlayTime = new TimeSpan(2, 0, 0),
                Categories = [BoardGameCategoryVM.FIGHTING],
                Type = BoardGameTypeVM.EXPANSION,
                ExpansionOfId = CommonTestData.Guid1,
            };
    }

    public static BoardGameResponseVM ScytheInvadersVM
    {
        get =>
            new()
            {
                MetaData = ResponseMetaDataVM1,
                Name = "Scythe: Invaders from Afar",
                MinPlayerCount = 1,
                MaxPlayerCount = 7,
                MinPlayTime = new TimeSpan(1, 55, 0),
                MaxPlayTime = new TimeSpan(2, 0, 0),
                Categories = [BoardGameCategoryVM.FIGHTING],
                Type = BoardGameTypeVM.EXPANSION,
                ExpansionOfId = CommonTestData.Guid1,
            };
    }

    public static BoardGameCreateVM ExpansionOfFenrisCreateVM
    {
        get
        {
            var boardGame = ScytheInvadersCreateVM;
            boardGame.ExpansionOfId = CommonTestData.Guid2;
            return boardGame;
        }
    }

    public static BoardGameCreateVM ExpansionOfDeletedGameCreateVM
    {
        get
        {
            var boardGame = ScytheInvadersCreateVM;
            boardGame.ExpansionOfId = CommonTestData.DeletedGuid;
            return boardGame;
        }
    }

    public static BoardGameUpdateVM ScytheUpdateVM
    {
        get =>
            new()
            {
                Name = "Scythe",
                MinPlayerCount = 1,
                MaxPlayerCount = 7,
                MinPlayTime = new TimeSpan(1, 30, 0),
                MaxPlayTime = new TimeSpan(2, 0, 0),
                BggUrl = "https://boardgamegeek.com/boardgame/169786/scythe",
                Categories = [BoardGameCategoryVM.ECONOMIC],
            };
    }

    public static BoardGameResponseVM ScytheUpdatedVM
    {
        get =>
            new()
            {
                MetaData = ResponseMetaDataVM1,
                Name = "Scythe",
                MinPlayerCount = 1,
                MaxPlayerCount = 7,
                MinPlayTime = new TimeSpan(1, 30, 0),
                MaxPlayTime = new TimeSpan(2, 0, 0),
                BggUrl = "https://boardgamegeek.com/boardgame/169786/scythe",
                Categories = [BoardGameCategoryVM.ECONOMIC],
                Type = BoardGameTypeVM.BASE_GAME,
            };
    }

    public static BoardGameUpdateVM ScytheRenamedToFenrisUpdateVM
    {
        get
        {
            var boardGame = ScytheUpdateVM;
            boardGame.Name = "scythe: the rise of FENRIS";
            return boardGame;
        }
    }

    public static BoardGameUpdateVM ScytheWithParentUpdateVM
    {
        get
        {
            var boardGame = ScytheUpdateVM;
            boardGame.ExpansionOfId = CommonTestData.Guid2;
            return boardGame;
        }
    }

    public static BoardGameUpdateVM ScytheFenrisWithoutParentUpdateVM
    {
        get
        {
            var boardGame = ScytheFenrisUpdateVM;
            boardGame.ExpansionOfId = null;
            return boardGame;
        }
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
