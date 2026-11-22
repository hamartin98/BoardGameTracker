using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Services;
using BoardGameTracker.TestData;
using BoardGameTracker.TestData.Models;

namespace BoardGameTracker.UnitTests.ServiceTests;

public class ScoringCategoryServiceTests
{
    private readonly TestDbContextFactory ContextFactory = new();

    private async Task SeedAsync(params PlayerScore[] scores)
    {
        using var context = ContextFactory.Create();
        context.BoardGames.AddRange(ModelsTestData.Scythe, ModelsTestData.ScytheFenris);
        context.ScoringCategories.AddRange(
            ModelsTestData.ScytheStars,
            ModelsTestData.ScytheTerritories,
            ModelsTestData.FenrisBonus
        );
        context.PlayerScores.AddRange(scores);
        await context.SaveChangesAsync();
    }

    private ScoringCategoryService CreateService()
    {
        return new ScoringCategoryService(ContextFactory.Create());
    }

    [Fact]
    public async Task CreateAppendsToScoringOrderTest()
    {
        await SeedAsync();

        var created = await CreateService()
            .CreateAsync(ModelsTestData.ScytheResourcesCreate, CancellationToken.None);

        Assert.Equal(2, created.Order);
    }

    [Fact]
    public async Task CreateWithNameOfAnotherGamesCategoryTest()
    {
        await SeedAsync();

        var created = await CreateService()
            .CreateAsync(ModelsTestData.FenrisStarsCreate, CancellationToken.None);

        Assert.Equal(1, created.Order);
    }

    [Fact]
    public async Task CreateWithDuplicateNameThrowsTest()
    {
        await SeedAsync();

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService()
                .CreateAsync(ModelsTestData.ScytheStarsUppercaseCreate, CancellationToken.None)
        );
    }

    [Fact]
    public async Task CreateForUnknownBoardGameThrowsNotFoundTest()
    {
        await SeedAsync();

        await Assert.ThrowsAsync<EntityNotFoundException<BoardGame>>(() =>
            CreateService()
                .CreateAsync(ModelsTestData.UnknownBoardGameCategoryCreate, CancellationToken.None)
        );
    }

    [Fact]
    public async Task UpdateKeepsBoardGameAndOrderTest()
    {
        await SeedAsync();

        var updated = await CreateService()
            .UpdateAsync(
                ModelsTestData.StarTokensUpdate,
                ModelsTestData.ScytheStars.Id,
                1,
                CancellationToken.None
            );

        Assert.Equal(ModelsTestData.ScytheStars.BoardGameId, updated.BoardGameId);
        Assert.Equal(ModelsTestData.ScytheStars.Order, updated.Order);
        Assert.Equal(ModelsTestData.StarTokensUpdate.Name, updated.Name);
    }

    [Fact]
    public async Task UpdateWithDuplicateNameThrowsTest()
    {
        await SeedAsync();

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService()
                .UpdateAsync(
                    ModelsTestData.TerritoriesUppercaseUpdate,
                    ModelsTestData.ScytheStars.Id,
                    1,
                    CancellationToken.None
                )
        );
    }

    [Fact]
    public async Task DeleteRenumbersLaterCategoriesTest()
    {
        await SeedAsync();

        await CreateService().DeleteAsync(ModelsTestData.ScytheStars.Id, 1, CancellationToken.None);

        var remaining = await CreateService()
            .GetByBoardGameAsync(ModelsTestData.Scythe.Id, CancellationToken.None);
        var territories = Assert.Single(remaining);
        Assert.Equal(0, territories.Order);
        Assert.Equal(2, territories.Version);
    }

    [Fact]
    public async Task DeleteCategoryWithScoresThrowsTest()
    {
        await SeedAsync(ModelsTestData.AliceScytheScore);

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService().DeleteAsync(ModelsTestData.ScytheStars.Id, 1, CancellationToken.None)
        );
    }

    [Fact]
    public async Task DeleteCategoryWithDeletedScoresTest()
    {
        await SeedAsync(ModelsTestData.AliceScytheScore);
        await new PlayerScoreService(ContextFactory.Create()).DeleteAsync(
            ModelsTestData.AliceScytheScore.Id,
            1,
            CancellationToken.None
        );

        await CreateService().DeleteAsync(ModelsTestData.ScytheStars.Id, 1, CancellationToken.None);

        await Assert.ThrowsAsync<EntityNotFoundException<ScoringCategory>>(() =>
            CreateService().GetByIdAsync(ModelsTestData.ScytheStars.Id, CancellationToken.None)
        );
    }

    [Fact]
    public async Task ReorderTest()
    {
        await SeedAsync();

        var reordered = await CreateService()
            .ReorderAsync(
                ModelsTestData.Scythe.Id,
                [ModelsTestData.ScytheTerritories.Id, ModelsTestData.ScytheStars.Id],
                CancellationToken.None
            );

        Assert.Equal(
            [ModelsTestData.ScytheTerritories.Id, ModelsTestData.ScytheStars.Id],
            reordered.Select(e => e.Id)
        );
        Assert.Equal([0, 1], reordered.Select(e => e.Order));
        Assert.All(reordered, e => Assert.Equal(2, e.Version));
    }

    public static TheoryData<Guid[]> InvalidOrders =>
        new()
        {
            { [CommonTestData.Guid1] },
            { [CommonTestData.Guid1, CommonTestData.Guid1] },
            { [CommonTestData.Guid1, CommonTestData.Guid2, CommonTestData.Guid3] },
            { [CommonTestData.Guid1, CommonTestData.Guid3] },
        };

    [Theory]
    [MemberData(nameof(InvalidOrders))]
    public async Task ReorderWithInvalidListThrowsTest(Guid[] orderedIds)
    {
        await SeedAsync();

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService()
                .ReorderAsync(ModelsTestData.Scythe.Id, [.. orderedIds], CancellationToken.None)
        );
    }

    [Fact]
    public async Task ReorderForUnknownBoardGameThrowsNotFoundTest()
    {
        await Assert.ThrowsAsync<EntityNotFoundException<BoardGame>>(() =>
            CreateService().ReorderAsync(CommonTestData.DeletedGuid, [], CancellationToken.None)
        );
    }

    [Fact]
    public async Task GetByBoardGameReturnsScoringOrderTest()
    {
        await SeedAsync();

        var categories = await CreateService()
            .GetByBoardGameAsync(ModelsTestData.Scythe.Id, CancellationToken.None);

        Assert.Equal(
            [ModelsTestData.ScytheStars.Id, ModelsTestData.ScytheTerritories.Id],
            categories.Select(e => e.Id)
        );
    }

    [Fact]
    public async Task GetByUnknownBoardGameThrowsNotFoundTest()
    {
        await Assert.ThrowsAsync<EntityNotFoundException<BoardGame>>(() =>
            CreateService().GetByBoardGameAsync(CommonTestData.DeletedGuid, CancellationToken.None)
        );
    }
}
