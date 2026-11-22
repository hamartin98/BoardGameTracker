using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;
using BoardGameTracker.ViewModels;
using BoardGameTracker.ViewModels.Filters;
using Mapster;

namespace BoardGameTracker.Mappers;

public class ScoringCategoryMapperConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<ScoringCategoryCreateVM, ScoringCategory>()
            .IgnoreEntityMetadata()
            .Ignore(
                dest => dest.BoardGame,
                dest => dest.NormalizedName,
                dest => dest.Order,
                dest => dest.CategoryScores
            );
        config
            .NewConfig<ScoringCategoryUpdateVM, ScoringCategory>()
            .IgnoreEntityMetadata()
            .Ignore(
                dest => dest.BoardGameId,
                dest => dest.BoardGame,
                dest => dest.NormalizedName,
                dest => dest.Order,
                dest => dest.CategoryScores
            );
        config
            .NewConfig<ScoringCategory, ScoringCategoryResponseVM>()
            .Map(dest => dest.MetaData, src => src);
        config.NewConfig<ScoringCategoryFilterVM, ScoringCategoryFilter>();
        config.NewConfig<PaginatedResponse<ScoringCategory>, ScoringCategoriesResponseVM>();
    }
}
