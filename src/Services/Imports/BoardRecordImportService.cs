using System.Globalization;
using System.Text.Json;
using BoardGameTracker.Database;
using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Enums;
using BoardGameTracker.Models.Imports;
using BoardGameTracker.Models.Imports.BoardRecord;
using Microsoft.EntityFrameworkCore;

namespace BoardGameTracker.Services.Imports;

/// <summary>
/// Imports BoardRecord backups; records imported before (by their UUID) are left unchanged
/// </summary>
public class BoardRecordImportService : IImportService
{
    public const string Source = "BoardRecord";

    private const int ExpansionGameType = 2;
    private const byte PlayerCountLowerLimit = 1;
    private const byte PlayerCountUpperLimit = 100;

    // Round types whose values are not points: round win, text and global choice
    private static readonly HashSet<int> NonScoringRoundTypes = [1, 6, 8];

    private static readonly string[] DateFormats =
    [
        "yyyy-MM-dd HH:mm",
        "yyyy-MM-dd HH:mm:ss",
        "yyyy-MM-ddTHH:mm:ss.FFFFFFF",
    ];

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly BoardGameContext Context;
    private readonly TimeProvider TimeProvider;
    private readonly ImportOptions Options;

    public BoardRecordImportService(
        BoardGameContext context,
        TimeProvider timeProvider,
        ImportOptions options
    )
    {
        Context = context;
        TimeProvider = timeProvider;
        Options = options;
    }

    public async Task<ImportResult> ImportBoardRecordAsync(
        Stream backup,
        bool dryRun,
        CancellationToken cancellationToken
    )
    {
        BoardRecordBackup file = await ReadBackupAsync(backup, cancellationToken);
        Import import = new(
            file,
            TimeZoneInfo.FindSystemTimeZoneById(Options.TimeZone),
            TimeProvider.GetUtcNow().UtcDateTime,
            new ImportResult { DryRun = dryRun }
        );

        await import.LoadAsync(Context, cancellationToken);
        import.ImportPlayers();
        import.ImportBoardGames();
        import.ImportScoringCategories();
        import.ImportPlaySessions();

        Context.AddRange(import.NewEntities);
        Context.AddRange(import.NewReferences);
        if (!dryRun)
        {
            await Context.SaveChangesAsync(cancellationToken);
        }
        return import.Result;
    }

    private static async Task<BoardRecordBackup> ReadBackupAsync(
        Stream backup,
        CancellationToken cancellationToken
    )
    {
        try
        {
            return await JsonSerializer.DeserializeAsync<BoardRecordBackup>(
                    backup,
                    JsonOptions,
                    cancellationToken
                ) ?? throw new InvalidImportFileException("The backup file is empty.");
        }
        catch (JsonException exception)
        {
            throw new InvalidImportFileException(
                $"The backup file is not a valid BoardRecord backup: {exception.Message}",
                exception
            );
        }
    }

