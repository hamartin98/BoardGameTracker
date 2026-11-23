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
            .Ignore(dest => dest.NormalizedName, dest => dest.Statistics);
        config.NewConfig<Player, PlayerResponseVM>().Map(dest => dest.MetaData, src => src);
        config.NewConfig<PlayerFilterVM, PlayerFilter>();
        config.NewConfig<PaginatedResponse<Player>, PlayersResponseVM>();
    }
}
