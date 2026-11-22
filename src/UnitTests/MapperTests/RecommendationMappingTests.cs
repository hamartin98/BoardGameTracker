using BoardGameTracker.Models.Recommendations;
using BoardGameTracker.TestData.Models;
using BoardGameTracker.TestData.ViewModels;

namespace BoardGameTracker.UnitTests.MapperTests;

public partial class MappingTests
{
    [Fact]
    public void RecommendationFilterToRequestMappingTest()
    {
        var expected = ModelsTestData.TwoPlayerEveningRequest;

        var actual = Mapper.Map<RecommendationRequest>(ViewModelsTestData.TwoPlayerEveningFilterVM);

        ModelsTestData.CheckRecommendationRequest(expected, actual);
    }
}
