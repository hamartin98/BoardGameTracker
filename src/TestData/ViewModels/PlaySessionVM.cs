using BoardGameTracker.ViewModels;
using BoardGameTracker.ViewModels.Enums;
using BoardGameTracker.ViewModels.Filters;
using Xunit;

namespace BoardGameTracker.TestData.ViewModels;

public partial class ViewModelsTestData
{
    public static PlaySessionResponseVM ScytheSessionVM
    {
        get =>
            new()
            {
                MetaData = ResponseMetaDataVM1,
                BoardGameId = CommonTestData.Guid1,
                UsedExpansionIds = [CommonTestData.Guid2],
                StartTime = CommonTestData.DateTime,
                Duration = new TimeSpan(2, 0, 0),
                SessionType = PlaySessionTypeVM.COMPETITIVE,
                Notes = "First game",
            };
    }

    public static PlaySessionCreateVM ScytheSessionCreateVM
    {
        get =>
            new()
            {
                BoardGameId = CommonTestData.Guid1,
                UsedExpansionIds = [CommonTestData.Guid2],
                StartTime = CommonTestData.DateTime,
                Duration = new TimeSpan(2, 0, 0),
                SessionType = PlaySessionTypeVM.COMPETITIVE,
                Notes = "First game",
            };
    }

    public static PlaySessionStartVM ScytheSessionStartVM
    {
        get =>
            new()
            {
                BoardGameId = CommonTestData.Guid1,
                UsedExpansionIds = [CommonTestData.Guid2],
                SessionType = PlaySessionTypeVM.COMPETITIVE,
                Notes = "First game",
            };
    }

    public static PlaySessionUpdateVM ScytheSessionUpdateVM
    {
        get =>
            new()
            {
                BoardGameId = CommonTestData.Guid1,
                StartTime = CommonTestData.DateTime,
                Duration = new TimeSpan(2, 0, 0),
                SessionType = PlaySessionTypeVM.COMPETITIVE,
                Notes = "First game",
            };
    }

    public static PlaySessionFilterVM ScytheSessionFilterVM
    {
        get =>
            new()
            {
                BoardGameId = CommonTestData.Guid1,
                SessionTypes = [PlaySessionTypeVM.COMPETITIVE, PlaySessionTypeVM.COOPERATIVE],
                StartTimeFrom = CommonTestData.DateTime,
                StartTimeTo = CommonTestData.DateTime.AddDays(1),
                Page = 2,
                PageSize = 5,
            };
    }

    public static PlaySessionResponseVM ScytheSessionWithScoresVM
    {
        get
        {
            var session = ScytheSessionVM;
            session.PlayerScoreIds = [CommonTestData.Guid1, CommonTestData.Guid2];
            return session;
        }
    }

    public static PlaySessionCreateVM ScytheSessionWithoutExpansionsCreateVM
    {
        get
        {
            var session = ScytheSessionCreateVM;
            session.UsedExpansionIds = [];
            return session;
        }
    }

    public static PlaySessionCreateVM ScytheSessionWithoutStartTimeCreateVM
    {
        get
        {
            var session = ScytheSessionWithoutExpansionsCreateVM;
            session.StartTime = null;
            session.Duration = null;
            return session;
        }
    }

    public static PlaySessionCreateVM ScytheSessionNegativeDurationCreateVM
    {
        get
        {
            var session = ScytheSessionWithoutExpansionsCreateVM;
            session.Duration = new TimeSpan(-1, 0, 0);
            return session;
        }
    }

    public static PlaySessionCreateVM FenrisSessionCreateVM
    {
        get
        {
            var session = ScytheSessionWithoutExpansionsCreateVM;
            session.BoardGameId = CommonTestData.Guid2;
            return session;
        }
    }

    public static PlaySessionCreateVM DeletedBoardGameSessionCreateVM
    {
        get
        {
            var session = ScytheSessionWithoutExpansionsCreateVM;
            session.BoardGameId = CommonTestData.DeletedGuid;
            return session;
        }
    }

    public static PlaySessionCreateVM ScytheSessionWithBaseGameAsExpansionCreateVM
    {
        get
        {
            var session = ScytheSessionCreateVM;
            session.UsedExpansionIds = [CommonTestData.Guid1];
            return session;
        }
    }

    public static PlaySessionCreateVM ScytheSessionWithDeletedExpansionCreateVM
    {
        get
        {
            var session = ScytheSessionCreateVM;
            session.UsedExpansionIds = [CommonTestData.DeletedGuid];
            return session;
        }
    }

    public static PlaySessionStartVM FenrisSessionStartVM
    {
        get
        {
            var session = ScytheSessionStartVM;
            session.BoardGameId = CommonTestData.Guid2;
            session.UsedExpansionIds = [];
            return session;
        }
    }

    public static PlaySessionResponseVM ScytheSessionStartedVM
    {
        get
        {
            var session = ScytheSessionVM;
            session.Duration = null;
            return session;
        }
    }

    public static PlaySessionResponseVM ScytheSessionFinishedVM
    {
        get
        {
            var session = ScytheSessionVM;
            session.Duration = new TimeSpan(1, 30, 0);
            return session;
        }
    }

    public static PlaySessionUpdateVM ScytheSessionRematchUpdateVM
    {
        get
        {
            var session = ScytheSessionUpdateVM;
            session.UsedExpansionIds = [CommonTestData.Guid2];
            session.Duration = new TimeSpan(1, 45, 0);
            session.SessionType = PlaySessionTypeVM.COOPERATIVE;
            session.Notes = "Rematch";
            return session;
        }
    }

    public static PlaySessionUpdateVM FenrisSessionUpdateVM
    {
        get
        {
            var session = ScytheSessionUpdateVM;
            session.BoardGameId = CommonTestData.Guid2;
            return session;
        }
    }

    public static PlaySessionUpdateVM ScytheSessionWithDeletedExpansionUpdateVM
    {
        get
        {
            var session = ScytheSessionUpdateVM;
            session.UsedExpansionIds = [CommonTestData.DeletedGuid];
            return session;
        }
    }

    public static void CheckPlaySessionVM(PlaySessionBaseVM expected, PlaySessionBaseVM actual)
    {
        Assert.Equal(expected.BoardGameId, actual.BoardGameId);
        Assert.Equal(expected.UsedExpansionIds.Order(), actual.UsedExpansionIds.Order());
        Assert.Equal(expected.StartTime, actual.StartTime);
        Assert.Equal(expected.Duration, actual.Duration);
        Assert.Equal(expected.SessionType, actual.SessionType);
        Assert.Equal(expected.Notes, actual.Notes);
    }

    public static void CheckPlaySessionResponseVM(
        PlaySessionResponseVM expected,
        PlaySessionResponseVM actual
    )
    {
        Assert.Equal(expected.MetaData.Id, actual.MetaData.Id);
        Assert.Equal(expected.MetaData.Version, actual.MetaData.Version);
        Assert.Equal(expected.PlayerScoreIds.Order(), actual.PlayerScoreIds.Order());
        CheckPlaySessionVM(expected, actual);
    }
}
