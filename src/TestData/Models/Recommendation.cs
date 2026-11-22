using BoardGameTracker.Models.Enums;
using BoardGameTracker.Models.Recommendations;
using Xunit;

namespace BoardGameTracker.TestData.Models;

public partial class ModelsTestData
{
    public static RecommendationRequest TwoPlayerEveningRequest
    {
        get =>
            new()
            {
                PlayerCount = 2,
                MinPlayTime = new TimeSpan(1, 0, 0),
                MaxPlayTime = new TimeSpan(2, 0, 0),
                Categories = BoardGameCategory.ECONOMIC | BoardGameCategory.ABSTRACT,
                Limit = 5,
            };
    }

    public static RecommendationRequest TwoPlayerRequest
    {
        get => new() { PlayerCount = 2 };
    }

    public static RecommendationOptions DefaultRecommendationOptions
    {
        get => new();
    }

    public static void CheckRecommendationRequest(
        RecommendationRequest expected,
        RecommendationRequest actual
    )
    {
        Assert.Equal(expected.PlayerCount, actual.PlayerCount);
        Assert.Equal(expected.MinPlayTime, actual.MinPlayTime);
        Assert.Equal(expected.MaxPlayTime, actual.MaxPlayTime);
        Assert.Equal(expected.Categories, actual.Categories);
        Assert.Equal(expected.Limit, actual.Limit);
    }
}
