using BoardGameTracker.Models.Enums;

namespace BoardGameTracker.Services.Statistics;

/// <summary>
/// Play session values the statistics are calculated from
/// </summary>
/// <param name="PlayedAt">Start time of the session, or its creation time without one</param>
/// <param name="Duration">Duration of the session; null while it is not finished</param>
public record SessionRecord(
    Guid Id,
    Guid BoardGameId,
    string BoardGameName,
    List<Guid> UsedExpansionIds,
    PlaySessionType SessionType,
    DateTime PlayedAt,
    TimeSpan? Duration,
    List<Guid> PlayerIds,
    Guid? StartPlayerId,
    List<ScoreRecord> Scores
);

/// <summary>
/// Player score values the statistics are calculated from
/// </summary>
public record ScoreRecord(Guid PlayerId, string PlayerName, int? Score, bool Win);
