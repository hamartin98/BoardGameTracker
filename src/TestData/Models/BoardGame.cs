using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Enums;

namespace BoardGameTracker.TestData.Models;

public partial class ModelsTestData
{
    public static readonly BoardGame Scythe = new()
    {
        Id = new Guid("77934a16-a70a-4ac3-9c0e-000000000001"),
        Name = "Scythe",
        MinPlayerCount = 1,
        MaxPlayerCount = 5,
        MinPlayTime = new TimeSpan(1, 55, 0),
        MaxPlayTime = new TimeSpan(2, 0, 0),
        BggUrl = "https://boardgamegeek.com/boardgame/169786/scythe",
        Categories =
            BoardGameCategory.EXPLORATION
            | BoardGameCategory.FIGHTING
            | BoardGameCategory.ECONOMIC
            | BoardGameCategory.SCIENCE_FICTION
            | BoardGameCategory.CIVILIZATION,
        Type = BoardGameType.BASE_GAME,
    };

    public static readonly BoardGame ScytheFenris = new()
    {
        Id = new Guid("77934a16-a70a-4ac3-9c0e-000000100001"),
        Name = "Scythe: The Rise of Fenris",
        MinPlayerCount = 1,
        MaxPlayerCount = 5,
        MinPlayTime = new TimeSpan(1, 55, 0),
        MaxPlayTime = new TimeSpan(2, 0, 0),
        BggUrl = "https://boardgamegeek.com/boardgameexpansion/242277/scythe-the-rise-of-fenris",
        Categories =
            BoardGameCategory.EXPLORATION
            | BoardGameCategory.FIGHTING
            | BoardGameCategory.ECONOMIC
            | BoardGameCategory.SCIENCE_FICTION
            | BoardGameCategory.CIVILIZATION,
        Type = BoardGameType.EXPANSION,
        ExpansionOfId = Scythe.Id,
    };

    public static readonly BoardGame DeletedBoardGame = new()
    {
        Id = new Guid("77934a16-a70a-4ac3-9c0e-000000000002"),
        IsDeleted = true,
        Name = "Scythe: The Rise of Fenris",
        MinPlayerCount = 1,
        MaxPlayerCount = 2,
        MinPlayTime = new TimeSpan(0, 30, 0),
        MaxPlayTime = new TimeSpan(1, 0, 0),
        Categories = BoardGameCategory.ANIMALS,
        Type = BoardGameType.BASE_GAME,
    };
}
