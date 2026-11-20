using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Services;
using BoardGameTracker.TestData;
using BoardGameTracker.TestData.Models;

namespace BoardGameTracker.UnitTests.ServiceTests;

public class PlayerServiceTests
{
    private readonly TestDbContextFactory ContextFactory = new();

    private async Task<Player> SeedAliceAsync()
    {
        using var context = ContextFactory.Create();
        var alice = ModelsTestData.Alice;
        context.Players.Add(alice);
        await context.SaveChangesAsync();
        return alice;
    }

    private async Task<Player> UpdateAsync(Player model, Guid id, int version)
    {
        using var context = ContextFactory.Create();
        return await new PlayerService(context).UpdateAsync(
            model,
            id,
            version,
            CancellationToken.None
        );
    }

    [Fact]
    public async Task UpdateIncrementsVersionEachTimeTest()
    {
        var alice = await SeedAliceAsync();

        var first = await UpdateAsync(ModelsTestData.AliceRenamed, alice.Id, 1);
        var second = await UpdateAsync(ModelsTestData.AliceRenamedAgain, alice.Id, 2);

        Assert.Equal(2, first.Version);
        Assert.Equal(3, second.Version);
        Assert.Equal(ModelsTestData.AliceRenamedAgain.Name, second.Name);
    }

    [Fact]
    public async Task UpdateKeepsCreatedAtAndRefreshesUpdatedAtTest()
    {
        var alice = await SeedAliceAsync();

        var updated = await UpdateAsync(ModelsTestData.AliceRenamed, alice.Id, 1);

        Assert.Equal(alice.Id, updated.Id);
        Assert.Equal(CommonTestData.DateTime, updated.CreatedAt);
        Assert.True(updated.UpdatedAt > CommonTestData.DateTime);
        Assert.False(updated.IsDeleted);
    }

    [Fact]
    public async Task UpdateWithStaleVersionThrowsTest()
    {
        var alice = await SeedAliceAsync();
        await UpdateAsync(ModelsTestData.AliceRenamed, alice.Id, 1);

        await Assert.ThrowsAsync<VersionMismatchException>(() =>
            UpdateAsync(ModelsTestData.AliceRenamedAgain, alice.Id, 1)
        );
    }

    [Fact]
    public async Task ConcurrentUpdateThrowsVersionMismatchTest()
    {
        var alice = await SeedAliceAsync();
        using var staleContext = ContextFactory.Create();
        await staleContext.Players.FindAsync(alice.Id);

        await UpdateAsync(ModelsTestData.AliceRenamed, alice.Id, 1);

        await Assert.ThrowsAsync<VersionMismatchException>(() =>
            new PlayerService(staleContext).UpdateAsync(
                ModelsTestData.AliceRenamedAgain,
                alice.Id,
                1,
                CancellationToken.None
            )
        );
    }

    [Fact]
    public async Task ConcurrentDeleteThrowsVersionMismatchTest()
    {
        var alice = await SeedAliceAsync();
        using var staleContext = ContextFactory.Create();
        await staleContext.Players.FindAsync(alice.Id);

        await UpdateAsync(ModelsTestData.AliceRenamed, alice.Id, 1);

        await Assert.ThrowsAsync<VersionMismatchException>(() =>
            new PlayerService(staleContext).DeleteAsync(alice.Id, 1, CancellationToken.None)
        );
    }
}
