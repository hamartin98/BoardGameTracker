using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Enums;
using BoardGameTracker.Models.Filters;
using BoardGameTracker.ViewModels;
using BoardGameTracker.ViewModels.Enums;
using BoardGameTracker.ViewModels.Filters;
using Mapster;

namespace BoardGameTracker.Mappers;

public class BoardGameMapperConfig : IRegister
{
    public static BoardGameCategory MapBoardGameCategory(List<BoardGameCategoryVM> categories)
    {
        return categories.Aggregate((BoardGameCategory)0, (a, b) => a | (BoardGameCategory)b);
    }

    public static List<BoardGameCategoryVM> MapBoardGameCategories(BoardGameCategory category)
    {
        List<BoardGameCategoryVM> result = [];
        foreach (BoardGameCategoryVM item in Enum.GetValues(typeof(BoardGameCategoryVM)))
        {
            if ((category & (BoardGameCategory)item) != 0)
                result.Add(item);
        }
        return result;
    }

    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<BoardGameCreateVM, BoardGame>()
            .Map(dest => dest.Categories, src => MapBoardGameCategory(src.Categories));
        config
            .NewConfig<BoardGame, BoardGameResponseVM>()
            .Map(dest => dest.MetaData, src => src)
            .Map(dest => dest.Categories, src => MapBoardGameCategories(src.Categories));
        config
            .NewConfig<BoardGameFilterVM, BoardGameFilter>()
            .Map(dest => dest.Types, src => src.Type);
        config
            .NewConfig<PaginatedResponse<BoardGame>, BoardGamesResponseVM>()
            .Map(dest => dest.Items, src => src);
    }
}
