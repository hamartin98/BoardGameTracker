using System.Text;
using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Enums;
using BoardGameTracker.Models.Imports;
using BoardGameTracker.Models.Imports.BoardRecord;
using BoardGameTracker.Services.Imports;
using BoardGameTracker.TestData;
using BoardGameTracker.TestData.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardGameTracker.UnitTests.ServiceTests;

public class BoardRecordImportServiceTests
{
    private readonly TestDbContextFactory ContextFactory = new();

    private readonly FixedTimeProvider TimeProvider = new(CommonTestData.DateTime);

    private Task<ImportResult> ImportAsync(BoardRecordBackup backup, bool dryRun = false)
    {
        return ImportAsync(ModelsTestData.ToStream(backup), dryRun);
    }

    private Task<ImportResult> ImportAsync(Stream backup, bool dryRun = false)
    {
        var service = new BoardRecordImportService(
            ContextFactory.Create(),
            TimeProvider,
            new ImportOptions()
        );
        return service.ImportBoardRecordAsync(backup, dryRun, CancellationToken.None);
    }

    private async Task<PlaySession> GetSessionAsync(Guid playUuid)
    {
        using var context = ContextFactory.Create();
        Guid id = await context
            .ExternalReferences.Where(e => e.ExternalId == playUuid)
            .Select(e => e.EntityId)
            .SingleAsync();
        return await context
            .PlaySessions.IgnoreQueryFilters()
            .Include(e => e.UsedExpansions)
            .Include(e => e.Players)
            .Include(e => e.PlayerScores)
                .ThenInclude(e => e.CategoryScores)
                    .ThenInclude(e => e.ScoringCategory)
            .Include(e => e.PlayerScores)
                .ThenInclude(e => e.Player)
            .SingleAsync(e => e.Id == id);
    }

    private async Task<BoardGame> GetBoardGameAsync(string name)
    {
        using var context = ContextFactory.Create();
        return await context
            .BoardGames.Include(e => e.ScoringCategories)
            .SingleAsync(e => e.Name == name);
    }

    private static Dictionary<string, int> CategoryValues(PlayerScore score)
    {
        return score.CategoryScores.ToDictionary(e => e.ScoringCategory!.Name, e => e.Value);
    }

    [Fact]
    public async Task ImportCountsTest()
    {
        ImportResult result = await ImportAsync(ModelsTestData.BoardRecordBackup);

        Assert.False(result.DryRun);
        Assert.Equal(2, result.Players.Created);
        Assert.Equal(3, result.BoardGames.Created);
        Assert.Equal(4, result.ScoringCategories.Created);
        Assert.Equal(1, result.ScoringCategories.Matched);
        Assert.Equal(4, result.PlaySessions.Created);
    }

    [Fact]
    public async Task ImportBoardGamesTest()
    {
        await ImportAsync(ModelsTestData.BoardRecordBackup);

        BoardGame concordia = await GetBoardGameAsync("Concordia");
        Assert.Equal(BoardGameType.BASE_GAME, concordia.Type);
        Assert.Equal(124361, concordia.BggId);
        Assert.Equal(TimeSpan.FromMinutes(100), concordia.MinPlayTime);
        Assert.Equal(TimeSpan.FromMinutes(100), concordia.MaxPlayTime);
        Assert.Equal(
            ["Vesta", "Jupiter"],
            concordia.ScoringCategories.OrderBy(e => e.Order).Select(e => e.Name)
        );

        BoardGame scythe = await GetBoardGameAsync("Scythe");
        Assert.Null(scythe.MinPlayTime);
        Assert.Equal(["Coins"], scythe.ScoringCategories.Select(e => e.Name));
    }

    [Fact]
    public async Task ImportExpansionTest()
    {
        ImportResult result = await ImportAsync(ModelsTestData.BoardRecordBackup);

        BoardGame concordia = await GetBoardGameAsync("Concordia");
        BoardGame salsa = await GetBoardGameAsync("Concordia: Salsa");
        Assert.Equal(BoardGameType.EXPANSION, salsa.Type);
        Assert.Equal(concordia.Id, salsa.ExpansionOfId);
        Assert.Equal(2, salsa.MinPlayerCount);
        Assert.Equal(5, salsa.MaxPlayerCount);
        Assert.Equal(["Salsa card"], salsa.ScoringCategories.Select(e => e.Name));
        Assert.Contains(
            "Game 'Concordia: Salsa' has no player count; 2 - 5 was used.",
            result.Warnings
        );
    }

