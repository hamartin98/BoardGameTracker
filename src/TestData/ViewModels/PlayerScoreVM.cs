using BoardGameTracker.ViewModels;
using BoardGameTracker.ViewModels.Filters;
using Xunit;

namespace BoardGameTracker.TestData.ViewModels;

public partial class ViewModelsTestData
{
    public static PlayerScoreResponseVM AliceScytheScoreVM
    {
        get =>
            new()
            {
                MetaData = ResponseMetaDataVM1,
                PlayerId = CommonTestData.Guid1,
                SessionId = CommonTestData.Guid1,
                Score = 78,
                Win = true,
                Notes = "Polania",
            };
    }

    public static PlayerScoreCreateVM AliceScytheScoreCreateVM
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

    public static PlayerScoreUpdateVM AliceScytheScoreUpdateVM
    {
        get =>
            new()
            {
                Score = 81,
                Win = false,
                Notes = "Polania, recounted",
            };
    }

    public static PlayerScoreFilterVM AliceWinsFilterVM
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

    public static void CheckPlayerScoreResponseVM(
        PlayerScoreResponseVM expected,
        PlayerScoreResponseVM actual
    )
    {
        Assert.Equal(expected.MetaData.Id, actual.MetaData.Id);
        Assert.Equal(expected.MetaData.Version, actual.MetaData.Version);
        Assert.Equal(expected.PlayerId, actual.PlayerId);
        Assert.Equal(expected.SessionId, actual.SessionId);
        Assert.Equal(expected.Score, actual.Score);
        Assert.Equal(expected.Win, actual.Win);
        Assert.Equal(expected.Notes, actual.Notes);
    }
}
