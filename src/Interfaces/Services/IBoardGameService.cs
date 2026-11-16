using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;

namespace BoardGameTracker.Interfaces.Services;

public interface IBoardGameService : ICrudService<BoardGame, BoardGameFilter> { }
