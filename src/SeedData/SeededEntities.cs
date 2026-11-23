using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Enums;

namespace BoardGameTracker.SeedData;

/// <summary>
/// Entities seeded into the in-memory database; each property returns a new instance
/// </summary>
public static class SeededEntities
{
    public static BoardGame Scythe
    {
        get =>
            new()
            {
                Id = SeedConstants.Guid1,
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
    }

    public static BoardGame ScytheFenris
    {
        get =>
            new()
            {
                Id = SeedConstants.Guid2,
                Name = "Scythe: The Rise of Fenris",
                MinPlayerCount = 1,
                MaxPlayerCount = 5,
                MinPlayTime = new TimeSpan(1, 55, 0),
                MaxPlayTime = new TimeSpan(2, 0, 0),
                BggUrl =
                    "https://boardgamegeek.com/boardgameexpansion/242277/scythe-the-rise-of-fenris",
                Categories =
                    BoardGameCategory.EXPLORATION
                    | BoardGameCategory.FIGHTING
                    | BoardGameCategory.ECONOMIC
                    | BoardGameCategory.SCIENCE_FICTION
                    | BoardGameCategory.CIVILIZATION,
                Type = BoardGameType.EXPANSION,
                ExpansionOfId = Scythe.Id,
            };
    }

    public static BoardGame DeletedBoardGame
    {
        get =>
            new()
            {
                Id = SeedConstants.DeletedGuid,
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

    public static Player Alice
    {
        get =>
            new()
            {
                Id = SeedConstants.Guid1,
                Name = "Alice",
                CreatedAt = SeedConstants.DateTime,
                UpdatedAt = SeedConstants.DateTime,
            };
    }

    public static Player Bob
    {
        get =>
            new()
            {
                Id = SeedConstants.Guid2,
                Name = "Bob",
                CreatedAt = SeedConstants.DateTime,
                UpdatedAt = SeedConstants.DateTime,
            };
    }

    public static PlaySession ScytheSession
    {
        get =>
            new()
            {
                Id = SeedConstants.Guid1,
                CreatedAt = SeedConstants.DateTime,
                UpdatedAt = SeedConstants.DateTime,
                BoardGameId = SeedConstants.Guid1,
                UsedExpansions = { ScytheFenris },
                Players = { Alice, Bob },
                StartPlayerId = SeedConstants.Guid1,
                StartTime = SeedConstants.DateTime,
                Duration = new TimeSpan(2, 0, 0),
                SessionType = PlaySessionType.COMPETITIVE,
                Notes = "First game",
            };
    }

    public static ScoringCategory ScytheStars
    {
        get =>
            new()
            {
                Id = SeedConstants.Guid1,
                CreatedAt = SeedConstants.DateTime,
                UpdatedAt = SeedConstants.DateTime,
                BoardGameId = SeedConstants.Guid1,
                Name = "Stars",
                Order = 0,
            };
    }

    public static ScoringCategory ScytheTerritories
    {
        get =>
            new()
            {
                Id = SeedConstants.Guid2,
                CreatedAt = SeedConstants.DateTime,
                UpdatedAt = SeedConstants.DateTime,
                BoardGameId = SeedConstants.Guid1,
                Name = "Territories",
                Order = 1,
            };
    }

    public static ScoringCategory FenrisBonus
    {
        get =>
            new()
            {
                Id = SeedConstants.Guid3,
                CreatedAt = SeedConstants.DateTime,
                UpdatedAt = SeedConstants.DateTime,
                BoardGameId = SeedConstants.Guid2,
                Name = "Fenris Bonus",
                Order = 0,
            };
    }

    public static PlayerScore AliceScytheScore
    {
        get =>
            new()
            {
                Id = SeedConstants.Guid1,
                CreatedAt = SeedConstants.DateTime,
                UpdatedAt = SeedConstants.DateTime,
                PlayerId = SeedConstants.Guid1,
                SessionId = SeedConstants.Guid1,
                Score = 78,
                Win = true,
                Notes = "Polania",
                CategoryScores =
                {
                    new() { ScoringCategoryId = SeedConstants.Guid1, Value = 30 },
                    new() { ScoringCategoryId = SeedConstants.Guid2, Value = 44 },
                    new() { ScoringCategoryId = SeedConstants.Guid3, Value = 4 },
                },
            };
    }

    public static PlayerScore BobScytheScore
    {
        get =>
            new()
            {
                Id = SeedConstants.Guid2,
                CreatedAt = SeedConstants.DateTime,
                UpdatedAt = SeedConstants.DateTime,
                PlayerId = SeedConstants.Guid2,
                SessionId = SeedConstants.Guid1,
                Score = 64,
                Win = false,
                Notes = "Saxony",
                CategoryScores =
                {
                    new() { ScoringCategoryId = SeedConstants.Guid1, Value = 24 },
                    new() { ScoringCategoryId = SeedConstants.Guid2, Value = 36 },
                    new() { ScoringCategoryId = SeedConstants.Guid3, Value = 4 },
                },
            };
    }
}
