using BoardGameTracker.Api.ViewModels;
using BoardGameTracker.Models.Entities;
using Mapster;

namespace BoardGameTracker.Mappers;

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

[Mapper]
public interface IPlayerScoreMapper
{
    PlayerScore MapToEntity(PlayerScoreCreateVM playerScore);
    PlayerScore MapToEntity(PlayerScoreUpdateVM playerScore);

    PlayerScoreResponseVM MapToVM(PlayerScore playerScore);
}
