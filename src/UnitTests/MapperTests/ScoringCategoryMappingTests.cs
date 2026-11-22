using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;
using BoardGameTracker.TestData.Models;
using BoardGameTracker.TestData.ViewModels;
using BoardGameTracker.ViewModels;

namespace BoardGameTracker.UnitTests.MapperTests;

public partial class MappingTests
{
    [Fact]
    public void ScoringCategoryModelToResponseMappingTest()
    {
        var expected = ViewModelsTestData.ScytheTerritoriesVM;

        var actual = Mapper.Map<ScoringCategoryResponseVM>(ModelsTestData.ScytheTerritories);

        ViewModelsTestData.CheckScoringCategoryResponseVM(expected, actual);
    }

    [Fact]
    public void ScoringCategoryCreateToModelMappingTest()
    {
        var expected = ModelsTestData.ScytheResourcesCreate;

        var actual = Mapper.Map<ScoringCategory>(ViewModelsTestData.ScytheResourcesCreateVM);

        ModelsTestData.CheckScoringCategory(expected, actual);
    }

    [Fact]
    public void ScoringCategoryUpdateToModelMappingTest()
    {
        var actual = Mapper.Map<ScoringCategory>(ViewModelsTestData.StarTokensUpdateVM);

        Assert.Equal(ModelsTestData.StarTokensUpdate.Name, actual.Name);
        Assert.Equal(Guid.Empty, actual.BoardGameId);
    }

    [Fact]
    public void ScoringCategoryFilterMappingTest()
    {
        var expected = ModelsTestData.ScytheStarsFilter;

        var actual = Mapper.Map<ScoringCategoryFilter>(ViewModelsTestData.ScytheStarsFilterVM);

        ModelsTestData.CheckScoringCategoryFilter(expected, actual);
    }
}
