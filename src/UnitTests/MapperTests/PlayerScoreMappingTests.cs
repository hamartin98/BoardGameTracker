using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;
using BoardGameTracker.TestData.Models;
using BoardGameTracker.TestData.ViewModels;
using BoardGameTracker.ViewModels;

namespace BoardGameTracker.UnitTests.MapperTests;

public partial class MappingTests
{
    [Fact]
    public void PlayerScoreModelToResponseMappingTest()
    {
        var expected = ViewModelsTestData.AliceScytheScoreVM;

        var actual = Mapper.Map<PlayerScoreResponseVM>(ModelsTestData.AliceScytheScore);

        ViewModelsTestData.CheckPlayerScoreResponseVM(expected, actual);
    }

    [Fact]
    public void PlayerScoreCreateToModelMappingTest()
    {
        var expected = ModelsTestData.AliceScytheScoreCreate;

        var actual = Mapper.Map<PlayerScore>(ViewModelsTestData.AliceScytheScoreCreateVM);

        ModelsTestData.CheckPlayerScore(expected, actual);
    }

    [Fact]
    public void PlayerScoreUpdateToModelMappingTest()
    {
        var expected = ModelsTestData.AliceScytheScoreUpdate;

        var actual = Mapper.Map<PlayerScore>(ViewModelsTestData.AliceScytheScoreUpdateVM);

        ModelsTestData.CheckPlayerScore(expected, actual);
    }

    [Fact]
    public void PlayerScoreFilterMappingTest()
    {
        var expected = ModelsTestData.AliceWinsFilter;

        var actual = Mapper.Map<PlayerScoreFilter>(ViewModelsTestData.AliceWinsFilterVM);

        ModelsTestData.CheckPlayerScoreFilter(expected, actual);
    }

    [Fact]
    public void PlayerScoreUpdateVMHasNoPlayerOrSessionTest()
    {
        Assert.Null(typeof(PlayerScoreUpdateVM).GetProperty(nameof(PlayerScoreCreateVM.PlayerId)));
        Assert.Null(typeof(PlayerScoreUpdateVM).GetProperty(nameof(PlayerScoreCreateVM.SessionId)));
    }
}
