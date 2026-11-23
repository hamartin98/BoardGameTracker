using BoardGameTracker.Models.Enums;
using BoardGameTracker.Services.Statistics;
using BoardGameTracker.TestData;

namespace BoardGameTracker.UnitTests.StatisticsTests;

public class StatisticsCalculatorTests
{
    private static readonly Guid GameId = Guid.NewGuid();
    private static readonly Guid OtherGameId = Guid.NewGuid();
    private static readonly Guid Alice = Guid.NewGuid();
    private static readonly Guid Bob = Guid.NewGuid();
    private static readonly Guid Carol = Guid.NewGuid();

    private static DateTime Day(int day) => CommonTestData.DateTime.AddDays(day);

    private static ScoreRecord Score(Guid playerId, int? score, bool win = false)
    {
        string name =
            playerId == Alice ? "Alice"
            : playerId == Bob ? "Bob"
            : "Carol";
        return new(playerId, name, score, win);
    }

    private static SessionRecord Session(
        DateTime playedAt,
        PlaySessionType type = PlaySessionType.COMPETITIVE,
        int? minutes = 60,
        Guid? startPlayerId = null,
        Guid? boardGameId = null,
        params ScoreRecord[] scores
    )
    {
        Guid gameId = boardGameId ?? GameId;
        List<Guid> playerIds = [.. scores.Select(e => e.PlayerId)];
        return new(
            Guid.NewGuid(),
            gameId,
            gameId == GameId ? "Game" : "Other game",
            [],
            type,
            playedAt,
            minutes is null ? null : TimeSpan.FromMinutes(minutes.Value),
            playerIds,
            startPlayerId,
            [.. scores]
        );
    }

    private static List<SessionRecord> Sessions =>
        [
            Session(
                Day(0),
                minutes: 60,
                startPlayerId: Alice,
                scores: [Score(Alice, 80, true), Score(Bob, 60)]
            ),
            Session(
                Day(1),
                minutes: 90,
                startPlayerId: Alice,
                scores: [Score(Alice, 50), Score(Bob, 70, true)]
            ),
            Session(
                Day(2),
                minutes: 120,
                scores: [Score(Alice, 80, true), Score(Bob, 40), Score(Carol, null)]
            ),
            // Unfinished: counted as a play but not in the play times
            Session(Day(3), minutes: null, scores: [Score(Alice, null), Score(Bob, null)]),
            Session(
                Day(4),
                PlaySessionType.COOPERATIVE,
                minutes: 30,
                scores: [Score(Alice, 100, true), Score(Bob, 100, true)]
            ),
            Session(
                Day(5),
                PlaySessionType.COOPERATIVE,
                minutes: 30,
                scores: [Score(Alice, 10), Score(Bob, 10)]
            ),
            Session(
                Day(6),
                PlaySessionType.SINGLE_PLAYER,
                minutes: 45,
                scores: [Score(Bob, 999, true)]
            ),
        ];

    [Fact]
    public void BoardGameWithoutSessionsTest()
    {
        var actual = BoardGameStatisticsCalculator.Calculate([]);

        Assert.Equal(0, actual.PlayCount);
        Assert.Null(actual.LastPlayedAt);
        Assert.Equal(TimeSpan.Zero, actual.TotalPlayTime);
        Assert.Empty(actual.AveragePlayTimes);
        Assert.Empty(actual.PlayerWinRates);
        Assert.Null(actual.HighScore);
        Assert.Null(actual.AverageScore);
        Assert.Null(actual.AverageWinningScore);
        Assert.Null(actual.StartPlayerWinRate);
        Assert.Null(actual.CooperativeWinRate);
    }

    [Fact]
    public void BoardGamePlaysAndPlayTimesTest()
    {
        var actual = BoardGameStatisticsCalculator.Calculate(Sessions);

        Assert.Equal(7, actual.PlayCount);
        Assert.Equal(Day(6), actual.LastPlayedAt);
        Assert.Equal(TimeSpan.FromMinutes(60 + 90 + 120 + 30 + 30 + 45), actual.TotalPlayTime);
        Assert.Collection(
            actual.AveragePlayTimes,
            e =>
            {
                Assert.Equal(1, e.PlayerCount);
                Assert.Equal(TimeSpan.FromMinutes(45), e.AveragePlayTime);
                Assert.Equal(1, e.SessionCount);
            },
            e =>
            {
                Assert.Equal(2, e.PlayerCount);
                Assert.Equal(TimeSpan.FromMinutes((60 + 90 + 30 + 30) / 4.0), e.AveragePlayTime);
                Assert.Equal(4, e.SessionCount);
            },
            e =>
            {
                Assert.Equal(3, e.PlayerCount);
                Assert.Equal(TimeSpan.FromMinutes(120), e.AveragePlayTime);
                Assert.Equal(1, e.SessionCount);
            }
        );
    }

