using BoardGameTracker.Models.Entities;
using BoardGameTracker.Services;
using BoardGameTracker.TestData.Models;

namespace BoardGameTracker.UnitTests.ServiceTests;

public class BoardGameServiceTests
{
    private readonly TestDbContextFactory ContextFactory = new();

    private async Task SeedAsync(params BoardGame[] boardGames)
    {
        using var context = ContextFactory.Create();
        context.BoardGames.AddRange(boardGames);
        await context.SaveChangesAsync();
    }

    private BoardGameService CreateService()
    {
        return new BoardGameService(ContextFactory.Create());
    }

    [Fact]
    public async Task CreateWithDuplicateNameThrowsTest()
    {
        await SeedAsync(ModelsTestData.Scythe);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CreateService().CreateAsync(ModelsTestData.ScytheUppercase, CancellationToken.None)
        );
    }

    [Fact]
    public async Task CreateWithDeletedGameNameSucceedsTest()
    {
        await SeedAsync(ModelsTestData.DeletedBoardGame);

        var created = await CreateService()
            .CreateAsync(ModelsTestData.DeletedBoardGameRecreated, CancellationToken.None);

        Assert.Equal(ModelsTestData.DeletedBoardGame.Name, created.Name);
    }

    [Fact]
    public async Task UpdateKeepingOwnNameSucceedsTest()
    {
        await SeedAsync(ModelsTestData.Scythe);

        var updated = await CreateService()
            .UpdateAsync(
                ModelsTestData.ScytheUppercase,
                ModelsTestData.Scythe.Id,
                1,
                CancellationToken.None
            );

        Assert.Equal(ModelsTestData.ScytheUppercase.Name, updated.Name);
    }

    [Fact]
    public async Task UpdateToOtherGamesNameThrowsTest()
    {
        var other = ModelsTestData.DeletedBoardGameRecreated;
        await SeedAsync(ModelsTestData.Scythe, other);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CreateService()
                .UpdateAsync(ModelsTestData.ScytheUppercase, other.Id, 1, CancellationToken.None)
        );
    }
}
