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
                CategoryScores =
                {
                    new() { ScoringCategoryId = CommonTestData.Guid1, Value = 30 },
                    new() { ScoringCategoryId = CommonTestData.Guid2, Value = 44 },
                    new() { ScoringCategoryId = CommonTestData.Guid3, Value = 4 },
                },
            };
    }

    public static PlayerScoreResponseVM BobScytheScoreVM
    {
        get =>
            new()
            {
                MetaData = ResponseMetaDataVM2,
                PlayerId = CommonTestData.Guid2,
                SessionId = CommonTestData.Guid1,
                Score = 64,
                Win = false,
                Notes = "Saxony",
                CategoryScores =
                {
                    new() { ScoringCategoryId = CommonTestData.Guid1, Value = 24 },
                    new() { ScoringCategoryId = CommonTestData.Guid2, Value = 36 },
                    new() { ScoringCategoryId = CommonTestData.Guid3, Value = 4 },
                },
            };
    }

    public static PlayerScoreCreateVM AliceScytheScoreCreateVM
    {
        get =>
            new()
            {
                PlayerId = CommonTestData.Guid1,
                SessionId = CommonTestData.Guid1,
                Win = true,
                Notes = "Polania",
                CategoryScores =
                {
                    new() { ScoringCategoryId = CommonTestData.Guid1, Value = 30 },
                    new() { ScoringCategoryId = CommonTestData.Guid2, Value = 44 },
                    new() { ScoringCategoryId = CommonTestData.Guid3, Value = 4 },
                },
            };
    }

    public static PlayerScoreUpdateVM AliceScytheScoreUpdateVM
    {
        get =>
            new()
            {
                Win = false,
                Notes = "Polania, recounted",
                CategoryScores =
                {
                    new() { ScoringCategoryId = CommonTestData.Guid1, Value = 31 },
                    new() { ScoringCategoryId = CommonTestData.Guid2, Value = 46 },
                    new() { ScoringCategoryId = CommonTestData.Guid3, Value = 4 },
                },
            };
    }

    public static PlayerScoreResponseVM AliceScytheScoreUpdatedVM
    {
        get
        {
            var score = AliceScytheScoreVM;
            score.MetaData.Version = 2;
            score.Score = 81;
            score.Win = false;
            score.Notes = "Polania, recounted";
            score.CategoryScores = AliceScytheScoreUpdateVM.CategoryScores;
            return score;
        }
    }

    public static PlayerScoreCreateVM AliceScytheWithoutExpansionsScoreCreateVM
    {
        get
        {
            var score = AliceScytheScoreCreateVM;
            score.CategoryScores =
            [
                .. score.CategoryScores.Where(e => e.ScoringCategoryId != CommonTestData.Guid3),
            ];
            return score;
        }
    }

    public static PlayerScoreUpdateVM AliceScytheScoreMissingCategoryUpdateVM
    {
        get
        {
            var score = AliceScytheScoreUpdateVM;
            score.CategoryScores =
            [
                .. score.CategoryScores.Where(e => e.ScoringCategoryId != CommonTestData.Guid3),
            ];
            return score;
        }
    }

    public static PlayerScoreUpdateVM AliceScytheScoreUnknownCategoryUpdateVM
    {
        get
        {
            var score = AliceScytheScoreUpdateVM;
            score.CategoryScores.Add(new() { ScoringCategoryId = CommonTestData.DeletedGuid });
            return score;
        }
    }

    public static PlayerScoreUpdateVM AliceScytheScoreDuplicateCategoryUpdateVM
    {
        get
        {
            var score = AliceScytheScoreUpdateVM;
            score.CategoryScores.Add(new() { ScoringCategoryId = CommonTestData.Guid1, Value = 1 });
            return score;
        }
    }

    public static PlayerScoreCreateVM UnknownPlayerScytheScoreCreateVM
    {
        get
        {
            var score = AliceScytheScoreCreateVM;
            score.PlayerId = CommonTestData.DeletedGuid;
            return score;
        }
    }

    public static PlayerScoreCreateVM AliceUnknownSessionScoreCreateVM
    {
        get
        {
            var score = AliceScytheScoreCreateVM;
            score.SessionId = CommonTestData.DeletedGuid;
            return score;
        }
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

    public static void CheckPlayerScoreVM(PlayerScoreCreateVM expected, PlayerScoreCreateVM actual)
    {
        Assert.Equal(expected.PlayerId, actual.PlayerId);
        Assert.Equal(expected.SessionId, actual.SessionId);
        Assert.Equal(expected.Win, actual.Win);
        Assert.Equal(expected.Notes, actual.Notes);
        Assert.Equal(
            expected
                .CategoryScores.OrderBy(e => e.ScoringCategoryId)
                .Select(e => (e.ScoringCategoryId, e.Value)),
            actual
                .CategoryScores.OrderBy(e => e.ScoringCategoryId)
                .Select(e => (e.ScoringCategoryId, e.Value))
        );
    }

    public static void CheckPlayerScoreResponseVM(
        PlayerScoreResponseVM expected,
        PlayerScoreResponseVM actual
    )
    {
        Assert.Equal(expected.MetaData.Id, actual.MetaData.Id);
        Assert.Equal(expected.MetaData.Version, actual.MetaData.Version);
        Assert.Equal(expected.Score, actual.Score);
        CheckPlayerScoreVM(expected, actual);
    }
}
