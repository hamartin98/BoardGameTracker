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

    private async Task SeedBoardGamesAsync()
    {
        using var context = ContextFactory.Create();
        context.BoardGames.AddRange(ModelsTestData.Scythe, ModelsTestData.ScytheFenris);
        await context.SaveChangesAsync();
    }

    private PlaySessionService CreateService()
    {
        return new PlaySessionService(ContextFactory.Create(), TimeProvider);
    }

    [Fact]
    public async Task CreateWithExpansionTest()
    {
        await SeedBoardGamesAsync();

        var created = await CreateService()
            .CreateAsync(ModelsTestData.ScytheSessionCreate, CancellationToken.None);

        Assert.Equal([ModelsTestData.ScytheFenris.Id], created.UsedExpansions.Select(e => e.Id));
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
        await SeedBoardGamesAsync();

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService().CreateAsync(ModelsTestData.ScytheFenrisSession, CancellationToken.None)
        );
    }

    [Fact]
    public async Task CreateWithBaseGameAsExpansionThrowsTest()
    {
        await SeedBoardGamesAsync();

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
        await SeedBoardGamesAsync();

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
        await SeedBoardGamesAsync();
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
        await SeedBoardGamesAsync();
        var created = await CreateService()
            .CreateAsync(ModelsTestData.ScytheSessionWithoutExpansions, CancellationToken.None);

        var updated = await CreateService()
            .UpdateAsync(ModelsTestData.ScytheSessionCreate, created.Id, 1, CancellationToken.None);

        Assert.Equal([ModelsTestData.ScytheFenris.Id], updated.UsedExpansions.Select(e => e.Id));
        Assert.Equal(2, updated.Version);
    }

    [Fact]
    public async Task UpdateWithoutChangesKeepsVersionTest()
    {
        await SeedBoardGamesAsync();
        var created = await CreateService()
            .CreateAsync(ModelsTestData.ScytheSessionCreate, CancellationToken.None);

        var updated = await CreateService()
            .UpdateAsync(ModelsTestData.ScytheSessionCreate, created.Id, 1, CancellationToken.None);

        Assert.Equal(1, updated.Version);
    }

    [Fact]
    public async Task StartSetsStartTimeToUtcNowTest()
    {
        await SeedBoardGamesAsync();

        var started = await CreateService()
            .StartAsync(ModelsTestData.ScytheSessionStart, CancellationToken.None);

        Assert.Equal(CommonTestData.DateTime, started.StartTime);
        Assert.Equal(DateTimeKind.Utc, started.StartTime?.Kind);
        Assert.Null(started.Duration);
        Assert.Equal([ModelsTestData.ScytheFenris.Id], started.UsedExpansions.Select(e => e.Id));
    }

    [Fact]
    public async Task StartIgnoresStartTimeAndDurationOnModelTest()
    {
        await SeedBoardGamesAsync();

        var started = await CreateService()
            .StartAsync(ModelsTestData.ScytheSessionCreate, CancellationToken.None);

        Assert.Equal(CommonTestData.DateTime, started.StartTime);
        Assert.Null(started.Duration);
    }

    [Fact]
    public async Task StartForExpansionThrowsTest()
    {
        await SeedBoardGamesAsync();

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService().StartAsync(ModelsTestData.ScytheFenrisSession, CancellationToken.None)
        );
    }
}
