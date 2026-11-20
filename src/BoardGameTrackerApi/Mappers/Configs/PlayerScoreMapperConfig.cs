using BoardGameTracker.Models.Entities;
using BoardGameTracker.ViewModels;
using Mapster;

namespace BoardGameTracker.Mappers.Configs;

public class PlayerScoreMapperConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<PlayerScoreCreateVM, PlayerScore>();
        config
            .NewConfig<PlayerScore, PlayerScoreResponseVM>()
            .Map(dest => dest.MetaData, src => src.Adapt<ResponseMetaDataVM>());
    }
}
