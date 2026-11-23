using System.Text.Json;
using BoardGameTracker.Models.Imports.BoardRecord;

namespace BoardGameTracker.TestData.Models;

public partial class ModelsTestData
{
    public static readonly Guid BoardRecordAliceUuid = new("b0a2d000-0000-4000-8000-000000000001");
    public static readonly Guid BoardRecordCarolUuid = new("b0a2d000-0000-4000-8000-000000000002");
    public static readonly Guid BoardRecordScytheUuid = new("b0a2d000-0000-4000-8000-000000000011");
    public static readonly Guid BoardRecordConcordiaUuid = new(
        "b0a2d000-0000-4000-8000-000000000012"
    );
    public static readonly Guid BoardRecordSalsaUuid = new("b0a2d000-0000-4000-8000-000000000013");
    public static readonly Guid BoardRecordCompetitivePlayUuid = new(
        "b0a2d000-0000-4000-8000-000000000101"
    );
    public static readonly Guid BoardRecordExpansionPlayUuid = new(
        "b0a2d000-0000-4000-8000-000000000102"
    );
    public static readonly Guid BoardRecordSecondSheetPlayUuid = new(
        "b0a2d000-0000-4000-8000-000000000103"
    );
    public static readonly Guid BoardRecordCoopPlayUuid = new(
        "b0a2d000-0000-4000-8000-000000000104"
    );
    public static readonly Guid BoardRecordNewPlayUuid = new(
        "b0a2d000-0000-4000-8000-000000000105"
    );

