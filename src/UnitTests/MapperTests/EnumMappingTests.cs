using BoardGameTracker.ViewModels.Enums;
using BoardGameTracker.Mappers;
using BoardGameTracker.Models.Enums;

namespace BoardGameTracker.UnitTests.MapperTests;

public partial class MappingTests
{
    [Theory]
    [InlineData(BoardGameTypeVM.BASE_GAME, BoardGameType.BASE_GAME)]
    [InlineData(BoardGameTypeVM.EXPANSION, BoardGameType.EXPANSION)]
    public void BoardGameTypeMappingTest(BoardGameTypeVM vmEnum, BoardGameType modelEnum)
    {
        // VM -> Model
        Assert.Equal(modelEnum, Mapper.Map<BoardGameType>(vmEnum));

        // Model -> VM
        Assert.Equal(vmEnum, Mapper.Map<BoardGameTypeVM>(modelEnum));
    }

    [Theory]
    [InlineData(OrderByDirectionVM.ASC, OrderByDirection.ASC)]
    [InlineData(OrderByDirectionVM.DESC, OrderByDirection.DESC)]
    public void OrderByDirectionMappingTest(OrderByDirectionVM vmEnum, OrderByDirection modelEnum)
    {
        // VM -> Model
        Assert.Equal(modelEnum, Mapper.Map<OrderByDirection>(vmEnum));

        // Model -> VM
        Assert.Equal(vmEnum, Mapper.Map<OrderByDirectionVM>(modelEnum));
    }

    [Theory]
    [InlineData(PlaySessionTypeVM.COMPETITIVE, PlaySessionType.COMPETITIVE)]
    [InlineData(PlaySessionTypeVM.COOPERATIVE, PlaySessionType.COOPERATIVE)]
    [InlineData(PlaySessionTypeVM.SINGLE_PLAYER, PlaySessionType.SINGLE_PLAYER)]
    public void PlaySessionTypeMappingTest(PlaySessionTypeVM vmEnum, PlaySessionType modelEnum)
    {
        // VM -> Model
        Assert.Equal(modelEnum, Mapper.Map<PlaySessionType>(vmEnum));

        // Model -> VM
        Assert.Equal(vmEnum, Mapper.Map<PlaySessionTypeVM>(modelEnum));
    }

