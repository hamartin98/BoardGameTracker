using BoardGameTracker.Models.Statistics;
using BoardGameTracker.ViewModels;
using Mapster;

namespace BoardGameTracker.Mappers;

public class StatisticsMapperConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<BoardGameStatistics, BoardGameStatisticsVM>();
        config.NewConfig<PlayTimeByPlayerCount, PlayTimeByPlayerCountVM>();
        config.NewConfig<PlayerWinRate, PlayerWinRateVM>();
        config.NewConfig<HighScore, HighScoreVM>();
        config.NewConfig<PlayerStatistics, PlayerStatisticsVM>();
        config.NewConfig<MostPlayedGame, MostPlayedGameVM>();
        config.NewConfig<PersonalBest, PersonalBestVM>();
    }
}
