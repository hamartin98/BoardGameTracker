using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Enums;
using BoardGameTracker.Models.Filters;
using Xunit;

namespace BoardGameTracker.TestData.Models;

public partial class ModelsTestData
{
    public static PlaySession ScytheSession
    {
        get =>
            new()
            {
                Id = CommonTestData.Guid1,
                CreatedAt = CommonTestData.DateTime,
                UpdatedAt = CommonTestData.DateTime,
                BoardGameId = CommonTestData.Guid1,
                UsedExpansions = { ScytheFenris },
                Players = { Alice, Bob },
                StartPlayerId = CommonTestData.Guid1,
                StartTime = CommonTestData.DateTime,
                Duration = new TimeSpan(2, 0, 0),
                SessionType = PlaySessionType.COMPETITIVE,
                Notes = "First game",
            };
    }

    public static PlaySession ScytheSessionCreate
    {
        get =>
            new()
            {
                BoardGameId = CommonTestData.Guid1,
                UsedExpansionIds = [CommonTestData.Guid2],
                PlayerIds = [CommonTestData.Guid1, CommonTestData.Guid2],
                StartPlayerId = CommonTestData.Guid1,
                StartTime = CommonTestData.DateTime,
                Duration = new TimeSpan(2, 0, 0),
                SessionType = PlaySessionType.COMPETITIVE,
                Notes = "First game",
            };
    }

    public static PlaySession ScytheSessionInProgress
    {
        get
        {
            var session = ScytheSession;
            session.StartTime = CommonTestData.DateTime.AddHours(-2);
            session.Duration = null;
            return session;
        }
    }

    public static PlaySession ScytheSessionWithoutStartTime
    {
        get
        {
            var session = ScytheSessionInProgress;
            session.StartTime = null;
            return session;
        }
    }

    public static PlaySession ScytheSessionStartingLater
    {
        get
        {
            var session = ScytheSessionInProgress;
            session.StartTime = CommonTestData.DateTime.AddHours(1);
            return session;
        }
    }

    public static PlaySession ScytheSessionStart
    {
        get =>
            new()
            {
                BoardGameId = CommonTestData.Guid1,
                UsedExpansionIds = [CommonTestData.Guid2],
                PlayerIds = [CommonTestData.Guid1, CommonTestData.Guid2],
                StartPlayerId = CommonTestData.Guid1,
                SessionType = PlaySessionType.COMPETITIVE,
                Notes = "First game",
            };
    }

    public static PlaySession ScytheSessionWithoutExpansions
    {
        get =>
            new()
            {
                BoardGameId = CommonTestData.Guid1,
                PlayerIds = [CommonTestData.Guid1, CommonTestData.Guid2],
                StartPlayerId = CommonTestData.Guid1,
                StartTime = CommonTestData.DateTime,
                Duration = new TimeSpan(2, 0, 0),
                SessionType = PlaySessionType.COMPETITIVE,
                Notes = "First game",
            };
    }

    public static PlaySession ScytheSessionWithBaseGameAsExpansion
    {
        get
        {
            var session = ScytheSessionWithoutExpansions;
            session.UsedExpansionIds = [CommonTestData.Guid1];
            return session;
        }
    }

    public static PlaySession ScytheSessionWithUnknownExpansion
    {
        get
        {
            var session = ScytheSessionWithoutExpansions;
            session.UsedExpansionIds = [CommonTestData.DeletedGuid];
            return session;
        }
    }

    public static PlaySession ScytheSessionWithoutPlayers
    {
        get
        {
            var session = ScytheSessionCreate;
            session.PlayerIds = [];
            session.StartPlayerId = null;
            return session;
        }
    }

    public static PlaySession ScytheSessionWithBobOnly
    {
        get
        {
            var session = ScytheSession;
            session.Players.Clear();
            session.Players.Add(Bob);
            session.StartPlayerId = CommonTestData.Guid2;
            return session;
        }
    }

    public static PlaySession ScytheSessionWithoutAlice
    {
        get
        {
            var session = ScytheSessionCreate;
            session.PlayerIds = [CommonTestData.Guid2];
            session.StartPlayerId = CommonTestData.Guid2;
            return session;
        }
    }

    public static PlaySession ScytheSessionWithUnknownPlayer
    {
        get
        {
            var session = ScytheSessionCreate;
            session.PlayerIds = [CommonTestData.Guid1, CommonTestData.DeletedGuid];
            return session;
        }
    }

    public static PlaySession ScytheSessionWithStartPlayerNotPlaying
    {
        get
        {
            var session = ScytheSessionCreate;
            session.PlayerIds = [CommonTestData.Guid2];
            return session;
        }
    }

    public static PlaySession ScytheFenrisSession
    {
        get
        {
            var session = ScytheSessionWithoutExpansions;
            session.BoardGameId = CommonTestData.Guid2;
            return session;
        }
    }

    public static PlaySessionFilter ScytheSessionFilter
    {
        get =>
            new()
            {
                BoardGameId = CommonTestData.Guid1,
                SessionTypes = [PlaySessionType.COMPETITIVE, PlaySessionType.COOPERATIVE],
                StartTimeFrom = CommonTestData.DateTime,
                StartTimeTo = CommonTestData.DateTime.AddDays(1),
                Page = 2,
                PageSize = 5,
            };
    }

    public static void CheckPlaySession(PlaySession expected, PlaySession actual)
    {
        Assert.Equal(expected.BoardGameId, actual.BoardGameId);
        Assert.Equal(expected.UsedExpansionIds.Order(), actual.UsedExpansionIds.Order());
        Assert.Equal(expected.PlayerIds.Order(), actual.PlayerIds.Order());
        Assert.Equal(expected.StartPlayerId, actual.StartPlayerId);
        Assert.Equal(expected.StartTime, actual.StartTime);
        Assert.Equal(expected.Duration, actual.Duration);
        Assert.Equal(expected.SessionType, actual.SessionType);
        Assert.Equal(expected.Notes, actual.Notes);
    }

    public static void CheckPlaySessionFilter(PlaySessionFilter expected, PlaySessionFilter actual)
    {
        Assert.Equal(expected.BoardGameId, actual.BoardGameId);
        Assert.Equal(expected.SessionTypes, actual.SessionTypes);
        Assert.Equal(expected.StartTimeFrom, actual.StartTimeFrom);
        Assert.Equal(expected.StartTimeTo, actual.StartTimeTo);
        Assert.Equal(expected.Page, actual.Page);
        Assert.Equal(expected.PageSize, actual.PageSize);
    }
}
