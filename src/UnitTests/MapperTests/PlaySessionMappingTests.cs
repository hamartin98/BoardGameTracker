using BoardGameTracker.Models.Entities;
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
    public void PlaySessionCreateToModelMappingTest()
    {
        var expected = ModelsTestData.ScytheSessionCreate;

        var actual = Mapper.Map<PlaySession>(ViewModelsTestData.ScytheSessionCreateVM);

        ModelsTestData.CheckPlaySession(expected, actual);
    }

    [Fact]
    public void PlaySessionUpdateToModelMappingTest()
    {
        var expected = ModelsTestData.ScytheSessionWithoutExpansions;

        var actual = Mapper.Map<PlaySession>(ViewModelsTestData.ScytheSessionUpdateVM);

        ModelsTestData.CheckPlaySession(expected, actual);
    }

    [Fact]
    public void PlaySessionFilterMappingTest()
    {
        var expected = ModelsTestData.ScytheSessionFilter;

        var actual = Mapper.Map<PlaySessionFilter>(ViewModelsTestData.ScytheSessionFilterVM);

        ModelsTestData.CheckPlaySessionFilter(expected, actual);
    }
}
