using BoardGameTracker.Models.Enums;
using BoardGameTracker.Services.Recommendations;
using BoardGameTracker.TestData;
using BoardGameTracker.TestData.Models;

namespace BoardGameTracker.UnitTests.RecommendationTests;

public class RecommendationScorerTests
{
    private static readonly TimeSpan Tolerance = TimeSpan.FromMinutes(30);

    private static TimeSpan Minutes(int minutes) => TimeSpan.FromMinutes(minutes);

    public static TheoryData<int, int?, int?, double> PointFitCases =>
        new()
        {
            { 90, 60, 120, 1 },
            { 60, 60, 120, 1 },
            { 150, 60, 120, Math.Exp(-1) },
            { 30, 60, null, Math.Exp(-1) },
            { 500, 60, null, 1 },
            { 5, null, null, 1 },
        };

    [Theory]
    [MemberData(nameof(PointFitCases))]
    public void PointFitTest(int time, int? min, int? max, double expected)
    {
        double actual = RecommendationScorer.PointFit(
            Minutes(time),
            min is null ? null : Minutes(min.Value),
            max is null ? null : Minutes(max.Value),
            Tolerance
        );

        Assert.Equal(expected, actual, 6);
    }

    public static TheoryData<int, int, int?, int?, double> RangeFitCases =>
        new()
        {
            // inside
            { 60, 90, 60, 120, 1 },
            // partly above: 20 min inside, 20 min fading from 0 to 20 min
            { 100, 140, null, 120, (20 + 30 * (1 - Math.Exp(-20.0 / 30))) / 40 },
            // fully above: fading from 30 to 60 min
            { 150, 180, 60, 120, 30 * (Math.Exp(-1) - Math.Exp(-2)) / 30 },
            // fully below an open-ended minimum: fading from 15 to 30 min
            { 30, 45, 60, null, 30 * (Math.Exp(-0.5) - Math.Exp(-1)) / 15 },
            // fixed play time inside and outside
            { 115, 115, 60, 120, 1 },
            { 130, 130, 60, 120, Math.Exp(-10.0 / 30) },
        };

    [Theory]
    [MemberData(nameof(RangeFitCases))]
    public void RangeFitTest(int gameMin, int gameMax, int? min, int? max, double expected)
    {
        double actual = RecommendationScorer.RangeFit(
            Minutes(gameMin),
            Minutes(gameMax),
            min is null ? null : Minutes(min.Value),
            max is null ? null : Minutes(max.Value),
            Tolerance
        );

        Assert.Equal(expected, actual, 6);
    }

    [Fact]
    public void RangeFitIsContinuousAtTheRangeEdgeTest()
    {
        double touching = RecommendationScorer.RangeFit(
            Minutes(120),
            Minutes(121),
            null,
            Minutes(120),
            Tolerance
        );

        Assert.True(touching > 0.98);
    }

    [Theory]
    [InlineData(0.5, 1.0, 0.6, 0.8)]
    [InlineData(0.5, null, 0.6, 0.5)]
    public void TimeScoreTest(double rangeFit, double? averageFit, double weight, double expected)
    {
        Assert.Equal(expected, RecommendationScorer.TimeScore(rangeFit, averageFit, weight), 6);
    }

    public static TheoryData<double?, double> RecencyCases =>
        new()
        {
            { null, 1 },
            { 0, 0 },
            { 60, 0.5 },
            { 120, 0.75 },
            { -5, 0 },
        };

    [Theory]
    [MemberData(nameof(RecencyCases))]
    public void RecencyScoreTest(double? daysAgo, double expected)
    {
        DateTime? lastPlayedAt = daysAgo is null
            ? null
            : CommonTestData.DateTime.AddDays(-daysAgo.Value);

        double actual = RecommendationScorer.RecencyScore(
            lastPlayedAt,
            CommonTestData.DateTime,
            60
        );

        Assert.Equal(expected, actual, 6);
    }

    [Theory]
    [InlineData(BoardGameCategory.ECONOMIC | BoardGameCategory.FIGHTING, 0.5)]
    [InlineData(BoardGameCategory.ECONOMIC | BoardGameCategory.ABSTRACT, 1)]
    [InlineData(BoardGameCategory.FIGHTING, 0)]
    public void CategoryScoreTest(BoardGameCategory game, double expected)
    {
        double actual = RecommendationScorer.CategoryScore(
            game,
            BoardGameCategory.ECONOMIC | BoardGameCategory.ABSTRACT
        );

        Assert.Equal(expected, actual, 6);
    }

    [Fact]
    public void TotalLeavesOutUnusedPartsTest()
    {
        double total = RecommendationScorer.Total((0.4, 1), (0.35, 0.5), (0.25, null));

        Assert.Equal(76.7, total);
    }

    [Fact]
    public void TotalWithoutUsedPartsIsZeroTest()
    {
        Assert.Equal(0, RecommendationScorer.Total((0.4, null)));
    }

    [Fact]
    public void ScoreWithOnlyPlayerCountUsesRecencyTest()
    {
        var recommendation = RecommendationScorer.Score(
            ModelsTestData.Scythe,
            new(CommonTestData.DateTime.AddDays(-60), TimeSpan.FromHours(2), 1),
            ModelsTestData.TwoPlayerRequest,
            ModelsTestData.DefaultRecommendationOptions,
            CommonTestData.DateTime
        );

        Assert.Null(recommendation.TimeScore);
        Assert.Null(recommendation.CategoryScore);
        Assert.Equal(0.5, recommendation.RecencyScore, 6);
        Assert.Equal(50, recommendation.Score);
    }

    [Fact]
    public void ScoreUsesAveragePlayTimeTest()
    {
        var withAverage = RecommendationScorer.Score(
            ModelsTestData.Scythe,
            new(null, TimeSpan.FromHours(4), 1),
            ModelsTestData.TwoPlayerEveningRequest,
            ModelsTestData.DefaultRecommendationOptions,
            CommonTestData.DateTime
        );
        var withoutAverage = RecommendationScorer.Score(
            ModelsTestData.Scythe,
            SessionStatistics.NeverPlayed,
            ModelsTestData.TwoPlayerEveningRequest,
            ModelsTestData.DefaultRecommendationOptions,
            CommonTestData.DateTime
        );

        Assert.Equal(1, withoutAverage.TimeScore);
        Assert.True(withAverage.TimeScore < withoutAverage.TimeScore);
        Assert.Equal(0.5, withAverage.CategoryScore);
    }

    [Fact]
    public void ScoreWithoutGamePlayTimeUsesAveragePlayTimeTest()
    {
        var boardGame = ModelsTestData.Scythe;
        boardGame.MinPlayTime = null;
        boardGame.MaxPlayTime = null;

        var withAverage = RecommendationScorer.Score(
            boardGame,
            new(null, TimeSpan.FromHours(4), 1),
            ModelsTestData.TwoPlayerEveningRequest,
            ModelsTestData.DefaultRecommendationOptions,
            CommonTestData.DateTime
        );
        var withoutAverage = RecommendationScorer.Score(
            boardGame,
            SessionStatistics.NeverPlayed,
            ModelsTestData.TwoPlayerEveningRequest,
            ModelsTestData.DefaultRecommendationOptions,
            CommonTestData.DateTime
        );

        Assert.NotNull(withAverage.TimeScore);
        Assert.True(withAverage.TimeScore < 1);
        Assert.Null(withoutAverage.TimeScore);
    }
}
