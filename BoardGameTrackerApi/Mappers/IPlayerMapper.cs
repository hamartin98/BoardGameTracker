using BoardGameTracker.Api.ViewModels;
using BoardGameTracker.Models.Entities;
using Mapster;

namespace BoardGameTracker.Mappers;

public class PlayerMapperConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<PlayerCreateVM, Player>();
        config
            .NewConfig<Player, PlayerResponseVM>()
            .Map(dest => dest.MetaData, src => src.Adapt<ResponseMetaDataVM>());
    }
}

[Mapper]
public interface IPlayerMapper
{
    Player MapToEntity(PlayerCreateVM player);
    Player MapToEntity(PlayerUpdateVM player);

    PlayerResponseVM MapToVM(Player player);
}
