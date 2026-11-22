using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Services;
using BoardGameTracker.TestData;
using BoardGameTracker.TestData.Models;

namespace BoardGameTracker.UnitTests.ServiceTests;

public class PlayerScoreServiceTests
{
    private readonly TestDbContextFactory ContextFactory = new();

    private async Task SeedAsync(params PlayerScore[] scores)
    {
        using var context = ContextFactory.Create();
        context.BoardGames.Add(ModelsTestData.Scythe);
        // Alice and Bob are added as the session's players
        context.PlaySessions.Add(ModelsTestData.ScytheSession);
        context.PlayerScores.AddRange(scores);
        await context.SaveChangesAsync();
    }

    private PlayerScoreService CreateService()
    {
        return new PlayerScoreService(ContextFactory.Create());
    }

    [Fact]
    public async Task CreateTest()
    {
        await SeedAsync();

        var created = await CreateService()
            .CreateAsync(ModelsTestData.AliceScytheScoreCreate, CancellationToken.None);

        ModelsTestData.CheckPlayerScore(ModelsTestData.AliceScytheScoreCreate, created);
    }

    [Fact]
    public async Task CreateForUnknownPlayerThrowsNotFoundTest()
    {
        await SeedAsync();

        await Assert.ThrowsAsync<EntityNotFoundException<Player>>(() =>
            CreateService()
                .CreateAsync(ModelsTestData.UnknownPlayerScoreCreate, CancellationToken.None)
        );
    }

    [Fact]
    public async Task CreateForPlayerNotInSessionThrowsTest()
    {
        using (var context = ContextFactory.Create())
        {
            context.BoardGames.Add(ModelsTestData.Scythe);
            context.Players.Add(ModelsTestData.Alice);
            context.PlaySessions.Add(ModelsTestData.ScytheSessionWithBobOnly);
            await context.SaveChangesAsync();
        }

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService()
                .CreateAsync(ModelsTestData.AliceScytheScoreCreate, CancellationToken.None)
        );
    }

    [Fact]
    public async Task CreateForUnknownSessionThrowsNotFoundTest()
    {
        await SeedAsync();

        await Assert.ThrowsAsync<EntityNotFoundException<PlaySession>>(() =>
            CreateService()
                .CreateAsync(ModelsTestData.UnknownSessionScoreCreate, CancellationToken.None)
        );
    }

    [Fact]
    public async Task CreateSecondScoreForPlayerInSessionThrowsTest()
    {
        await SeedAsync(ModelsTestData.AliceScytheScore);

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService()
                .CreateAsync(ModelsTestData.AliceScytheScoreCreate, CancellationToken.None)
        );
    }

    [Fact]
    public async Task CreateAfterDeletingScoreSucceedsTest()
    {
        await SeedAsync(ModelsTestData.AliceScytheScore);
        await CreateService()
            .DeleteAsync(ModelsTestData.AliceScytheScore.Id, 1, CancellationToken.None);

        var created = await CreateService()
            .CreateAsync(ModelsTestData.AliceScytheScoreCreate, CancellationToken.None);

        Assert.NotEqual(ModelsTestData.AliceScytheScore.Id, created.Id);
    }

    [Fact]
    public async Task UpdateKeepsPlayerAndSessionTest()
    {
        await SeedAsync(ModelsTestData.AliceScytheScore);

        var updated = await CreateService()
            .UpdateAsync(
                ModelsTestData.AliceScytheScoreUpdate,
                ModelsTestData.AliceScytheScore.Id,
                1,
                CancellationToken.None
            );

        Assert.Equal(ModelsTestData.AliceScytheScore.PlayerId, updated.PlayerId);
        Assert.Equal(ModelsTestData.AliceScytheScore.SessionId, updated.SessionId);
        Assert.Equal(ModelsTestData.AliceScytheScoreUpdate.Score, updated.Score);
        Assert.Equal(2, updated.Version);
    }

    [Fact]
    public async Task DeletingSessionDeletesItsScoresTest()
    {
        await SeedAsync(ModelsTestData.AliceScytheScore, ModelsTestData.BobScytheScore);

        await new PlaySessionService(ContextFactory.Create(), TimeProvider.System).DeleteAsync(
            ModelsTestData.ScytheSession.Id,
            1,
            CancellationToken.None
        );

        using var context = ContextFactory.Create();
        Assert.Empty(context.PlayerScores);
    }

    [Fact]
    public async Task DeletingPlayerDeletesItsScoresTest()
    {
        await SeedAsync(ModelsTestData.AliceScytheScore, ModelsTestData.BobScytheScore);

        await new PlayerService(ContextFactory.Create()).DeleteAsync(
            ModelsTestData.Alice.Id,
            1,
            CancellationToken.None
        );

        using var context = ContextFactory.Create();
        Assert.Equal([ModelsTestData.BobScytheScore.Id], context.PlayerScores.Select(e => e.Id));
    }

    [Fact]
    public async Task GetSessionPlayerScoresTest()
    {
        await SeedAsync(ModelsTestData.BobScytheScore, ModelsTestData.AliceScytheScore);

        var scores = await new PlaySessionService(
            ContextFactory.Create(),
            TimeProvider.System
        ).GetPlayerScoresAsync(ModelsTestData.ScytheSession.Id, CancellationToken.None);

        Assert.Equal(
            [ModelsTestData.AliceScytheScore.Id, ModelsTestData.BobScytheScore.Id],
            scores.Select(e => e.Id)
        );
    }

    [Fact]
    public async Task GetPlayerScoresOfUnknownSessionThrowsNotFoundTest()
    {
        await Assert.ThrowsAsync<EntityNotFoundException<PlaySession>>(() =>
            new PlaySessionService(
                ContextFactory.Create(),
                TimeProvider.System
            ).GetPlayerScoresAsync(CommonTestData.DeletedGuid, CancellationToken.None)
        );
    }

    [Fact]
    public async Task GetPlayerScoresOfDeletedSessionThrowsNotFoundTest()
    {
        await SeedAsync(ModelsTestData.AliceScytheScore);
        await new PlaySessionService(ContextFactory.Create(), TimeProvider.System).DeleteAsync(
            ModelsTestData.ScytheSession.Id,
            1,
            CancellationToken.None
        );

        await Assert.ThrowsAsync<EntityNotFoundException<PlaySession>>(() =>
            new PlaySessionService(
                ContextFactory.Create(),
                TimeProvider.System
            ).GetPlayerScoresAsync(ModelsTestData.ScytheSession.Id, CancellationToken.None)
        );
    }
}
