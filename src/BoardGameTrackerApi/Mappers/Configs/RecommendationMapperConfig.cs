using BoardGameTracker.Models.Recommendations;
using BoardGameTracker.ViewModels;
using BoardGameTracker.ViewModels.Filters;
using Mapster;

namespace BoardGameTracker.Mappers;

public class RecommendationMapperConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<RecommendationFilterVM, RecommendationRequest>()
            .Map(dest => dest.PlayerCount, src => src.PlayerCount ?? 0)
            .Map(
                dest => dest.Categories,
                src => BoardGameMapperConfig.MapBoardGameCategory(src.Categories)
            );
        config.NewConfig<BoardGameRecommendation, BoardGameRecommendationVM>();
    }
}
