using System.ComponentModel.DataAnnotations;
using BoardGameTracker.TestData.ViewModels;
using BoardGameTracker.ViewModels;

namespace BoardGameTracker.UnitTests.ViewModelTests;

public class PlayerScoreVMValidationTests
{
    private const string DuplicateCategory = "Each scoring category can only be scored once.";

    public static TheoryData<PlayerScoreBaseVM, string[]> ValidationCases =>
        new()
        {
            { ViewModelsTestData.AliceScytheScoreCreateVM, [] },
            { ViewModelsTestData.AliceScytheScoreUpdateVM, [] },
            { ViewModelsTestData.AliceScytheScoreDuplicateCategoryUpdateVM, [DuplicateCategory] },
        };

    [Theory]
    [MemberData(nameof(ValidationCases))]
    public void ValidateTest(PlayerScoreBaseVM score, string[] expectedErrors)
    {
        var errors = score.Validate(new ValidationContext(score));

        Assert.Equal(expectedErrors, errors.Select(e => e.ErrorMessage));
    }
}