    [Fact]
    public void BoardGameCompetitiveResultsTest()
    {
        var actual = BoardGameStatisticsCalculator.Calculate(Sessions);

        Assert.Collection(
            actual.PlayerWinRates,
            e =>
            {
                Assert.Equal(Alice, e.PlayerId);
                Assert.Equal("Alice", e.PlayerName);
                Assert.Equal(4, e.PlayCount);
                Assert.Equal(2, e.WinCount);
                Assert.Equal(0.5, e.WinRate);
            },
            e =>
            {
                Assert.Equal(Bob, e.PlayerId);
                Assert.Equal(4, e.PlayCount);
                Assert.Equal(1, e.WinCount);
                Assert.Equal(0.25, e.WinRate);
            },
            e =>
            {
                Assert.Equal(Carol, e.PlayerId);
                Assert.Equal(1, e.PlayCount);
                Assert.Equal(0, e.WinRate);
            }
        );

        // Tied 80s: the earlier one; the solo 999 and co-op 100 are not competitive
        Assert.NotNull(actual.HighScore);
        Assert.Equal(Alice, actual.HighScore.PlayerId);
        Assert.Equal(80, actual.HighScore.Score);
        Assert.Equal(Day(0), actual.HighScore.PlayedAt);

        Assert.Equal((80 + 60 + 50 + 70 + 80 + 40) / 6.0, actual.AverageScore);
        Assert.Equal((80 + 70 + 80) / 3.0, actual.AverageWinningScore);
        // Alice started two sessions and won one of them
        Assert.Equal(0.5, actual.StartPlayerWinRate);
        Assert.Equal(0.5, actual.CooperativeWinRate);
    }

    [Fact]
    public void PlayerWithoutSessionsTest()
    {
        var actual = PlayerStatisticsCalculator.Calculate(Carol, []);

        Assert.Equal(0, actual.PlayCount);
        Assert.Null(actual.LastPlayedAt);
        Assert.Equal(TimeSpan.Zero, actual.TotalPlayTime);
        Assert.Equal(0, actual.CompetitivePlayCount);
        Assert.Equal(0, actual.WinCount);
        Assert.Null(actual.WinRate);
        Assert.Null(actual.MostPlayedGame);
        Assert.Empty(actual.PersonalBests);
    }

    [Fact]
    public void PlayerStatisticsTest()
    {
        List<SessionRecord> sessions =
        [
            .. Sessions,
            Session(Day(7), boardGameId: OtherGameId, scores: [Score(Bob, 30, true)]),
            Session(Day(8), boardGameId: OtherGameId, scores: [Score(Alice, 20)]),
        ];

        var actual = PlayerStatisticsCalculator.Calculate(Bob, sessions);

        Assert.Equal(8, actual.PlayCount);
        Assert.Equal(Day(7), actual.LastPlayedAt);
        Assert.Equal(TimeSpan.FromMinutes(60 + 90 + 120 + 30 + 30 + 45 + 60), actual.TotalPlayTime);
        Assert.Equal(5, actual.CompetitivePlayCount);
        Assert.Equal(2, actual.WinCount);
        Assert.Equal(0.4, actual.WinRate);
        Assert.NotNull(actual.MostPlayedGame);
        Assert.Equal(GameId, actual.MostPlayedGame.BoardGameId);
        Assert.Equal(7, actual.MostPlayedGame.PlayCount);
        Assert.Collection(
            actual.PersonalBests,
            e =>
            {
                Assert.Equal(GameId, e.BoardGameId);
                Assert.Equal("Game", e.BoardGameName);
                Assert.Equal(70, e.Score);
                Assert.Equal(Day(1), e.PlayedAt);
            },
            e =>
            {
                Assert.Equal(OtherGameId, e.BoardGameId);
                Assert.Equal(30, e.Score);
            }
        );
    }

    [Fact]
    public void MostPlayedGameTieTakesLastPlayedTest()
    {
        List<SessionRecord> sessions =
        [
            Session(Day(0), scores: [Score(Alice, 1)]),
            Session(Day(1), boardGameId: OtherGameId, scores: [Score(Alice, 1)]),
        ];

        var actual = PlayerStatisticsCalculator.Calculate(Alice, sessions);

        Assert.Equal(OtherGameId, actual.MostPlayedGame?.BoardGameId);
    }
}