    [Fact]
    public async Task ImportPlaySessionTest()
    {
        await ImportAsync(ModelsTestData.BoardRecordBackup);

        PlaySession session = await GetSessionAsync(ModelsTestData.BoardRecordCompetitivePlayUuid);
        Assert.Equal((await GetBoardGameAsync("Concordia")).Id, session.BoardGameId);
        Assert.Equal(new DateTime(2024, 7, 1, 18, 0, 0, DateTimeKind.Utc), session.StartTime);
        Assert.Equal(new DateTime(2024, 7, 1, 19, 30, 0, DateTimeKind.Utc), session.CreatedAt);
        Assert.Equal(TimeSpan.FromHours(1), session.Duration);
        Assert.Equal(PlaySessionType.COMPETITIVE, session.SessionType);
        Assert.Equal(["Alice", "Carol"], session.Players.Select(e => e.Name).Order());

        PlayerScore alice = session.PlayerScores.Single(e => e.Player!.Name == "Alice");
        PlayerScore carol = session.PlayerScores.Single(e => e.Player!.Name == "Carol");
        Assert.Equal(alice.PlayerId, session.StartPlayerId);
        Assert.True(alice.Win);
        Assert.False(carol.Win);
        Assert.Equal(30, alice.Score);
        Assert.Equal(new() { ["Vesta"] = 10, ["Jupiter"] = 20 }, CategoryValues(alice));
        Assert.Equal(new() { ["Vesta"] = 0, ["Jupiter"] = 25 }, CategoryValues(carol));
    }

    [Fact]
    public async Task ImportExpansionPlaySessionTest()
    {
        await ImportAsync(ModelsTestData.BoardRecordBackup);

        PlaySession session = await GetSessionAsync(ModelsTestData.BoardRecordExpansionPlayUuid);
        Assert.Equal((await GetBoardGameAsync("Concordia")).Id, session.BoardGameId);
        Assert.Equal(["Concordia: Salsa"], session.UsedExpansions.Select(e => e.Name));
        Assert.Equal(new DateTime(2024, 12, 1, 9, 0, 0, DateTimeKind.Utc), session.StartTime);
        Assert.Null(session.Duration);

        PlayerScore alice = session.PlayerScores.Single(e => e.Player!.Name == "Alice");
        PlayerScore carol = session.PlayerScores.Single(e => e.Player!.Name == "Carol");
        Assert.Equal(
            new()
            {
                ["Vesta"] = 5,
                ["Jupiter"] = 0,
                ["Salsa card"] = 10,
            },
            CategoryValues(alice)
        );
        Assert.Equal(12, carol.Score);
        Assert.Empty(carol.CategoryScores);
    }

    [Fact]
    public async Task ImportPlayOfSecondScoreSheetKeepsTotalTest()
    {
        await ImportAsync(ModelsTestData.BoardRecordBackup);

        PlaySession session = await GetSessionAsync(ModelsTestData.BoardRecordSecondSheetPlayUuid);
        Assert.Equal(PlaySessionType.SINGLE_PLAYER, session.SessionType);
        Assert.Equal("Rematch\nCampaign: Campaign 1, scenario: 2", session.Notes);

        PlayerScore alice = Assert.Single(session.PlayerScores);
        Assert.Equal(50, alice.Score);
        Assert.Empty(alice.CategoryScores);
        Assert.Equal("Faction: Rusviet\nTeam: Team 1", alice.Notes);
    }

    [Fact]
    public async Task ImportCooperativePlayTest()
    {
        ImportResult result = await ImportAsync(ModelsTestData.BoardRecordBackup);

        PlaySession session = await GetSessionAsync(ModelsTestData.BoardRecordCoopPlayUuid);
        Assert.Equal(PlaySessionType.COOPERATIVE, session.SessionType);
        Assert.All(session.PlayerScores, e => Assert.True(e.Win));

        PlayerScore alice = session.PlayerScores.Single(e => e.Player!.Name == "Alice");
        PlayerScore carol = session.PlayerScores.Single(e => e.Player!.Name == "Carol");
        Assert.Equal(40, alice.Score);
        Assert.Empty(alice.CategoryScores);
        Assert.Null(carol.Score);
        Assert.Contains(
            "Play of 'Concordia' on 2025-02-01 18:00, player 'Alice': the round scores don't add up to the total; only the total was imported.",
            result.Warnings
        );
    }

    [Fact]
    public async Task ImportAgainAddsOnlyNewPlaysTest()
    {
        await ImportAsync(ModelsTestData.BoardRecordBackup);

        ImportResult result = await ImportAsync(ModelsTestData.BoardRecordBackupWithNewPlay);

        Assert.Equal(1, result.PlaySessions.Created);
        Assert.Equal(4, result.PlaySessions.AlreadyImported);
        Assert.Equal(0, result.Players.Created + result.BoardGames.Created);
        Assert.Equal(2, result.Players.AlreadyImported);
        Assert.Equal(3, result.BoardGames.AlreadyImported);
        Assert.Equal(5, result.ScoringCategories.AlreadyImported);
        using var context = ContextFactory.Create();
        Assert.Equal(5, await context.PlaySessions.CountAsync());
        Assert.Equal(2, await context.Players.CountAsync());
    }

