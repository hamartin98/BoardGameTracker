using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;
using Xunit;

namespace BoardGameTracker.TestData.Models;

public partial class ModelsTestData
{
    public static PlayerScore AliceScytheScore
    {
        get =>
            new()
            {
                Id = CommonTestData.Guid1,
                CreatedAt = CommonTestData.DateTime,
                UpdatedAt = CommonTestData.DateTime,
                PlayerId = CommonTestData.Guid1,
                SessionId = CommonTestData.Guid1,
                Score = 78,
                Win = true,
                Notes = "Polania",
                CategoryScores =
                {
                    new() { ScoringCategoryId = CommonTestData.Guid1, Value = 30 },
                    new() { ScoringCategoryId = CommonTestData.Guid2, Value = 44 },
                    new() { ScoringCategoryId = CommonTestData.Guid3, Value = 4 },
                },
            };
    }

    public static PlayerScore BobScytheScore
    {
        get =>
            new()
            {
                Id = CommonTestData.Guid2,
                CreatedAt = CommonTestData.DateTime,
                UpdatedAt = CommonTestData.DateTime,
                PlayerId = CommonTestData.Guid2,
                SessionId = CommonTestData.Guid1,
                Score = 64,
                Win = false,
                Notes = "Saxony",
                CategoryScores =
                {
                    new() { ScoringCategoryId = CommonTestData.Guid1, Value = 24 },
                    new() { ScoringCategoryId = CommonTestData.Guid2, Value = 36 },
                    new() { ScoringCategoryId = CommonTestData.Guid3, Value = 4 },
                },
            };
    }

    public static PlayerScore AliceScytheScoreCreate
    {
        get =>
            new()
            {
                PlayerId = CommonTestData.Guid1,
                SessionId = CommonTestData.Guid1,
                Win = true,
                Notes = "Polania",
                CategoryScores =
                {
                    new() { ScoringCategoryId = CommonTestData.Guid1, Value = 30 },
                    new() { ScoringCategoryId = CommonTestData.Guid2, Value = 44 },
                    new() { ScoringCategoryId = CommonTestData.Guid3, Value = 4 },
                },
            };
    }

    public static PlayerScore AliceScytheScoreUpdate
    {
        get =>
            new()
            {
                Win = false,
                Notes = "Polania, recounted",
                CategoryScores =
                {
                    new() { ScoringCategoryId = CommonTestData.Guid1, Value = 31 },
                    new() { ScoringCategoryId = CommonTestData.Guid2, Value = 46 },
                    new() { ScoringCategoryId = CommonTestData.Guid3, Value = 4 },
                },
            };
    }

    public static PlayerScore AliceScytheScoreUpdated
    {
        get =>
            new()
            {
                PlayerId = CommonTestData.Guid1,
                SessionId = CommonTestData.Guid1,
                Score = 81,
                Win = false,
                Notes = "Polania, recounted",
                CategoryScores =
                {
                    new() { ScoringCategoryId = CommonTestData.Guid1, Value = 31 },
                    new() { ScoringCategoryId = CommonTestData.Guid2, Value = 46 },
                    new() { ScoringCategoryId = CommonTestData.Guid3, Value = 4 },
                },
            };
    }

    public static PlayerScore AliceScytheScoreMissingCategoryCreate
    {
        get
        {
            var score = AliceScytheScoreCreate;
            score.CategoryScores.Remove(score.CategoryScores.Last());
            return score;
        }
    }

    public static PlayerScore AliceScytheScoreUnknownCategoryCreate
    {
        get
        {
            var score = AliceScytheScoreCreate;
            score.CategoryScores.Add(new() { ScoringCategoryId = CommonTestData.DeletedGuid });
            return score;
        }
    }

    public static PlayerScore AliceScoreWithoutCategoriesCreate
    {
        get
        {
            var score = AliceScytheScoreCreate;
            score.CategoryScores.Clear();
            return score;
        }
    }

    public static PlayerScore AliceScoreWithoutCategoriesUpdate
    {
        get
        {
            var score = AliceScytheScoreUpdate;
            score.CategoryScores.Clear();
            return score;
        }
    }

    public static PlayerScore AliceLegacyScytheScore
    {
        get
        {
            var score = AliceScytheScore;
            score.CategoryScores.Clear();
            return score;
        }
    }

    public static PlayerScore UnknownPlayerScoreCreate
    {
        get
        {
            var score = AliceScytheScoreCreate;
            score.PlayerId = CommonTestData.DeletedGuid;
            return score;
        }
    }

    public static PlayerScore UnknownSessionScoreCreate
    {
        get
        {
            var score = AliceScytheScoreCreate;
            score.SessionId = CommonTestData.DeletedGuid;
            return score;
        }
    }

    public static PlayerScoreFilter AliceWinsFilter
    {
        get =>
            new()
            {
                PlayerId = CommonTestData.Guid1,
                SessionId = CommonTestData.Guid1,
                Win = true,
                Page = 2,
                PageSize = 5,
            };
    }

    public static void CheckPlayerScore(PlayerScore expected, PlayerScore actual)
    {
        Assert.Equal(expected.PlayerId, actual.PlayerId);
        Assert.Equal(expected.SessionId, actual.SessionId);
        Assert.Equal(expected.Score, actual.Score);
        Assert.Equal(expected.Win, actual.Win);
        Assert.Equal(expected.Notes, actual.Notes);
        Assert.Equal(
            expected
                .CategoryScores.OrderBy(e => e.ScoringCategoryId)
                .Select(e => (e.ScoringCategoryId, e.Value)),
            actual
                .CategoryScores.OrderBy(e => e.ScoringCategoryId)
                .Select(e => (e.ScoringCategoryId, e.Value))
        );
    }

    public static void CheckPlayerScoreFilter(PlayerScoreFilter expected, PlayerScoreFilter actual)
    {
        Assert.Equal(expected.PlayerId, actual.PlayerId);
        Assert.Equal(expected.SessionId, actual.SessionId);
        Assert.Equal(expected.Win, actual.Win);
        Assert.Equal(expected.Page, actual.Page);
        Assert.Equal(expected.PageSize, actual.PageSize);
    }
}
