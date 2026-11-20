using BoardGameTracker.Models.Entities;
using BoardGameTracker.ViewModels;
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
            .NewConfig<PlayerScore, PlayerScoreResponseVM>()
            .Map(dest => dest.MetaData, src => src.Adapt<ResponseMetaDataVM>());
    }
}
