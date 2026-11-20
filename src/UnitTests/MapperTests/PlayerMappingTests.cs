using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;
using BoardGameTracker.TestData.Models;
using BoardGameTracker.TestData.ViewModels;
using BoardGameTracker.ViewModels;

namespace BoardGameTracker.UnitTests.MapperTests;

public partial class MappingTests
{
    [Fact]
    public void PlayerModelToResponseMappingTest()
    {
        var expected = ViewModelsTestData.AliceVM;

        var actual = Mapper.Map<PlayerResponseVM>(ModelsTestData.Alice);

        ViewModelsTestData.CheckPlayerResponseVM(expected, actual);
    }

    [Fact]
    public void PlayerCreateToModelMappingTest()
    {
        var expected = ModelsTestData.Alice;

        var actual = Mapper.Map<Player>(ViewModelsTestData.AliceCreateVM);

        ModelsTestData.CheckPlayer(expected, actual);
    }

    [Fact]
    public void PlayersPageToResponseMappingTest()
    {
        var expected = ViewModelsTestData.PlayersPage2VM;

        var actual = Mapper.Map<PlayersResponseVM>(ModelsTestData.PlayersPage2);

        ViewModelsTestData.CheckPlayersResponseVM(expected, actual);
    }

    [Fact]
    public void PlayerFilterMappingTest()
    {
        var expected = ModelsTestData.AliFilter;

        var actual = Mapper.Map<PlayerFilter>(ViewModelsTestData.AliFilterVM);

        ModelsTestData.CheckPlayerFilter(expected, actual);
    }
}
