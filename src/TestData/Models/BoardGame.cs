using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Enums;
using BoardGameTracker.SeedData;
using Xunit;

namespace BoardGameTracker.TestData.Models;

public partial class ModelsTestData
{
    public static BoardGame Scythe
    {
        get => SeededEntities.Scythe;
    }

    public static BoardGame ScytheFenris
    {
        get => SeededEntities.ScytheFenris;
    }

    public static BoardGame DeletedBoardGame
    {
        get => SeededEntities.DeletedBoardGame;
    }

    public static BoardGame ScytheUppercase
    {
        get =>
            new()
            {
                Name = "SCYTHE",
                MinPlayerCount = 1,
                MaxPlayerCount = 5,
                MinPlayTime = new TimeSpan(1, 55, 0),
                MaxPlayTime = new TimeSpan(2, 0, 0),
                Type = BoardGameType.BASE_GAME,
            };
    }

    public static BoardGame DeletedBoardGameRecreated
    {
        get =>
            new()
            {
                Name = DeletedBoardGame.Name,
                MinPlayerCount = 1,
                MaxPlayerCount = 2,
                MinPlayTime = new TimeSpan(0, 30, 0),
                MaxPlayTime = new TimeSpan(1, 0, 0),
                Type = BoardGameType.BASE_GAME,
            };
    }

    public static BoardGame ExpansionOfExpansion
    {
        get =>
            new()
            {
                Name = "Expansion of an expansion",
                MinPlayerCount = 1,
                MaxPlayerCount = 5,
                MinPlayTime = new TimeSpan(1, 0, 0),
                MaxPlayTime = new TimeSpan(2, 0, 0),
                Type = BoardGameType.EXPANSION,
                ExpansionOfId = ScytheFenris.Id,
            };
    }

    public static BoardGame ScytheWithParent
    {
        get
        {
            var boardGame = Scythe;
            boardGame.ExpansionOfId = CommonTestData.Guid2;
            return boardGame;
        }
    }

    public static BoardGame ScytheFenrisUpdate
    {
        get
        {
            var boardGame = ScytheFenris;
            boardGame.Id = default;
            boardGame.Type = default;
            return boardGame;
        }
    }

    public static BoardGame ScytheFenrisUpdateWithoutParent
    {
        get
        {
            var boardGame = ScytheFenrisUpdate;
            boardGame.ExpansionOfId = null;
            return boardGame;
        }
    }

    public static BoardGame ExpansionWithoutParent
    {
        get
        {
            var boardGame = ExpansionOfExpansion;
            boardGame.ExpansionOfId = null;
            return boardGame;
        }
    }

    public static void CheckBoardGame(BoardGame expected, BoardGame actual)
    {
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.MinPlayerCount, actual.MinPlayerCount);
        Assert.Equal(expected.MaxPlayerCount, actual.MaxPlayerCount);
        Assert.Equal(expected.MinPlayTime, actual.MinPlayTime);
        Assert.Equal(expected.MaxPlayTime, actual.MaxPlayTime);
        Assert.Equal(expected.BggId, actual.BggId);
        Assert.Equal(expected.Categories, actual.Categories);
        Assert.Equal(expected.Type, actual.Type);
        CommonTestData.CheckNullableProperty(expected.ExpansionOfId, actual.ExpansionOfId);
    }
}
