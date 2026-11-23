using BoardGameTracker.Models.Commands;
using BoardGameTracker.Models.Filters;
using BoardGameTracker.TestData.Models;
using BoardGameTracker.TestData.ViewModels;
using BoardGameTracker.ViewModels;

namespace BoardGameTracker.UnitTests.MapperTests;

public partial class MappingTests
{
    [Fact]
    public void PlaySessionModelToResponseMappingTest()
    {
        var expected = ViewModelsTestData.ScytheSessionVM;

        var actual = Mapper.Map<PlaySessionResponseVM>(ModelsTestData.ScytheSession);

        ViewModelsTestData.CheckPlaySessionResponseVM(expected, actual);
    }

    [Fact]
    public void PlaySessionCreateToCommandMappingTest()
    {
        var expected = ModelsTestData.ScytheSessionCreate;

        var actual = Mapper.Map<CreatePlaySessionCommand>(ViewModelsTestData.ScytheSessionCreateVM);

        ModelsTestData.CheckCreatePlaySessionCommand(expected, actual);
    }

    [Fact]
    public void PlaySessionRandomStartPlayerCreateToCommandMappingTest()
    {
        var expected = ModelsTestData.ScytheSessionRandomStartPlayer;

        var actual = Mapper.Map<CreatePlaySessionCommand>(
            ViewModelsTestData.ScytheSessionRandomStartPlayerCreateVM
        );

        ModelsTestData.CheckCreatePlaySessionCommand(expected, actual);
    }

    [Fact]
    public void PlaySessionUpdateToCommandMappingTest()
    {
        var expected = ModelsTestData.ScytheSessionWithoutExpansions;

        var actual = Mapper.Map<UpdatePlaySessionCommand>(ViewModelsTestData.ScytheSessionUpdateVM);

        ModelsTestData.CheckUpdatePlaySessionCommand(expected, actual);
    }

    [Fact]
    public void PlaySessionFilterMappingTest()
    {
        var expected = ModelsTestData.ScytheSessionFilter;

        var actual = Mapper.Map<PlaySessionFilter>(ViewModelsTestData.ScytheSessionFilterVM);

        ModelsTestData.CheckPlaySessionFilter(expected, actual);
    }

    [Fact]
    public void PlaySessionStartToCommandMappingTest()
    {
        var expected = ModelsTestData.ScytheSessionStart;

        var actual = Mapper.Map<CreatePlaySessionCommand>(ViewModelsTestData.ScytheSessionStartVM);

        ModelsTestData.CheckCreatePlaySessionCommand(expected, actual);
    }
}
