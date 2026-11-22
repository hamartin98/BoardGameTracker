using BoardGameTracker.ViewModels;
using BoardGameTracker.ViewModels.Enums;
using BoardGameTracker.ViewModels.Filters;
using Xunit;

namespace BoardGameTracker.TestData.ViewModels;

public partial class ViewModelsTestData
{
    public static RecommendationFilterVM TwoPlayerEveningFilterVM
    {
        get =>
            new()
            {
                PlayerCount = 2,
                MinPlayTime = new TimeSpan(1, 0, 0),
                MaxPlayTime = new TimeSpan(2, 0, 0),
                Categories = [BoardGameCategoryVM.ECONOMIC, BoardGameCategoryVM.ABSTRACT],
                Limit = 5,
            };
    }

    public static RecommendationFilterVM WithoutPlayerCountFilterVM
    {
        get
        {
            var filter = TwoPlayerEveningFilterVM;
            filter.PlayerCount = null;
            return filter;
        }
    }

    public static RecommendationFilterVM ZeroPlayerCountFilterVM
    {
        get
        {
            var filter = TwoPlayerEveningFilterVM;
            filter.PlayerCount = 0;
            return filter;
        }
    }

    public static RecommendationFilterVM ReversedPlayTimeFilterVM
    {
        get
        {
            var filter = TwoPlayerEveningFilterVM;
            filter.MinPlayTime = new TimeSpan(3, 0, 0);
            return filter;
        }
    }

    public static RecommendationFilterVM NegativePlayTimeFilterVM
    {
        get
        {
            var filter = TwoPlayerEveningFilterVM;
            filter.MinPlayTime = new TimeSpan(0, -10, 0);
            return filter;
        }
    }

    public static RecommendationFilterVM ZeroLimitFilterVM
    {
        get
        {
            var filter = TwoPlayerEveningFilterVM;
            filter.Limit = 0;
            return filter;
        }
    }

    public static RecommendationFilterVM OnlyPlayerCountFilterVM
    {
        get => new() { PlayerCount = 2 };
    }

    public static BoardGameRecommendationVM ScytheRecommendationVM
    {
        get =>
            new()
            {
                BoardGame = ScytheVM,
                Score = 0,
                TimeScore = null,
                RecencyScore = 0,
                CategoryScore = null,
                AveragePlayTime = new TimeSpan(2, 0, 0),
                LastPlayedAt = CommonTestData.DateTime,
                SessionCount = 1,
            };
    }

    public static BoardGameRecommendationVM ScytheRecommendationAfterHalfLifeVM
    {
        get
        {
            var recommendation = ScytheRecommendationVM;
            recommendation.Score = 50;
            recommendation.RecencyScore = 0.5;
            return recommendation;
        }
    }

    public static BoardGameCreateVM KingdominoCreateVM
    {
        get
        {
            var boardGame = AzulCreateVM;
            boardGame.Name = "Kingdomino";
            boardGame.Categories = [BoardGameCategoryVM.ANIMALS];
            return boardGame;
        }
    }

    public static PlaySessionCreateVM ScytheShortTwoPlayerSessionCreateVM
    {
        get
        {
            var session = ScytheSessionWithoutExpansionsCreateVM;
            session.Duration = new TimeSpan(1, 0, 0);
            return session;
        }
    }

    public static PlaySessionCreateVM ScytheSoloSessionCreateVM
    {
        get
        {
            var session = ScytheSessionWithoutExpansionsCreateVM;
            session.PlayerIds = [CommonTestData.Guid1];
            session.Duration = new TimeSpan(0, 45, 0);
            return session;
        }
    }

    public static void CheckBoardGameRecommendationVM(
        BoardGameRecommendationVM expected,
        BoardGameRecommendationVM actual
    )
    {
        Assert.Equal(expected.BoardGame.MetaData.Id, actual.BoardGame.MetaData.Id);
        Assert.Equal(expected.BoardGame.Name, actual.BoardGame.Name);
        Assert.Equal(expected.Score, actual.Score);
        Assert.Equal(expected.TimeScore, actual.TimeScore);
        Assert.Equal(expected.RecencyScore, actual.RecencyScore, 6);
        Assert.Equal(expected.CategoryScore, actual.CategoryScore);
        Assert.Equal(expected.AveragePlayTime, actual.AveragePlayTime);
        Assert.Equal(expected.LastPlayedAt, actual.LastPlayedAt);
        Assert.Equal(expected.SessionCount, actual.SessionCount);
    }
}
