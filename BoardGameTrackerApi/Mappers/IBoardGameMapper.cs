using BoardGameTracker.Api.ViewModels;
using BoardGameTracker.Api.ViewModels.Enums;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Enums;
using Mapster;

namespace BoardGameTracker.Mappers;

public class BoardGameMapperConfig : IRegister
{
    public static BoardGameCategory MapBoardGameCategory(List<BoardGameCategoryVM> types)
    {
        return types.Aggregate((BoardGameCategory)0, (a, b) => a | b.Adapt<BoardGameCategory>());
    }

    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<BoardGameCreateVM, BoardGame>()
            .Map(dest => dest.Categories, src => MapBoardGameCategory(src.Categories));
        config
            .NewConfig<BoardGame, BoardGameResponseVM>()
            .Map(dest => dest.MetaData, src => src.Adapt<ResponseMetaDataVM>())
            .Map(dest => dest.Expansions, src => src.Expansions);
    }
}

[Mapper]
public interface IBoardGameMapper
{
    BoardGame MapToEntity(BoardGameCreateVM boardGame);
    BoardGame MapToEntity(BoardGameUpdateVM boardGame);

    BoardGameResponseVM MapToVM(BoardGame boardGame);
}
