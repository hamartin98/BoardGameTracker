using BoardGameTracker.Mappers;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Enums;
using BoardGameTracker.TestData.Models;
using BoardGameTracker.TestData.ViewModels;
using BoardGameTracker.ViewModels;
using BoardGameTracker.ViewModels.Enums;

namespace BoardGameTracker.UnitTests.MapperTests;

public partial class MappingTests
{
    [Fact]
    public void BoardGameModelToResponseMappingTest()
    {
        var expected = ViewModelsTestData.ScytheVM;

        var actual = Mapper.Map<BoardGameResponseVM>(ModelsTestData.Scythe);

        ViewModelsTestData.CheckBoardGameResponseVM(expected, actual);
    }

    [Fact]
    public void BoardGameUpdateToModelMappingTest()
    {
        var expected = ModelsTestData.ScytheFenrisUpdate;

        var actual = Mapper.Map<BoardGame>(ViewModelsTestData.ScytheFenrisUpdateVM);

        ModelsTestData.CheckBoardGame(expected, actual);
    }

    [Fact]
    public void BoardGameUpdateVMHasNoTypeTest()
    {
        Assert.Null(typeof(BoardGameUpdateVM).GetProperty(nameof(BoardGameCreateVM.Type)));
    }
}
