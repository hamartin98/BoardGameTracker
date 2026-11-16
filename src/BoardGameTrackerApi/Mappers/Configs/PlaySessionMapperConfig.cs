using BoardGameTracker.Api.ViewModels;
using BoardGameTracker.Models.Entities;
using Mapster;

namespace BoardGameTracker.Mappers;

public class PlaySessionMapperConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<PlaySessionCreateVM, PlaySession>();
        config
            .NewConfig<PlaySession, PlaySessionResponseVM>()
            .Map(dest => dest.MetaData, src => src.Adapt<ResponseMetaDataVM>());
    }
}