    [Theory]
    [InlineData(BoardGameCategoryVM.ABSTRACT, BoardGameCategory.ABSTRACT)]
    [InlineData(BoardGameCategoryVM.ANIMALS, BoardGameCategory.ANIMALS)]
    [InlineData(BoardGameCategoryVM.AVIATION, BoardGameCategory.AVIATION)]
    [InlineData(BoardGameCategoryVM.BOOK, BoardGameCategory.BOOK)]
    [InlineData(BoardGameCategoryVM.CARD_GAME, BoardGameCategory.CARD_GAME)]
    [InlineData(BoardGameCategoryVM.CHILDRENS_GAME, BoardGameCategory.CHILDRENS_GAME)]
    [InlineData(BoardGameCategoryVM.CITY_BUILDING, BoardGameCategory.CITY_BUILDING)]
    [InlineData(BoardGameCategoryVM.CIVILIZATION, BoardGameCategory.CIVILIZATION)]
    [InlineData(BoardGameCategoryVM.DEDUCTION, BoardGameCategory.DEDUCTION)]
    [InlineData(BoardGameCategoryVM.DEXTERITY, BoardGameCategory.DEXTERITY)]
    [InlineData(BoardGameCategoryVM.DICE, BoardGameCategory.DICE)]
    [InlineData(BoardGameCategoryVM.ECONOMIC, BoardGameCategory.ECONOMIC)]
    [InlineData(BoardGameCategoryVM.EXPLORATION, BoardGameCategory.EXPLORATION)]
    [InlineData(BoardGameCategoryVM.FANTASY, BoardGameCategory.FANTASY)]
    [InlineData(BoardGameCategoryVM.FARMING, BoardGameCategory.FARMING)]
    [InlineData(BoardGameCategoryVM.FIGHTING, BoardGameCategory.FIGHTING)]
    [InlineData(BoardGameCategoryVM.HORROR, BoardGameCategory.HORROR)]
    [InlineData(BoardGameCategoryVM.HUMOR, BoardGameCategory.HUMOR)]
    [InlineData(
        BoardGameCategoryVM.INDUSTRY_MANUFACTURING,
        BoardGameCategory.INDUSTRY_MANUFACTURING
    )]
    [InlineData(BoardGameCategoryVM.MAFIA, BoardGameCategory.MAFIA)]
    [InlineData(BoardGameCategoryVM.MAZE, BoardGameCategory.MAZE)]
    [InlineData(BoardGameCategoryVM.MEDIEVAL, BoardGameCategory.MEDIEVAL)]
    [InlineData(BoardGameCategoryVM.MINIATURES, BoardGameCategory.MINIATURES)]
    [InlineData(BoardGameCategoryVM.MOVIES_TV, BoardGameCategory.MOVIES_TV)]
    [InlineData(BoardGameCategoryVM.MURDER_MISTERY, BoardGameCategory.MURDER_MISTERY)]
    [InlineData(BoardGameCategoryVM.MUSIC, BoardGameCategory.MUSIC)]
    [InlineData(BoardGameCategoryVM.MYTHOLOGY, BoardGameCategory.MYTHOLOGY)]
    [InlineData(BoardGameCategoryVM.NAUTICAL, BoardGameCategory.NAUTICAL)]
    [InlineData(BoardGameCategoryVM.NEGOTIATION, BoardGameCategory.NEGOTIATION)]
    [InlineData(BoardGameCategoryVM.NOVEL_BASED, BoardGameCategory.NOVEL_BASED)]
    [InlineData(BoardGameCategoryVM.PARTY, BoardGameCategory.PARTY)]
    [InlineData(BoardGameCategoryVM.PIRATES, BoardGameCategory.PIRATES)]
    [InlineData(BoardGameCategoryVM.PREHISTORIC, BoardGameCategory.PREHISTORIC)]
    [InlineData(BoardGameCategoryVM.PRINT_AND_PLAY, BoardGameCategory.PRINT_AND_PLAY)]
    [InlineData(BoardGameCategoryVM.PUZZLE, BoardGameCategory.PUZZLE)]
    [InlineData(BoardGameCategoryVM.RACING, BoardGameCategory.RACING)]
    [InlineData(BoardGameCategoryVM.REAL_TIME, BoardGameCategory.REAL_TIME)]
    [InlineData(BoardGameCategoryVM.SCIENCE_FICTION, BoardGameCategory.SCIENCE_FICTION)]
    [InlineData(BoardGameCategoryVM.SPACE_EXPLORATION, BoardGameCategory.SPACE_EXPLORATION)]
    [InlineData(BoardGameCategoryVM.SPIES_SECRET_AGENTS, BoardGameCategory.SPIES_SECRET_AGENTS)]
    [InlineData(BoardGameCategoryVM.SPORTS, BoardGameCategory.SPORTS)]
    [InlineData(BoardGameCategoryVM.TRAINS, BoardGameCategory.TRAINS)]
    [InlineData(BoardGameCategoryVM.TRANSPORTATION, BoardGameCategory.TRANSPORTATION)]
    [InlineData(BoardGameCategoryVM.TRAVEL, BoardGameCategory.TRAVEL)]
    [InlineData(BoardGameCategoryVM.TRIVIA, BoardGameCategory.TRIVIA)]
    [InlineData(BoardGameCategoryVM.VIDEO_GAME_THEME, BoardGameCategory.VIDEO_GAME_THEME)]
    [InlineData(BoardGameCategoryVM.WARGAME, BoardGameCategory.WARGAME)]
    [InlineData(BoardGameCategoryVM.WORD_GAME, BoardGameCategory.WORD_GAME)]
    [InlineData(BoardGameCategoryVM.WORLD_WAR_I, BoardGameCategory.WORLD_WAR_I)]
    [InlineData(BoardGameCategoryVM.WORLD_WAR_II, BoardGameCategory.WORLD_WAR_II)]
    [InlineData(BoardGameCategoryVM.ZOMBIES, BoardGameCategory.ZOMBIES)]
    public void BoardGameCategoryMappingTest(
        BoardGameCategoryVM vmEnum,
        BoardGameCategory modelEnum
    )
    {
        // VM -> Model
        Assert.Equal(modelEnum, Mapper.Map<BoardGameCategory>(vmEnum));

        // Model -> VM
        Assert.Equal(vmEnum, Mapper.Map<BoardGameCategoryVM>(modelEnum));
    }

    public class CategoryListTestData : TheoryData<List<BoardGameCategoryVM>, BoardGameCategory>
    {
        public CategoryListTestData()
        {
            Add([], BoardGameCategory.NONE);
            Add([BoardGameCategoryVM.ECONOMIC], BoardGameCategory.ECONOMIC);
            Add(
                [BoardGameCategoryVM.CIVILIZATION, BoardGameCategoryVM.ZOMBIES],
                BoardGameCategory.CIVILIZATION | BoardGameCategory.ZOMBIES
            );
        }
    }

    [Theory]
    [ClassData(typeof(CategoryListTestData))]
    public void BoardGameCateogryListToFlagsMappingTest(
        List<BoardGameCategoryVM> boardGameCategoryVMs,
        BoardGameCategory boardGameCategory
    )
    {
        Assert.Equal(
            boardGameCategory,
            BoardGameMapperConfig.MapBoardGameCategory(boardGameCategoryVMs)
        );

        Assert.Equal(
            boardGameCategoryVMs,
            BoardGameMapperConfig.MapBoardGameCategories(boardGameCategory)
        );
    }
}
