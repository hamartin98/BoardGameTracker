using BoardGameTracker.Api.ViewModels;
using BoardGameTracker.Models.Entities;
using Mapster;

namespace BoardGameTracker.Mappers;

public class ResponseMapperConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<AbstractEntity, ResponseMetaDataVM>();
    }
}