    [Fact]
    public async Task ImportMatchesExistingEntitiesTest()
    {
        using (var context = ContextFactory.Create())
        {
            context.BoardGames.Add(ModelsTestData.Scythe);
            context.Players.Add(ModelsTestData.Alice);
            await context.SaveChangesAsync();
        }

        ImportResult result = await ImportAsync(ModelsTestData.BoardRecordBackup);

        Assert.Equal(1, result.Players.Matched);
        Assert.Equal(1, result.Players.Created);
        Assert.Equal(1, result.BoardGames.Matched);
        Assert.Equal(2, result.BoardGames.Created);
        PlaySession session = await GetSessionAsync(ModelsTestData.BoardRecordSecondSheetPlayUuid);
        Assert.Equal(ModelsTestData.Scythe.Id, session.BoardGameId);
        Assert.Equal(ModelsTestData.Alice.Id, Assert.Single(session.Players).Id);
    }

    [Fact]
    public async Task ImportExpansionPrefersExistingBaseGameTest()
    {
        using (var context = ContextFactory.Create())
        {
            context.BoardGames.Add(ModelsTestData.Scythe);
            await context.SaveChangesAsync();
        }

        ImportResult result = await ImportAsync(
            ModelsTestData.BoardRecordBackupWithSeveralBaseGames
        );

        BoardGame salsa = await GetBoardGameAsync("Concordia: Salsa");
        Assert.Equal(ModelsTestData.Scythe.Id, salsa.ExpansionOfId);
        Assert.Contains(
            "Expansion 'Concordia: Salsa' has several base games; it was added to 'Scythe'.",
            result.Warnings
        );
    }

    [Fact]
    public async Task ImportSkipsPlayWithMissingPlayerTest()
    {
        ImportResult result = await ImportAsync(ModelsTestData.BoardRecordBackupWithMissingPlayer);

        Assert.Equal(1, result.PlaySessions.Skipped);
        Assert.Equal(3, result.PlaySessions.Created);
        Assert.Contains(
            "Play of 'Concordia' on 2024-07-01 20:00 was skipped: a player is missing or deleted.",
            result.Warnings
        );
    }

    [Fact]
    public async Task ImportDoesNotRestoreDeletedEntitiesTest()
    {
        await ImportAsync(ModelsTestData.BoardRecordBackup);
        using (var context = ContextFactory.Create())
        {
            PlaySession session = await context.PlaySessions.FirstAsync(e =>
                e.SessionType == PlaySessionType.SINGLE_PLAYER
            );
            context.Remove(session);
            await context.SaveChangesAsync();
        }

        ImportResult result = await ImportAsync(ModelsTestData.BoardRecordBackup);

        Assert.Equal(0, result.PlaySessions.Created);
        using var verify = ContextFactory.Create();
        Assert.Equal(3, await verify.PlaySessions.CountAsync());
    }

    [Fact]
    public async Task DryRunSavesNothingTest()
    {
        ImportResult result = await ImportAsync(ModelsTestData.BoardRecordBackup, dryRun: true);

        Assert.True(result.DryRun);
        Assert.Equal(4, result.PlaySessions.Created);
        using var context = ContextFactory.Create();
        Assert.Empty(await context.PlaySessions.ToListAsync());
        Assert.Empty(await context.ExternalReferences.ToListAsync());
    }

    [Fact]
    public async Task ImportReadsBoardRecordFieldNamesTest()
    {
        const string json = """
            {
              "BackupDate": "2026-11-22T10:54:32.9862473+01:00",
              "Players": [{ "Name": "Patrik", "Shortname": "Patrik", "ID": 1, "UUID": "402b503a-b811-45bf-83ed-027fd85a5414" }],
              "Games": [{ "BGGID": 167791, "GameType": 1, "MinPlayers": 1, "MaxPlayers": 5, "PlayingTime": 7200,
                "Title": "Terraforming Mars", "ID": 1, "UUID": "74a7ad42-7998-4dc5-9f82-ee0c0799a284", "ScoreSheets": [] }],
              "Plays": []
            }
            """;

        await ImportAsync(new MemoryStream(Encoding.UTF8.GetBytes(json)));

        BoardGame game = await GetBoardGameAsync("Terraforming Mars");
        Assert.Equal(167791, game.BggId);
        Assert.Equal(TimeSpan.FromHours(2), game.MaxPlayTime);
        using var context = ContextFactory.Create();
        Assert.Equal("Patrik", (await context.Players.SingleAsync()).Name);
    }

    [Fact]
    public async Task ImportInvalidFileThrowsTest()
    {
        await Assert.ThrowsAsync<InvalidImportFileException>(() =>
            ImportAsync(new MemoryStream(Encoding.UTF8.GetBytes("not json")))
        );
    }
}
