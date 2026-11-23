using BoardGameTracker.Models.Commands;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Enums;
using BoardGameTracker.Models.Filters;
using BoardGameTracker.SeedData;
using Xunit;

namespace BoardGameTracker.TestData.Models;

public partial class ModelsTestData
{
    public static PlaySession ScytheSession
    {
        get => SeededEntities.ScytheSession;
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

    public static CreatePlaySessionCommand ScytheSessionCreate
    {
        get =>
            new()
            {
                Values = new()
                {
                    BoardGameId = CommonTestData.Guid1,
                    StartPlayerId = CommonTestData.Guid1,
                    StartTime = CommonTestData.DateTime,
                    Duration = new TimeSpan(2, 0, 0),
                    SessionType = PlaySessionType.COMPETITIVE,
                    Notes = "First game",
                },
                UsedExpansionIds = [CommonTestData.Guid2],
                PlayerIds = [CommonTestData.Guid1, CommonTestData.Guid2],
            };
    }

    public static CreatePlaySessionCommand ScytheSessionStart
    {
        get
        {
            var session = ScytheSessionCreate;
            session.Values.StartTime = null;
            session.Values.Duration = null;
            session.StartNow = true;
            return session;
        }
    }

    public static CreatePlaySessionCommand ScytheSessionWithoutExpansions
    {
        get
        {
            var session = ScytheSessionCreate;
            session.UsedExpansionIds = [];
            return session;
        }
    }

    public static CreatePlaySessionCommand ScytheSessionWithBaseGameAsExpansion
    {
        get
        {
            var session = ScytheSessionWithoutExpansions;
            session.UsedExpansionIds = [CommonTestData.Guid1];
            return session;
        }
    }

    public static CreatePlaySessionCommand ScytheSessionWithUnknownExpansion
    {
        get
        {
            var session = ScytheSessionWithoutExpansions;
            session.UsedExpansionIds = [CommonTestData.DeletedGuid];
            return session;
        }
    }

    public static CreatePlaySessionCommand ScytheSessionWithoutPlayers
    {
        get
        {
            var session = ScytheSessionCreate;
            session.PlayerIds = [];
            session.Values.StartPlayerId = null;
            return session;
        }
    }

    public static CreatePlaySessionCommand ScytheSessionWithoutAlice
    {
        get
        {
            var session = ScytheSessionCreate;
            session.PlayerIds = [CommonTestData.Guid2];
            session.Values.StartPlayerId = CommonTestData.Guid2;
            return session;
        }
    }

    public static CreatePlaySessionCommand ScytheSessionRandomStartPlayer
    {
        get
        {
            var session = ScytheSessionCreate;
            session.Values.StartPlayerId = null;
            session.RandomStartPlayer = true;
            return session;
        }
    }

    public static CreatePlaySessionCommand ScytheSessionRandomStartPlayerWithoutPlayers
    {
        get
        {
            var session = ScytheSessionRandomStartPlayer;
            session.PlayerIds = [];
            return session;
        }
    }

    public static CreatePlaySessionCommand ScytheSessionWithUnknownPlayer
    {
        get
        {
            var session = ScytheSessionCreate;
            session.PlayerIds = [CommonTestData.Guid1, CommonTestData.DeletedGuid];
            return session;
        }
    }

    public static CreatePlaySessionCommand ScytheSessionWithStartPlayerNotPlaying
    {
        get
        {
            var session = ScytheSessionCreate;
            session.PlayerIds = [CommonTestData.Guid2];
            return session;
        }
    }

    public static CreatePlaySessionCommand ScytheFenrisSession
    {
        get
        {
            var session = ScytheSessionWithoutExpansions;
            session.Values.BoardGameId = CommonTestData.Guid2;
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
        Assert.Equal(expected.StartPlayerId, actual.StartPlayerId);
        Assert.Equal(expected.StartTime, actual.StartTime);
        Assert.Equal(expected.Duration, actual.Duration);
        Assert.Equal(expected.SessionType, actual.SessionType);
        Assert.Equal(expected.Notes, actual.Notes);
    }

    public static void CheckUpdatePlaySessionCommand(
        UpdatePlaySessionCommand expected,
        UpdatePlaySessionCommand actual
    )
    {
        CheckPlaySession(expected.Values, actual.Values);
        Assert.Equal(expected.UsedExpansionIds.Order(), actual.UsedExpansionIds.Order());
        Assert.Equal(expected.PlayerIds.Order(), actual.PlayerIds.Order());
    }

    public static void CheckCreatePlaySessionCommand(
        CreatePlaySessionCommand expected,
        CreatePlaySessionCommand actual
    )
    {
        CheckUpdatePlaySessionCommand(expected, actual);
        Assert.Equal(expected.RandomStartPlayer, actual.RandomStartPlayer);
        Assert.Equal(expected.StartNow, actual.StartNow);
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
