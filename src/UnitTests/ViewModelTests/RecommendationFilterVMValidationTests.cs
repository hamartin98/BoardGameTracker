using System.ComponentModel.DataAnnotations;
using BoardGameTracker.TestData.ViewModels;
using BoardGameTracker.ViewModels.Filters;

namespace BoardGameTracker.UnitTests.ViewModelTests;

public class RecommendationFilterVMValidationTests
{
    public static TheoryData<RecommendationFilterVM, string[]> ValidationCases =>
        new()
        {
            { ViewModelsTestData.TwoPlayerEveningFilterVM, [] },
            { ViewModelsTestData.OnlyPlayerCountFilterVM, [] },
            {
                ViewModelsTestData.WithoutPlayerCountFilterVM,
                ["The PlayerCount field is required."]
            },
            {
                ViewModelsTestData.ZeroPlayerCountFilterVM,
                ["The field PlayerCount must be between 1 and 100."]
            },
            {
                ViewModelsTestData.ReversedPlayTimeFilterVM,
                ["MinPlayTime should be less than MaxPlayTime."]
            },
            { ViewModelsTestData.NegativePlayTimeFilterVM, ["MinPlayTime should be positive."] },
            {
                ViewModelsTestData.ZeroLimitFilterVM,
                ["The field Limit must be between 1 and 2147483647."]
            },
        };

    [Theory]
    [MemberData(nameof(ValidationCases))]
    public void ValidateTest(RecommendationFilterVM filter, string[] expectedErrors)
    {
        List<ValidationResult> errors = [];

        Validator.TryValidateObject(filter, new ValidationContext(filter), errors, true);

        Assert.Equal(expectedErrors, errors.Select(e => e.ErrorMessage));
    }
}
