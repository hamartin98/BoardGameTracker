using BoardGameTracker.Api.ViewModels;
using BoardGameTracker.Api.ViewModels.Enums;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Enums;
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
    }
}
