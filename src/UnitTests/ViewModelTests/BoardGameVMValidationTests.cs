using System.ComponentModel.DataAnnotations;
using BoardGameTracker.TestData.ViewModels;
using BoardGameTracker.ViewModels;

namespace BoardGameTracker.UnitTests.ViewModelTests;

public class BoardGameVMValidationTests
{
    private const string MinPlayTimeRange =
        "MinPlayTime should be in the range 00:10:00 - 1.00:00:00.";
    private const string MaxPlayTimeRange =
        "MaxPlayTime should be in the range 00:10:00 - 1.00:00:00.";
    private const string PlayTimeOrder = "MinPlayTime should be less than MaxPlayTime.";
    private const string MinPlayerCountRange = "MinPlayerCount should be in the range 1 - 100.";
    private const string MaxPlayerCountRange = "MaxPlayerCount should be in the range 1 - 100.";
    private const string PlayerCountOrder = "MinPlayerCount should be less than MaxPlayerCount.";
    private const string InvalidBggId = "BggId should be positive.";

    public static TheoryData<BoardGameBaseVM, string[]> ValidationCases =>
        new()
        {
            { ViewModelsTestData.AzulCreateVM, [] },
            { ViewModelsTestData.AzulLimitBoundsCreateVM, [] },
            { ViewModelsTestData.AzulWithoutBggIdCreateVM, [] },
            { ViewModelsTestData.AzulInvalidPlayTimeCreateVM, [MinPlayTimeRange] },
            { ViewModelsTestData.AzulTooLongPlayTimeCreateVM, [MaxPlayTimeRange] },
            { ViewModelsTestData.AzulReversedPlayTimeCreateVM, [PlayTimeOrder] },
            { ViewModelsTestData.AzulNoPlayerCreateVM, [MinPlayerCountRange] },
            { ViewModelsTestData.AzulTooManyPlayersCreateVM, [MaxPlayerCountRange] },
            { ViewModelsTestData.AzulReversedPlayerCountCreateVM, [PlayerCountOrder] },
            { ViewModelsTestData.AzulInvalidBggIdCreateVM, [InvalidBggId] },
            {
                ViewModelsTestData.AzulAllInvalidCreateVM,
                [
                    MinPlayTimeRange,
                    MaxPlayTimeRange,
                    PlayTimeOrder,
                    MinPlayerCountRange,
                    MaxPlayerCountRange,
                    PlayerCountOrder,
                    InvalidBggId,
                ]
            },
        };

    [Theory]
    [MemberData(nameof(ValidationCases))]
    public void ValidateTest(BoardGameBaseVM boardGame, string[] expectedErrors)
    {
        var errors = boardGame.Validate(new ValidationContext(boardGame));

        Assert.Equal(expectedErrors, errors.Select(e => e.ErrorMessage));
    }
}
