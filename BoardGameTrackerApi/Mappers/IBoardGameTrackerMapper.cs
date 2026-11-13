using BoardGameTracker.Api.ViewModels;
using BoardGameTracker.Models.Entities;
using Mapster;

namespace BoardGameTracker.Mappers;

[Mapper]
public interface IBoardGameTrackerMapper
{
    // Board game
    BoardGame MapToEntity(BoardGameCreateVM boardGame);
    BoardGame MapToEntity(BoardGameUpdateVM boardGame);

    BoardGameResponseVM MapToVM(BoardGame boardGame);

    // Player
    Player MapToEntity(PlayerCreateVM player);
    Player MapToEntity(PlayerUpdateVM player);

    PlayerResponseVM MapToVM(Player player);

    // PlayerScore
    PlayerScore MapToEntity(PlayerScoreCreateVM playerScore);
    PlayerScore MapToEntity(PlayerScoreUpdateVM playerScore);

    PlayerScoreResponseVM MapToVM(PlayerScore playerScore);

    // PlaySesion
    PlaySession MapToEntity(PlaySessionCreateVM playSession);
    PlaySession MapToEntity(PlaySessionUpdateVM playSession);

    PlaySessionResponseVM MapToVM(PlaySession playSession);
}
