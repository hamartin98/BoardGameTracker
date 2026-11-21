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
            };
    }

    public static PlayerScore AliceScytheScoreCreate
    {
        get =>
            new()
            {
                PlayerId = CommonTestData.Guid1,
                SessionId = CommonTestData.Guid1,
                Score = 78,
                Win = true,
                Notes = "Polania",
            };
    }

    public static PlayerScore AliceScytheScoreUpdate
    {
        get =>
            new()
            {
                Score = 81,
                Win = false,
                Notes = "Polania, recounted",
            };
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
