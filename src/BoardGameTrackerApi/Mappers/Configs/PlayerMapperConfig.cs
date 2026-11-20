using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;
using BoardGameTracker.ViewModels;
using BoardGameTracker.ViewModels.Filters;
using Mapster;

namespace BoardGameTracker.Mappers;

public class PlayerMapperConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<PlayerCreateVM, Player>()
            .IgnoreEntityMetadata()
            .Ignore(dest => dest.NormalizedName);
        config
            .NewConfig<Player, PlayerResponseVM>()
            .Map(dest => dest.MetaData, src => src.Adapt<ResponseMetaDataVM>());
        config.NewConfig<PlayerFilterVM, PlayerFilter>();
        config
            .NewConfig<PaginatedResponse<Player>, PlayersResponseVM>()
            .Map(dest => dest.Items, src => src);
    }
}
