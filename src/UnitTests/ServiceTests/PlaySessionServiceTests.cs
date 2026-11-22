using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Services;
using BoardGameTracker.TestData;
using BoardGameTracker.TestData.Models;

namespace BoardGameTracker.UnitTests.ServiceTests;

public class PlaySessionServiceTests
{
    private readonly TestDbContextFactory ContextFactory = new();

    private readonly FixedTimeProvider TimeProvider = new(CommonTestData.DateTime);

    private async Task SeedReferencesAsync()
    {
        using var context = ContextFactory.Create();
        context.BoardGames.AddRange(ModelsTestData.Scythe, ModelsTestData.ScytheFenris);
        context.Players.AddRange(ModelsTestData.Alice, ModelsTestData.Bob);
        await context.SaveChangesAsync();
    }

    private PlaySessionService CreateService()
    {
        return new PlaySessionService(ContextFactory.Create(), TimeProvider);
    }

    [Fact]
    public async Task CreateWithExpansionTest()
    {
        await SeedReferencesAsync();

        var created = await CreateService()
            .CreateAsync(ModelsTestData.ScytheSessionCreate, CancellationToken.None);

        Assert.Equal([ModelsTestData.ScytheFenris.Id], created.UsedExpansions.Select(e => e.Id));
    }

    [Fact]
    public async Task CreateWithPlayersTest()
    {
        await SeedReferencesAsync();

        var created = await CreateService()
            .CreateAsync(ModelsTestData.ScytheSessionCreate, CancellationToken.None);

        Assert.Equal(
            [ModelsTestData.Alice.Id, ModelsTestData.Bob.Id],
            created.Players.Select(e => e.Id).Order()
        );
        Assert.Equal(ModelsTestData.Alice.Id, created.StartPlayerId);
    }

    [Fact]
    public async Task CreateWithUnknownPlayerThrowsNotFoundTest()
    {
        await SeedReferencesAsync();

        await Assert.ThrowsAsync<EntityNotFoundException<Player>>(() =>
            CreateService()
                .CreateAsync(ModelsTestData.ScytheSessionWithUnknownPlayer, CancellationToken.None)
        );
    }

