using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;
using BoardGameTracker.ViewModels;
using BoardGameTracker.ViewModels.Filters;
using Mapster;

namespace BoardGameTracker.Mappers;

public class PlayerScoreMapperConfig : IRegister
{
    private static TypeAdapterSetter<TSource, PlayerScore> MapRequestFields<TSource>(
        TypeAdapterSetter<TSource, PlayerScore> setter
    )
        where TSource : PlayerScoreBaseVM
    {
        // The get-only CategoryScores collection is filled after mapping
        return setter
            .IgnoreEntityMetadata()
            .Ignore(dest => dest.Score, dest => dest.CategoryScores)
            .AfterMapping(
                (src, dest) =>
                {
                    foreach (CategoryScoreVM categoryScore in src.CategoryScores)
                    {
                        dest.CategoryScores.Add(
                            new()
                            {
                                ScoringCategoryId = categoryScore.ScoringCategoryId,
                                Value = categoryScore.Value,
                            }
                        );
                    }
                }
            );
    }

    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CategoryScore, CategoryScoreVM>();
        MapRequestFields(config.NewConfig<PlayerScoreCreateVM, PlayerScore>())
            .Ignore(dest => dest.Player, dest => dest.Session);
        MapRequestFields(config.NewConfig<PlayerScoreUpdateVM, PlayerScore>())
            .Ignore(
                dest => dest.Player,
                dest => dest.Session,
                dest => dest.PlayerId,
                dest => dest.SessionId
            );
        config
            .NewConfig<PlayerScore, PlayerScoreResponseVM>()
            .Map(dest => dest.MetaData, src => src);
        config.NewConfig<PlayerScoreFilterVM, PlayerScoreFilter>();
        config.NewConfig<PaginatedResponse<PlayerScore>, PlayerScoresResponseVM>();
    }
}
