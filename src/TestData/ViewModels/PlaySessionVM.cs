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

    public static void CheckPlaySessionResponseVM(
        PlaySessionResponseVM expected,
        PlaySessionResponseVM actual
    )
    {
        Assert.Equal(expected.MetaData.Id, actual.MetaData.Id);
        Assert.Equal(expected.MetaData.Version, actual.MetaData.Version);
        Assert.Equal(expected.BoardGameId, actual.BoardGameId);
        Assert.Equal(expected.UsedExpansionIds.Order(), actual.UsedExpansionIds.Order());
        Assert.Equal(expected.PlayerScoreIds.Order(), actual.PlayerScoreIds.Order());
        Assert.Equal(expected.StartTime, actual.StartTime);
        Assert.Equal(expected.Duration, actual.Duration);
        Assert.Equal(expected.SessionType, actual.SessionType);
        Assert.Equal(expected.Notes, actual.Notes);
    }
}
