using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;
using BoardGameTracker.ViewModels;
using BoardGameTracker.ViewModels.Filters;
using Mapster;

namespace BoardGameTracker.Mappers;

public class PlaySessionMapperConfig : IRegister
{
    private static TypeAdapterSetter<TSource, PlaySession> MapRequestFields<TSource>(
        TypeAdapterSetter<TSource, PlaySession> setter
    )
        where TSource : PlaySessionBaseVM
    {
        return setter.IgnoreEntityMetadata().Ignore(dest => dest.BoardGame);
    }

    public void Register(TypeAdapterConfig config)
    {
        MapRequestFields(config.NewConfig<PlaySessionCreateVM, PlaySession>());
        MapRequestFields(config.NewConfig<PlaySessionUpdateVM, PlaySession>());
        config
            .NewConfig<PlaySession, PlaySessionResponseVM>()
            .Map(dest => dest.MetaData, src => src)
            .Map(
                dest => dest.UsedExpansionIds,
                src => src.UsedExpansions.Select(e => e.Id).ToList()
            )
            .Map(dest => dest.PlayerScoreIds, src => src.PlayerScores.Select(s => s.Id).ToList());
        config.NewConfig<PlaySessionFilterVM, PlaySessionFilter>();
        config.NewConfig<PaginatedResponse<PlaySession>, PlaySessionsResponseVM>();
    }
}