    /// <summary>
    /// State of one import; entities are looked up in memory, including soft-deleted ones
    /// </summary>
    private sealed class Import(
        BoardRecordBackup file,
        TimeZoneInfo timeZone,
        DateTime now,
        ImportResult result
    )
    {
        public ImportResult Result { get; } = result;

        public List<object> NewEntities { get; } = [];

        public List<ExternalReference> NewReferences { get; } = [];

        private Dictionary<Guid, Guid> References = [];
        private List<Player> Players = [];
        private List<BoardGame> BoardGames = [];
        private List<ScoringCategory> Categories = [];

        // Entities of the source records by source id; null when the entity is deleted
        private readonly Dictionary<long, Player?> PlayerBySourceId = [];
        private readonly Dictionary<long, BoardGame?> GameBySourceId = [];
        private readonly Dictionary<long, ScoringCategory?> CategoryByRoundId = [];

        private readonly HashSet<Guid> CreatedBoardGameIds = [];

        private readonly Dictionary<long, BoardRecordRound> RoundById = file
            .Games.SelectMany(e => e.ScoreSheets)
            .SelectMany(e => e.Rounds)
            .GroupBy(e => e.Id)
            .ToDictionary(e => e.Key, e => e.First());

        public async Task LoadAsync(BoardGameContext context, CancellationToken cancellationToken)
        {
            References = await context
                .ExternalReferences.Where(e => e.Source == Source)
                .ToDictionaryAsync(e => e.ExternalId, e => e.EntityId, cancellationToken);
            Players = await context.Players.IgnoreQueryFilters().ToListAsync(cancellationToken);
            BoardGames = await context
                .BoardGames.IgnoreQueryFilters()
                .ToListAsync(cancellationToken);
            Categories = await context
                .ScoringCategories.IgnoreQueryFilters()
                .ToListAsync(cancellationToken);
        }

        public void ImportPlayers()
        {
            foreach (BoardRecordPlayer source in file.Players)
            {
                if (References.TryGetValue(source.Uuid, out Guid id))
                {
                    PlayerBySourceId[source.Id] = Players.SingleOrDefault(e =>
                        e.Id == id && !e.IsDeleted
                    );
                    Result.Players.AlreadyImported++;
                    continue;
                }

                string name = source.Name.Trim();
                if (name.Length == 0)
                {
                    Warn(Result.Players, $"Player {source.Uuid} has no name.");
                    continue;
                }

                Player? player = Players.FirstOrDefault(e =>
                    !e.IsDeleted && e.NormalizedName == name.ToLowerInvariant()
                );
                if (player is null)
                {
                    player = new Player { Id = Guid.NewGuid(), Name = name };
                    Players.Add(player);
                    NewEntities.Add(player);
                    Result.Players.Created++;
                }
                else
                {
                    Result.Players.Matched++;
                }
                PlayerBySourceId[source.Id] = player;
                AddReference(source.Uuid, nameof(Player), player.Id);
            }
        }

        public void ImportBoardGames()
        {
            Dictionary<long, List<long>> baseIdsByExpansionId = file
                .ObjectGroups.Where(e => e.ObjectId != e.GroupId)
                .GroupBy(e => e.GroupId)
                .ToDictionary(e => e.Key, e => e.Select(e => e.ObjectId).ToList());

            // Base games first, so the expansions can reference them
            foreach (
                BoardRecordGame source in file.Games.OrderBy(e =>
                    baseIdsByExpansionId.ContainsKey(e.Id) || e.GameType == ExpansionGameType
                )
            )
            {
                if (References.TryGetValue(source.Uuid, out Guid id))
                {
                    GameBySourceId[source.Id] = BoardGames.SingleOrDefault(e =>
                        e.Id == id && !e.IsDeleted
                    );
                    Result.BoardGames.AlreadyImported++;
                    continue;
                }

                string name = source.Title.Trim();
                if (name.Length == 0)
                {
                    Warn(Result.BoardGames, $"Game {source.Uuid} has no title.");
                    continue;
                }

                int? bggId = source.BggId > 0 ? source.BggId : null;
                BoardGame? boardGame =
                    BoardGames.FirstOrDefault(e =>
                        !e.IsDeleted && bggId is not null && e.BggId == bggId
                    )
                    ?? BoardGames.FirstOrDefault(e =>
                        !e.IsDeleted && e.NormalizedName == name.ToLowerInvariant()
                    );
                if (boardGame is null)
                {
                    boardGame = CreateBoardGame(
                        source,
                        name,
                        bggId,
                        baseIdsByExpansionId.GetValueOrDefault(source.Id) ?? []
                    );
                    BoardGames.Add(boardGame);
                    NewEntities.Add(boardGame);
                    CreatedBoardGameIds.Add(boardGame.Id);
                    Result.BoardGames.Created++;
                }
                else
                {
                    Result.BoardGames.Matched++;
                }
                GameBySourceId[source.Id] = boardGame;
                AddReference(source.Uuid, nameof(BoardGame), boardGame.Id);
            }
        }

        private BoardGame CreateBoardGame(
            BoardRecordGame source,
            string name,
            int? bggId,
            List<long> baseIds
        )
        {
            List<BoardGame> bases =
            [
                .. baseIds
                    .Select(e => GameBySourceId.GetValueOrDefault(e))
                    .OfType<BoardGame>()
                    .Where(e => e.Type == BoardGameType.BASE_GAME)
                    .Distinct(),
            ];
            // Prefer a base game that existed before this import
            BoardGame? baseGame =
                bases.FirstOrDefault(e => !CreatedBoardGameIds.Contains(e.Id))
                ?? bases.FirstOrDefault();
            if (bases.Count > 1)
            {
                Result.Warnings.Add(
                    $"Expansion '{name}' has several base games; it was added to '{baseGame!.Name}'."
                );
            }
            else if (
                baseGame is null
                && (baseIds.Count > 0 || source.GameType == ExpansionGameType)
            )
            {
                Result.Warnings.Add(
                    $"Expansion '{name}' has no imported base game; it was imported as a base game."
                );
            }

            TimeSpan? playTime =
                source.PlayingTime > 0 ? TimeSpan.FromSeconds(source.PlayingTime.Value) : null;
            (byte minPlayers, byte maxPlayers) = GetPlayerCounts(source, name, baseGame);
            return new BoardGame
            {
                Id = Guid.NewGuid(),
                Name = name,
                BggId = bggId,
                MinPlayTime = playTime,
                MaxPlayTime = playTime,
                MinPlayerCount = minPlayers,
                MaxPlayerCount = maxPlayers,
                Type = baseGame is null ? BoardGameType.BASE_GAME : BoardGameType.EXPANSION,
                ExpansionOfId = baseGame?.Id,
            };
        }

        /// <summary>
        /// Player counts of the game; missing ones come from the base game or the plays of the game
        /// </summary>
        private (byte Min, byte Max) GetPlayerCounts(
            BoardRecordGame source,
            string name,
            BoardGame? baseGame
        )
        {
            int? min = source.MinPlayers;
            int? max = source.MaxPlayers;
            if (min is null || max is null)
            {
                List<int> counts =
                [
                    .. file
                        .Plays.Where(e => e.GameId == source.Id)
                        .Select(e => e.PlayerScores.Count),
                ];
                min ??= (int?)baseGame?.MinPlayerCount ?? (counts.Count > 0 ? counts.Min() : null);
                max ??= (int?)baseGame?.MaxPlayerCount ?? (counts.Count > 0 ? counts.Max() : null);
                Result.Warnings.Add(
                    $"Game '{name}' has no player count; {min ?? PlayerCountLowerLimit} - {max ?? min ?? PlayerCountLowerLimit} was used."
                );
            }
            byte minCount = ClampPlayerCount(min ?? PlayerCountLowerLimit);
            byte maxCount = ClampPlayerCount(max ?? minCount);
            return (Math.Min(minCount, maxCount), maxCount);
        }

        private static byte ClampPlayerCount(int count)
        {
            return (byte)Math.Clamp(count, PlayerCountLowerLimit, PlayerCountUpperLimit);
        }

        /// <summary>
        /// Imports the scoring rounds of each game's first score sheet; an expansion's rounds that
        /// match a category of its base game by name use that category
        /// </summary>
        public void ImportScoringCategories()
        {
            foreach (BoardRecordGame source in file.Games)
            {
                BoardGame? boardGame = GameBySourceId.GetValueOrDefault(source.Id);
                BoardRecordScoreSheet? sheet = source.ScoreSheets.FirstOrDefault();
                if (boardGame is null || sheet is null || !sheet.HasPoints)
                {
                    continue;
                }

                foreach (BoardRecordRound round in sheet.Rounds.Where(IsScoringRound))
                {
                    if (References.TryGetValue(round.Uuid, out Guid id))
                    {
                        CategoryByRoundId[round.Id] = Categories.SingleOrDefault(e =>
                            e.Id == id && !e.IsDeleted
                        );
                        Result.ScoringCategories.AlreadyImported++;
                        continue;
                    }

                    string name = round.Name.Trim();
                    if (name.Length == 0)
                    {
                        Warn(
                            Result.ScoringCategories,
                            $"A scoring round of '{boardGame.Name}' has no name."
                        );
                        continue;
                    }

                    // Same-named rounds of a sheet share one category
                    ScoringCategory? category = Categories.FirstOrDefault(e =>
                        !e.IsDeleted
                        && (
                            e.BoardGameId == boardGame.Id
                            || e.BoardGameId == boardGame.ExpansionOfId
                        )
                        && e.NormalizedName == name.ToLowerInvariant()
                    );
                    if (category is null)
                    {
                        category = new ScoringCategory
                        {
                            Id = Guid.NewGuid(),
                            BoardGameId = boardGame.Id,
                            Name = name,
                            Order = Categories.Count(e =>
                                !e.IsDeleted && e.BoardGameId == boardGame.Id
                            ),
                        };
                        Categories.Add(category);
                        NewEntities.Add(category);
                        Result.ScoringCategories.Created++;
                    }
                    else
                    {
                        Result.ScoringCategories.Matched++;
                    }
                    CategoryByRoundId[round.Id] = category;
                    AddReference(round.Uuid, nameof(ScoringCategory), category.Id);
                }
            }
        }

        private static bool IsScoringRound(BoardRecordRound round)
        {
            return !round.IgnoreScore
                && !round.IsGlobal
                && (
                    round.RoundType is null || !NonScoringRoundTypes.Contains(round.RoundType.Value)
                );
        }

        public void ImportPlaySessions()
        {
            Dictionary<long, BoardRecordGame> gameById = file
                .Games.GroupBy(e => e.Id)
                .ToDictionary(e => e.Key, e => e.First());

            foreach (BoardRecordPlay play in file.Plays)
            {
                if (References.ContainsKey(play.Uuid))
                {
                    Result.PlaySessions.AlreadyImported++;
                    continue;
                }

                string title =
                    gameById.GetValueOrDefault(play.GameId)?.Title.Trim() ?? $"#{play.GameId}";
                string description = $"Play of '{title}' on {play.Date}";
                BoardGame? played = GameBySourceId.GetValueOrDefault(play.GameId);
                BoardGame? baseGame =
                    played?.Type == BoardGameType.EXPANSION
                        ? BoardGames.SingleOrDefault(e =>
                            e.Id == played.ExpansionOfId && !e.IsDeleted
                        )
                        : played;
                if (played is null || baseGame is null)
                {
                    Warn(
                        Result.PlaySessions,
                        $"{description} was skipped: its game is missing or deleted."
                    );
                    continue;
                }

                List<Player?> players =
                [
                    .. play.PlayerScores.Select(e =>
                        PlayerBySourceId.GetValueOrDefault(e.PlayerId)
                    ),
                ];
                if (players.Any(e => e is null))
                {
                    Warn(
                        Result.PlaySessions,
                        $"{description} was skipped: a player is missing or deleted."
                    );
                    continue;
                }
                if (players.Distinct().Count() != players.Count)
                {
                    Warn(
                        Result.PlaySessions,
                        $"{description} was skipped: a player has several scores."
                    );
                    continue;
                }

                BoardRecordGame sourceGame = gameById[play.GameId];
                BoardRecordScoreSheet? sheet = sourceGame.ScoreSheets.FirstOrDefault(e =>
                    e.Id == play.ScoreSheetId
                );
                PlaySession session = CreatePlaySession(play, baseGame, sheet, description);
                if (played != baseGame)
                {
                    session.UsedExpansions.Add(played);
                }
                foreach (Player player in players.OfType<Player>())
                {
                    session.Players.Add(player);
                }

                // Breakdowns are only kept for plays scored on the first score sheet
                List<Guid> sessionCategoryIds =
                    sheet is not null && sheet == sourceGame.ScoreSheets.FirstOrDefault()
                        ?
                        [
                            .. Categories
                                .Where(e =>
                                    !e.IsDeleted
                                    && (e.BoardGameId == baseGame.Id || e.BoardGameId == played.Id)
                                )
                                .Select(e => e.Id),
                        ]
                        : [];
                for (int i = 0; i < play.PlayerScores.Count; i++)
                {
                    session.PlayerScores.Add(
                        CreatePlayerScore(
                            play,
                            play.PlayerScores[i],
                            players[i]!,
                            session,
                            sessionCategoryIds,
                            description
                        )
                    );
                }

                NewEntities.Add(session);
                Result.PlaySessions.Created++;
                AddReference(play.Uuid, nameof(PlaySession), session.Id);
            }
        }

        private PlaySession CreatePlaySession(
            BoardRecordPlay play,
            BoardGame baseGame,
            BoardRecordScoreSheet? sheet,
            string description
        )
        {
            DateTime? startTime = ParseLocalTime(play.Date, description);
            DateTime? createdAt = ParseLocalTime(play.CreateTime, description) ?? startTime;
            PlaySessionType sessionType =
                sheet?.IsCoop == true || play.CoopVictory ? PlaySessionType.COOPERATIVE
                : play.PlayerScores.Count == 1 ? PlaySessionType.SINGLE_PLAYER
                : PlaySessionType.COMPETITIVE;

            return new PlaySession
            {
                Id = Guid.NewGuid(),
                BoardGameId = baseGame.Id,
                StartTime = startTime,
                Duration = play.Duration > 0 ? TimeSpan.FromSeconds(play.Duration.Value) : null,
                StartPlayerId = play.PlayerScores.FirstOrDefault(e => e.StartPlayer) is { } start
                    ? PlayerBySourceId[start.PlayerId]!.Id
                    : null,
                SessionType = sessionType,
                Notes = JoinNotes(play.Comments, GetCampaignNote(play)),
                CreatedAt = createdAt ?? default,
            };
        }

        private string? GetCampaignNote(BoardRecordPlay play)
        {
            if (play.CampaignId is null)
            {
                return null;
            }
            string? name = file.Campaigns.FirstOrDefault(e => e.Id == play.CampaignId)?.Name;
            string scenario = string.IsNullOrWhiteSpace(play.CampaignScenario)
                ? ""
                : $", scenario: {play.CampaignScenario.Trim()}";
            return $"Campaign: {name ?? play.CampaignId.ToString()}{scenario}";
        }

        private PlayerScore CreatePlayerScore(
            BoardRecordPlay play,
            BoardRecordPlayerScore source,
            Player player,
            PlaySession session,
            List<Guid> sessionCategoryIds,
            string description
        )
        {
            PlayerScore score = new()
            {
                Id = Guid.NewGuid(),
                PlayerId = player.Id,
                SessionId = session.Id,
                CreatedAt = session.CreatedAt,
                Score = ToPoints(source.TotalScore, description),
                Win = source.Victory || play.CoopVictory,
                Notes = JoinNotes([
                    .. source
                        .RoundScores.Where(e => !string.IsNullOrWhiteSpace(e.TextValue))
                        .Select(e =>
                            RoundById.TryGetValue(e.RoundId, out BoardRecordRound? round)
                                ? $"{round.Name.Trim()}: {e.TextValue!.Trim()}"
                                : e.TextValue!.Trim()
                        ),
                    string.IsNullOrWhiteSpace(source.Team) ? null : $"Team: {source.Team.Trim()}",
                ]),
            };

            Dictionary<Guid, int>? breakdown = GetBreakdown(
                source,
                score.Score,
                sessionCategoryIds,
                $"{description}, player '{player.Name}'"
            );
            foreach ((Guid categoryId, int value) in breakdown ?? [])
            {
                score.CategoryScores.Add(
                    new CategoryScore
                    {
                        PlayerScoreId = score.Id,
                        ScoringCategoryId = categoryId,
                        Value = value,
                    }
                );
            }
            return score;
        }

        /// <summary>
        /// Points per session category, or null when the rounds don't give a breakdown of the total
        /// </summary>
        private Dictionary<Guid, int>? GetBreakdown(
            BoardRecordPlayerScore source,
            int? total,
            List<Guid> sessionCategoryIds,
            string description
        )
        {
            if (total is null || sessionCategoryIds.Count == 0)
            {
                return null;
            }

            Dictionary<Guid, int> values = sessionCategoryIds.ToDictionary(e => e, _ => 0);
            bool hasValue = false;
            foreach (BoardRecordRoundScore roundScore in source.RoundScores)
            {
                if (!RoundById.TryGetValue(roundScore.RoundId, out BoardRecordRound? round))
                {
                    Result.Warnings.Add(
                        $"{description}: a score of a deleted round; only the total was imported."
                    );
                    return null;
                }
                if (!IsScoringRound(round) || roundScore.Score is null)
                {
                    continue;
                }
                if (
                    CategoryByRoundId.GetValueOrDefault(round.Id) is not ScoringCategory category
                    || !values.ContainsKey(category.Id)
                )
                {
                    return null;
                }
                values[category.Id] += ToPoints(roundScore.Score, description) ?? 0;
                hasValue = true;
            }

            if (!hasValue)
            {
                return null;
            }
            if (values.Values.Sum() != total)
            {
                Result.Warnings.Add(
                    $"{description}: the round scores don't add up to the total; only the total was imported."
                );
                return null;
            }
            return values;
        }

        private int? ToPoints(decimal? value, string description)
        {
            if (value is null)
            {
                return null;
            }
            if (value != decimal.Truncate(value.Value))
            {
                Result.Warnings.Add($"{description}: the score {value} was rounded.");
            }
            return (int)Math.Round(value.Value, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Converts a local time of the backup to UTC
        /// </summary>
        private DateTime? ParseLocalTime(string? value, string description)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            if (
                !DateTime.TryParseExact(
                    value.Trim(),
                    DateFormats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime local
                )
            )
            {
                Result.Warnings.Add($"{description}: the time '{value}' could not be read.");
                return null;
            }
            // A time skipped by a daylight saving change
            if (timeZone.IsInvalidTime(local))
            {
                local = local.AddHours(1);
            }
            return TimeZoneInfo.ConvertTimeToUtc(local, timeZone);
        }

        private static string? JoinNotes(params string?[] notes)
        {
            List<string> parts =
            [
                .. notes.Where(e => !string.IsNullOrWhiteSpace(e)).Select(e => e!.Trim()),
            ];
            return parts.Count == 0 ? null : string.Join("\n", parts);
        }

        private void AddReference(Guid externalId, string entityType, Guid entityId)
        {
            References[externalId] = entityId;
            NewReferences.Add(
                new ExternalReference
                {
                    Id = Guid.NewGuid(),
                    Source = Source,
                    ExternalId = externalId,
                    EntityType = entityType,
                    EntityId = entityId,
                    ImportedAt = now,
                }
            );
        }

        private void Warn(ImportCounts counts, string message)
        {
            counts.Skipped++;
            Result.Warnings.Add(message);
        }
    }
}
