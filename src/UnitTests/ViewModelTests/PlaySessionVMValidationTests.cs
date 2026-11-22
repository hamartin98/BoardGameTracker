using System.ComponentModel.DataAnnotations;
using BoardGameTracker.TestData.ViewModels;

namespace BoardGameTracker.UnitTests.ViewModelTests;

public class PlaySessionVMValidationTests
{
    private const string NegativeDuration = "Duration should be positive.";
    private const string RandomAndFixedStartPlayer =
        "StartPlayerId cannot be set when RandomStartPlayer is true.";

    public static TheoryData<IValidatableObject, string[]> ValidationCases =>
        new()
        {
            { ViewModelsTestData.ScytheSessionCreateVM, [] },
            { ViewModelsTestData.ScytheSessionRandomStartPlayerCreateVM, [] },
            { ViewModelsTestData.ScytheSessionNegativeDurationCreateVM, [NegativeDuration] },
            {
                ViewModelsTestData.ScytheSessionRandomAndFixedStartPlayerCreateVM,
                [RandomAndFixedStartPlayer]
            },
            { ViewModelsTestData.ScytheSessionStartVM, [] },
            { ViewModelsTestData.ScytheSessionRandomStartPlayerStartVM, [] },
            {
                ViewModelsTestData.ScytheSessionRandomAndFixedStartPlayerStartVM,
                [RandomAndFixedStartPlayer]
            },
        };

    [Theory]
    [MemberData(nameof(ValidationCases))]
    public void ValidateTest(IValidatableObject session, string[] expectedErrors)
    {
        var errors = session.Validate(new ValidationContext(session));

        Assert.Equal(expectedErrors, errors.Select(e => e.ErrorMessage));
    }
}
