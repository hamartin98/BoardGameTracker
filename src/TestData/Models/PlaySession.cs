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
                StartTime = CommonTestData.DateTime,
                Duration = new TimeSpan(2, 0, 0),
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
