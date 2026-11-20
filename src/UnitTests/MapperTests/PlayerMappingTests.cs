using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Enums;
using BoardGameTracker.Models.Filters;
using BoardGameTracker.TestData.Models;
using BoardGameTracker.TestData.ViewModels;
using BoardGameTracker.ViewModels;
using BoardGameTracker.ViewModels.Enums;
using BoardGameTracker.ViewModels.Filters;

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
        var createVM = new PlayerCreateVM { Name = "Alice" };

        var actual = Mapper.Map<Player>(createVM);

        ModelsTestData.CheckPlayer(ModelsTestData.Alice, actual);
    }

    [Fact]
    public void PlayerFilterMappingTest()
    {
        var filterVM = new PlayerFilterVM
        {
            Name = "ali",
            Page = 2,
            PageSize = 20,
            OrderBy = "name",
            OrderDirection = OrderByDirectionVM.DESC,
        };

        var actual = Mapper.Map<PlayerFilter>(filterVM);

        Assert.Equal("ali", actual.Name);
        Assert.Equal(2, actual.Page);
        Assert.Equal(20, actual.PageSize);
        Assert.Equal("name", actual.OrderBy);
        Assert.Equal(OrderByDirection.DESC, actual.OrderDirection);
    }
}
