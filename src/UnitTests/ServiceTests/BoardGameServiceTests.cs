using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Enums;
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
        var context = ContextFactory.Create();
        return new BoardGameService(context, new StatisticsService(context));
    }

    [Fact]
    public async Task CreateWithDuplicateNameThrowsTest()
    {
        await SeedAsync(ModelsTestData.Scythe);

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
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

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService()
                .UpdateAsync(ModelsTestData.ScytheUppercase, other.Id, 1, CancellationToken.None)
        );
    }

    [Fact]
    public async Task GetExpansionsReturnsExpansionsTest()
    {
        await SeedAsync(ModelsTestData.Scythe, ModelsTestData.ScytheFenris);

        var expansions = await CreateService()
            .GetExpansionsAsync(ModelsTestData.Scythe.Id, CancellationToken.None);

        Assert.Equal([ModelsTestData.ScytheFenris.Id], expansions.Select(e => e.Id));
    }

    [Fact]
    public async Task GetExpansionsOfUnknownGameThrowsTest()
    {
        await Assert.ThrowsAsync<EntityNotFoundException<BoardGame>>(() =>
            CreateService().GetExpansionsAsync(Guid.NewGuid(), CancellationToken.None)
        );
    }

    [Fact]
    public async Task GetExpansionsOfDeletedGameThrowsTest()
    {
        await SeedAsync(ModelsTestData.DeletedBoardGame);

        await Assert.ThrowsAsync<EntityNotFoundException<BoardGame>>(() =>
            CreateService()
                .GetExpansionsAsync(ModelsTestData.DeletedBoardGame.Id, CancellationToken.None)
        );
    }

    [Fact]
    public async Task CreateExpansionOfExpansionThrowsTest()
    {
        await SeedAsync(ModelsTestData.Scythe, ModelsTestData.ScytheFenris);

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService().CreateAsync(ModelsTestData.ExpansionOfExpansion, CancellationToken.None)
        );
    }

    [Fact]
    public async Task UpdateKeepsStoredTypeTest()
    {
        await SeedAsync(ModelsTestData.Scythe, ModelsTestData.ScytheFenris);

        var updated = await CreateService()
            .UpdateAsync(
                ModelsTestData.ScytheFenrisUpdate,
                ModelsTestData.ScytheFenris.Id,
                1,
                CancellationToken.None
            );

        Assert.Equal(BoardGameType.EXPANSION, updated.Type);
        Assert.Equal(ModelsTestData.Scythe.Id, updated.ExpansionOfId);
    }

    [Fact]
    public async Task UpdateBaseGameWithParentThrowsTest()
    {
        await SeedAsync(ModelsTestData.Scythe, ModelsTestData.ScytheFenris);

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService()
                .UpdateAsync(
                    ModelsTestData.ScytheWithParent,
                    ModelsTestData.Scythe.Id,
                    1,
                    CancellationToken.None
                )
        );
    }

    [Fact]
    public async Task UpdateExpansionWithoutParentThrowsTest()
    {
        await SeedAsync(ModelsTestData.Scythe, ModelsTestData.ScytheFenris);

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService()
                .UpdateAsync(
                    ModelsTestData.ScytheFenrisUpdateWithoutParent,
                    ModelsTestData.ScytheFenris.Id,
                    1,
                    CancellationToken.None
                )
        );
    }

    [Fact]
    public async Task UpdateUnknownBoardGameThrowsNotFoundTest()
    {
        await Assert.ThrowsAsync<EntityNotFoundException<BoardGame>>(() =>
            CreateService()
                .UpdateAsync(ModelsTestData.Scythe, Guid.NewGuid(), 1, CancellationToken.None)
        );
    }

    [Fact]
    public async Task UpdateWithStaleVersionThrowsBeforeRuleChecksTest()
    {
        await SeedAsync(ModelsTestData.Scythe, ModelsTestData.ScytheFenris);

        await Assert.ThrowsAsync<VersionMismatchException>(() =>
            CreateService()
                .UpdateAsync(
                    ModelsTestData.ScytheWithParent,
                    ModelsTestData.Scythe.Id,
                    2,
                    CancellationToken.None
                )
        );
    }

    [Fact]
    public async Task CreateExpansionWithoutParentThrowsTest()
    {
        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService()
                .CreateAsync(ModelsTestData.ExpansionWithoutParent, CancellationToken.None)
        );
    }

    [Fact]
    public async Task CreateBaseGameWithParentThrowsTest()
    {
        await SeedAsync(ModelsTestData.ScytheFenris);

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService().CreateAsync(ModelsTestData.ScytheWithParent, CancellationToken.None)
        );
    }

    [Fact]
    public async Task DeleteCascadesToExpansionsSessionsAndScoresTest()
    {
        using (var context = ContextFactory.Create())
        {
            context.BoardGames.Add(ModelsTestData.Scythe);
            context.PlaySessions.Add(ModelsTestData.ScytheSession);
            context.PlayerScores.AddRange(
                ModelsTestData.AliceScytheScore,
                ModelsTestData.BobScytheScore
            );
            await context.SaveChangesAsync();
        }

        await CreateService().DeleteAsync(ModelsTestData.Scythe.Id, 1, CancellationToken.None);

        using var verifyContext = ContextFactory.Create();
        Assert.Empty(verifyContext.BoardGames);
        Assert.Empty(verifyContext.PlaySessions);
        Assert.Empty(verifyContext.PlayerScores);
    }
}
