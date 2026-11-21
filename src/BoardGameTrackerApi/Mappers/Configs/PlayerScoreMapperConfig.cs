using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;
using BoardGameTracker.ViewModels;
using BoardGameTracker.ViewModels.Filters;
using Mapster;

namespace BoardGameTracker.Mappers;

public class PlayerScoreMapperConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<PlayerScoreCreateVM, PlayerScore>()
            .IgnoreEntityMetadata()
            .Ignore(dest => dest.Player, dest => dest.Session);
        config
            .NewConfig<PlayerScoreUpdateVM, PlayerScore>()
            .IgnoreEntityMetadata()
            .Ignore(
                dest => dest.Player,
                dest => dest.Session,
                dest => dest.PlayerId,
                dest => dest.SessionId
            );
        config
            .NewConfig<PlayerScore, PlayerScoreResponseVM>()
            .Map(dest => dest.MetaData, src => src);
        config.NewConfig<PlayerScoreFilterVM, PlayerScoreFilter>();
        config.NewConfig<PaginatedResponse<PlayerScore>, PlayerScoresResponseVM>();
    }
}