    [Fact]
    public async Task CreateWithStartPlayerNotPlayingThrowsTest()
    {
        await SeedReferencesAsync();

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService()
                .CreateAsync(
                    ModelsTestData.ScytheSessionWithStartPlayerNotPlaying,
                    CancellationToken.None
                )
        );
    }

    [Fact]
    public async Task CreateWithRandomStartPlayerPicksSessionPlayerTest()
    {
        await SeedReferencesAsync();

        var created = await CreateService()
            .CreateAsync(ModelsTestData.ScytheSessionRandomStartPlayer, CancellationToken.None);

        Assert.Contains(created.StartPlayerId!.Value, created.Players.Select(e => e.Id));
    }

    [Fact]
    public async Task CreateWithRandomStartPlayerWithoutPlayersThrowsTest()
    {
        await SeedReferencesAsync();

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService()
                .CreateAsync(
                    ModelsTestData.ScytheSessionRandomStartPlayerWithoutPlayers,
                    CancellationToken.None
                )
        );
    }

    [Fact]
    public async Task StartWithRandomStartPlayerPicksSessionPlayerTest()
    {
        await SeedReferencesAsync();

        var started = await CreateService()
            .StartAsync(ModelsTestData.ScytheSessionRandomStartPlayer, CancellationToken.None);

        Assert.Contains(started.StartPlayerId!.Value, started.Players.Select(e => e.Id));
    }

    [Fact]
    public async Task CreateForUnknownBoardGameThrowsNotFoundTest()
    {
        await Assert.ThrowsAsync<EntityNotFoundException<BoardGame>>(() =>
            CreateService().CreateAsync(ModelsTestData.ScytheSessionCreate, CancellationToken.None)
        );
    }

    [Fact]
    public async Task CreateForExpansionThrowsTest()
    {
        await SeedReferencesAsync();

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService().CreateAsync(ModelsTestData.ScytheFenrisSession, CancellationToken.None)
        );
    }

    [Fact]
    public async Task CreateWithBaseGameAsExpansionThrowsTest()
    {
        await SeedReferencesAsync();

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService()
                .CreateAsync(
                    ModelsTestData.ScytheSessionWithBaseGameAsExpansion,
                    CancellationToken.None
                )
        );
    }

    [Fact]
    public async Task CreateWithUnknownExpansionThrowsNotFoundTest()
    {
        await SeedReferencesAsync();

        await Assert.ThrowsAsync<EntityNotFoundException<BoardGame>>(() =>
            CreateService()
                .CreateAsync(
                    ModelsTestData.ScytheSessionWithUnknownExpansion,
                    CancellationToken.None
                )
        );
    }

    [Fact]
    public async Task UpdateRemovingExpansionsBumpsVersionTest()
    {
        await SeedReferencesAsync();
        var created = await CreateService()
            .CreateAsync(ModelsTestData.ScytheSessionCreate, CancellationToken.None);

        var updated = await CreateService()
            .UpdateAsync(
                ModelsTestData.ScytheSessionWithoutExpansions,
                created.Id,
                1,
                CancellationToken.None
            );

        Assert.Empty(updated.UsedExpansions);
        Assert.Equal(2, updated.Version);
    }

    [Fact]
    public async Task UpdateAddingExpansionsTest()
    {
        await SeedReferencesAsync();
        var created = await CreateService()
            .CreateAsync(ModelsTestData.ScytheSessionWithoutExpansions, CancellationToken.None);

        var updated = await CreateService()
            .UpdateAsync(ModelsTestData.ScytheSessionCreate, created.Id, 1, CancellationToken.None);

        Assert.Equal([ModelsTestData.ScytheFenris.Id], updated.UsedExpansions.Select(e => e.Id));
        Assert.Equal(2, updated.Version);
    }

    [Fact]
    public async Task UpdateRemovingPlayersBumpsVersionTest()
    {
        await SeedReferencesAsync();
        var created = await CreateService()
            .CreateAsync(ModelsTestData.ScytheSessionCreate, CancellationToken.None);

        var updated = await CreateService()
            .UpdateAsync(
                ModelsTestData.ScytheSessionWithoutPlayers,
                created.Id,
                1,
                CancellationToken.None
            );

        Assert.Empty(updated.Players);
        Assert.Null(updated.StartPlayerId);
        Assert.Equal(2, updated.Version);
    }

    [Fact]
    public async Task UpdateWithStartPlayerNotPlayingThrowsTest()
    {
        await SeedReferencesAsync();
        var created = await CreateService()
            .CreateAsync(ModelsTestData.ScytheSessionCreate, CancellationToken.None);

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService()
                .UpdateAsync(
                    ModelsTestData.ScytheSessionWithStartPlayerNotPlaying,
                    created.Id,
                    1,
                    CancellationToken.None
                )
        );
    }

    [Fact]
    public async Task UpdateRemovingPlayerWithScoreThrowsTest()
    {
        using (var context = ContextFactory.Create())
        {
            context.BoardGames.Add(ModelsTestData.Scythe);
            context.PlaySessions.Add(ModelsTestData.ScytheSession);
            context.PlayerScores.Add(ModelsTestData.AliceScytheScore);
            await context.SaveChangesAsync();
        }

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService()
                .UpdateAsync(
                    ModelsTestData.ScytheSessionWithoutAlice,
                    ModelsTestData.ScytheSession.Id,
                    1,
                    CancellationToken.None
                )
        );
    }

    [Fact]
    public async Task UpdateWithoutChangesKeepsVersionTest()
    {
        await SeedReferencesAsync();
        var created = await CreateService()
            .CreateAsync(ModelsTestData.ScytheSessionCreate, CancellationToken.None);

        var updated = await CreateService()
            .UpdateAsync(ModelsTestData.ScytheSessionCreate, created.Id, 1, CancellationToken.None);

        Assert.Equal(1, updated.Version);
    }

    [Fact]
    public async Task StartSetsStartTimeToUtcNowTest()
    {
        await SeedReferencesAsync();

        var started = await CreateService()
            .StartAsync(ModelsTestData.ScytheSessionStart, CancellationToken.None);

        Assert.Equal(CommonTestData.DateTime, started.StartTime);
        Assert.Equal(DateTimeKind.Utc, started.StartTime?.Kind);
        Assert.Null(started.Duration);
        Assert.Equal([ModelsTestData.ScytheFenris.Id], started.UsedExpansions.Select(e => e.Id));
        Assert.Equal(
            [ModelsTestData.Alice.Id, ModelsTestData.Bob.Id],
            started.Players.Select(e => e.Id).Order()
        );
        Assert.Equal(ModelsTestData.Alice.Id, started.StartPlayerId);
    }

    [Fact]
    public async Task StartIgnoresStartTimeAndDurationOnModelTest()
    {
        await SeedReferencesAsync();

        var started = await CreateService()
            .StartAsync(ModelsTestData.ScytheSessionCreate, CancellationToken.None);

        Assert.Equal(CommonTestData.DateTime, started.StartTime);
        Assert.Null(started.Duration);
    }

    [Fact]
    public async Task StartForExpansionThrowsTest()
    {
        await SeedReferencesAsync();

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService().StartAsync(ModelsTestData.ScytheFenrisSession, CancellationToken.None)
        );
    }

    private async Task SeedSessionAsync(PlaySession session)
    {
        using var context = ContextFactory.Create();
        context.PlaySessions.Add(session);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task FinishSetsDurationSinceStartTest()
    {
        await SeedSessionAsync(ModelsTestData.ScytheSessionInProgress);

        var finished = await CreateService()
            .FinishAsync(ModelsTestData.ScytheSession.Id, 1, CancellationToken.None);

        Assert.Equal(TimeSpan.FromHours(2), finished.Duration);
        Assert.Equal(2, finished.Version);
        Assert.Equal([ModelsTestData.ScytheFenris.Id], finished.UsedExpansions.Select(e => e.Id));
        Assert.Equal(
            [ModelsTestData.Alice.Id, ModelsTestData.Bob.Id],
            finished.Players.Select(e => e.Id).Order()
        );
    }

    [Fact]
    public async Task FinishAlreadyFinishedSessionThrowsTest()
    {
        await SeedSessionAsync(ModelsTestData.ScytheSession);

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService().FinishAsync(ModelsTestData.ScytheSession.Id, 1, CancellationToken.None)
        );
    }

    [Fact]
    public async Task FinishSessionWithoutStartTimeThrowsTest()
    {
        await SeedSessionAsync(ModelsTestData.ScytheSessionWithoutStartTime);

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService().FinishAsync(ModelsTestData.ScytheSession.Id, 1, CancellationToken.None)
        );
    }

    [Fact]
    public async Task FinishSessionStartingLaterThrowsTest()
    {
        await SeedSessionAsync(ModelsTestData.ScytheSessionStartingLater);

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService().FinishAsync(ModelsTestData.ScytheSession.Id, 1, CancellationToken.None)
        );
    }

    [Fact]
    public async Task FinishWithStaleVersionThrowsTest()
    {
        await SeedSessionAsync(ModelsTestData.ScytheSessionInProgress);

        await Assert.ThrowsAsync<VersionMismatchException>(() =>
            CreateService().FinishAsync(ModelsTestData.ScytheSession.Id, 2, CancellationToken.None)
        );
    }

    [Fact]
    public async Task FinishUnknownSessionThrowsNotFoundTest()
    {
        await Assert.ThrowsAsync<EntityNotFoundException<PlaySession>>(() =>
            CreateService().FinishAsync(CommonTestData.DeletedGuid, 1, CancellationToken.None)
        );
    }
}