    public static BoardRecordBackup BoardRecordBackup
    {
        get =>
            new()
            {
                BackupDate = new DateTimeOffset(2026, 11, 22, 10, 54, 0, TimeSpan.FromHours(1)),
                Players =
                [
                    new()
                    {
                        Id = 1,
                        Uuid = BoardRecordAliceUuid,
                        Name = "Alice",
                    },
                    new()
                    {
                        Id = 2,
                        Uuid = BoardRecordCarolUuid,
                        Name = "Carol ",
                    },
                ],
                Games =
                [
                    new()
                    {
                        Id = 1,
                        Uuid = BoardRecordScytheUuid,
                        Title = "Scythe",
                        BggId = 169786,
                        GameType = 1,
                        MinPlayers = 1,
                        MaxPlayers = 5,
                        ScoreSheets =
                        [
                            new() { Id = 1, Rounds = [Round(1, "Coins"), Round(2, "Faction", 6)] },
                            new() { Id = 2, Rounds = [Round(3, "Coins"), Round(4, "Faction", 6)] },
                        ],
                    },
                    new()
                    {
                        Id = 2,
                        Uuid = BoardRecordConcordiaUuid,
                        Title = "Concordia",
                        BggId = 124361,
                        GameType = 1,
                        MinPlayers = 2,
                        MaxPlayers = 5,
                        PlayingTime = 6000,
                        ScoreSheets =
                        [
                            new() { Id = 3, Rounds = [Round(5, "Vesta"), Round(6, "Jupiter")] },
                        ],
                    },
                    new()
                    {
                        Id = 3,
                        Uuid = BoardRecordSalsaUuid,
                        Title = "Concordia: Salsa",
                        GameType = 2,
                        ScoreSheets =
                        [
                            new() { Id = 4, Rounds = [Round(7, "vesta"), Round(8, "Salsa card")] },
                        ],
                    },
                ],
                ObjectGroups =
                [
                    new() { GroupId = 3, ObjectId = 3 },
                    new() { GroupId = 3, ObjectId = 2 },
                ],
                Campaigns = [new() { Id = 1, Name = "Campaign 1" }],
                Plays =
                [
                    new()
                    {
                        Id = 1,
                        Uuid = BoardRecordCompetitivePlayUuid,
                        Date = "2024-07-01 20:00",
                        CreateTime = "2024-07-01 21:30",
                        GameId = 2,
                        ScoreSheetId = 3,
                        Duration = 3600,
                        PlayerScores =
                        [
                            new()
                            {
                                PlayerId = 1,
                                TotalScore = 30,
                                Victory = true,
                                StartPlayer = true,
                                RoundScores =
                                [
                                    new() { RoundId = 5, Score = 10 },
                                    new() { RoundId = 6, Score = 20 },
                                ],
                            },
                            new()
                            {
                                PlayerId = 2,
                                TotalScore = 25,
                                RoundScores =
                                [
                                    new() { RoundId = 5 },
                                    new() { RoundId = 6, Score = 25 },
                                ],
                            },
                        ],
                    },
                    new()
                    {
                        Id = 2,
                        Uuid = BoardRecordExpansionPlayUuid,
                        Date = "2024-12-01 10:00",
                        GameId = 3,
                        ScoreSheetId = 4,
                        PlayerScores =
                        [
                            new()
                            {
                                PlayerId = 1,
                                TotalScore = 15,
                                RoundScores =
                                [
                                    new() { RoundId = 7, Score = 5 },
                                    new() { RoundId = 8, Score = 10 },
                                ],
                            },
                            new()
                            {
                                PlayerId = 2,
                                TotalScore = 12,
                                Victory = false,
                                RoundScores = [new() { RoundId = 7 }, new() { RoundId = 8 }],
                            },
                        ],
                    },
                    new()
                    {
                        Id = 3,
                        Uuid = BoardRecordSecondSheetPlayUuid,
                        Date = "2025-01-10 18:00",
                        GameId = 1,
                        ScoreSheetId = 2,
                        Comments = "Rematch",
                        CampaignId = 1,
                        CampaignScenario = "2",
                        PlayerScores =
                        [
                            new()
                            {
                                PlayerId = 1,
                                TotalScore = 50,
                                Victory = true,
                                Team = "Team 1",
                                RoundScores =
                                [
                                    new() { RoundId = 3, Score = 50 },
                                    new() { RoundId = 4, TextValue = "Rusviet" },
                                ],
                            },
                        ],
                    },
                    new()
                    {
                        Id = 4,
                        Uuid = BoardRecordCoopPlayUuid,
                        Date = "2025-02-01 18:00",
                        GameId = 2,
                        ScoreSheetId = 3,
                        CoopVictory = true,
                        PlayerScores =
                        [
                            new()
                            {
                                PlayerId = 1,
                                TotalScore = 40,
                                RoundScores =
                                [
                                    new() { RoundId = 5, Score = 10 },
                                    new() { RoundId = 6, Score = 20 },
                                ],
                            },
                            new() { PlayerId = 2 },
                        ],
                    },
                ],
            };
    }

    public static BoardRecordBackup BoardRecordBackupWithNewPlay
    {
        get
        {
            BoardRecordBackup backup = BoardRecordBackup;
            backup.Plays.Add(
                new()
                {
                    Id = 5,
                    Uuid = BoardRecordNewPlayUuid,
                    Date = "2026-11-21 09:00",
                    GameId = 2,
                    ScoreSheetId = 3,
                    PlayerScores = [new() { PlayerId = 2, TotalScore = 33 }],
                }
            );
            return backup;
        }
    }

    public static BoardRecordBackup BoardRecordBackupWithSeveralBaseGames
    {
        get
        {
            BoardRecordBackup backup = BoardRecordBackup;
            backup.ObjectGroups.Add(new() { GroupId = 3, ObjectId = 1 });
            return backup;
        }
    }

    public static BoardRecordBackup BoardRecordBackupWithMissingPlayer
    {
        get
        {
            BoardRecordBackup backup = BoardRecordBackup;
            backup.Plays[0].PlayerScores[1].PlayerId = 99;
            return backup;
        }
    }

    public static MemoryStream ToStream(BoardRecordBackup backup)
    {
        return new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(backup));
    }

    private static BoardRecordRound Round(long id, string name, int? roundType = null)
    {
        return new()
        {
            Id = id,
            Uuid = new Guid($"b0a2d000-0000-4000-8000-1000000000{id:D2}"),
            Name = name,
            RoundType = roundType,
        };
    }
}
