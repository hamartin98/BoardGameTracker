using BoardGameTracker.Models.Entities;
using BoardGameTracker.ViewModels;
using Mapster;

namespace BoardGameTracker.Mappers;

public class PlaySessionMapperConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<PlaySessionCreateVM, PlaySession>()
            .IgnoreEntityMetadata()
            .Ignore(dest => dest.BoardGame);
        config
            .NewConfig<PlaySession, PlaySessionResponseVM>()
            .Map(dest => dest.MetaData, src => src)
            .Map(
                dest => dest.UsedExpansionIds,
                src => src.UsedExpansions.Select(e => e.Id).ToList()
            )
            .Map(dest => dest.PlayerScoreIds, src => src.PlayerScores.Select(s => s.Id).ToList());
    }
}
