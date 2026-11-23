using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Services;
using BoardGameTracker.TestData;
using BoardGameTracker.TestData.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardGameTracker.UnitTests.ServiceTests;

public class PlayerScoreServiceTests
{
    private readonly TestDbContextFactory ContextFactory = new();

    private async Task SeedAsync(params PlayerScore[] scores)
    {
        using (var context = ContextFactory.Create())
        {
            context.ScoringCategories.AddRange(
                ModelsTestData.ScytheStars,
                ModelsTestData.ScytheTerritories,
                ModelsTestData.FenrisBonus
            );
            await context.SaveChangesAsync();
        }
        await SeedWithoutCategoriesAsync(scores);
    }

    private async Task SeedWithoutCategoriesAsync(params PlayerScore[] scores)
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

        ModelsTestData.CheckPlayerScore(ModelsTestData.AliceScytheScore, created);
    }

    [Fact]
    public async Task CreateWithMissingCategoryThrowsTest()
    {
        await SeedAsync();

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService()
                .CreateAsync(
                    ModelsTestData.AliceScytheScoreMissingCategoryCreate,
                    CancellationToken.None
                )
        );
    }

    [Fact]
    public async Task CreateWithUnknownCategoryThrowsNotFoundTest()
    {
        await SeedAsync();

        await Assert.ThrowsAsync<EntityNotFoundException<ScoringCategory>>(() =>
            CreateService()
                .CreateAsync(
                    ModelsTestData.AliceScytheScoreUnknownCategoryCreate,
                    CancellationToken.None
                )
        );
    }

    [Fact]
    public async Task CreateWithCategoryOfAnotherGameThrowsTest()
    {
        await SeedAsync();
        using (var context = ContextFactory.Create())
        {
            // Fenris is not a used expansion of this session
            var session = context.PlaySessions.Include(e => e.UsedExpansions).Single();
            session.UsedExpansions.Clear();
            await context.SaveChangesAsync();
        }

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService()
                .CreateAsync(ModelsTestData.AliceScytheScoreCreate, CancellationToken.None)
        );
    }

    [Fact]
    public async Task CreateWithoutCategoriesInGameWithoutCategoriesHasNoScoreTest()
    {
        await SeedWithoutCategoriesAsync();

        var created = await CreateService()
            .CreateAsync(ModelsTestData.AliceScoreWithoutCategoriesCreate, CancellationToken.None);

        Assert.Null(created.Score);
        Assert.Empty(created.CategoryScores);
    }

    [Fact]
    public async Task UpdateWithoutCategoriesKeepsRecordedTotalTest()
    {
        await SeedWithoutCategoriesAsync(ModelsTestData.AliceLegacyScytheScore);

        var updated = await CreateService()
            .UpdateAsync(
                ModelsTestData.AliceScoreWithoutCategoriesUpdate,
                ModelsTestData.AliceScytheScore.Id,
                1,
                CancellationToken.None
            );

        Assert.Equal(ModelsTestData.AliceLegacyScytheScore.Score, updated.Score);
        Assert.Equal(2, updated.Version);
    }

    [Fact]
    public async Task UpdateLegacyScoreInGameWithCategoriesRequiresCategoriesTest()
    {
        await SeedAsync(ModelsTestData.AliceLegacyScytheScore);

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService()
                .UpdateAsync(
                    ModelsTestData.AliceScoreWithoutCategoriesUpdate,
                    ModelsTestData.AliceScytheScore.Id,
                    1,
                    CancellationToken.None
                )
        );
    }

    [Fact]
    public async Task UpdateWithSameCategoryScoresKeepsVersionTest()
    {
        await SeedAsync(ModelsTestData.AliceScytheScore);

        var updated = await CreateService()
            .UpdateAsync(
                ModelsTestData.AliceScytheScoreCreate,
                ModelsTestData.AliceScytheScore.Id,
                1,
                CancellationToken.None
            );

        Assert.Equal(1, updated.Version);
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

        ModelsTestData.CheckPlayerScore(ModelsTestData.AliceScytheScoreUpdated, updated);
        Assert.Equal(2, updated.Version);
    }

    [Fact]
    public async Task DeletingSessionDeletesItsScoresTest()
    {
        await SeedAsync(ModelsTestData.AliceScytheScore, ModelsTestData.BobScytheScore);

        await new PlaySessionService(
            ContextFactory.Create(),
            TimeProvider.System,
            Random.Shared
        ).DeleteAsync(ModelsTestData.ScytheSession.Id, 1, CancellationToken.None);

        using var context = ContextFactory.Create();
        Assert.Empty(context.PlayerScores);
    }

    [Fact]
    public async Task DeletingPlayerDeletesItsScoresTest()
    {
        await SeedAsync(ModelsTestData.AliceScytheScore, ModelsTestData.BobScytheScore);

        var playerContext = ContextFactory.Create();
        await new PlayerService(playerContext, new StatisticsService(playerContext)).DeleteAsync(
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
            TimeProvider.System,
            Random.Shared
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
                TimeProvider.System,
                Random.Shared
            ).GetPlayerScoresAsync(CommonTestData.DeletedGuid, CancellationToken.None)
        );
    }

    [Fact]
    public async Task GetPlayerScoresOfDeletedSessionThrowsNotFoundTest()
    {
        await SeedAsync(ModelsTestData.AliceScytheScore);
        await new PlaySessionService(
            ContextFactory.Create(),
            TimeProvider.System,
            Random.Shared
        ).DeleteAsync(ModelsTestData.ScytheSession.Id, 1, CancellationToken.None);

        await Assert.ThrowsAsync<EntityNotFoundException<PlaySession>>(() =>
            new PlaySessionService(
                ContextFactory.Create(),
                TimeProvider.System,
                Random.Shared
            ).GetPlayerScoresAsync(ModelsTestData.ScytheSession.Id, CancellationToken.None)
        );
    }
}
