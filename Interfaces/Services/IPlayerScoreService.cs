using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;

namespace BoardGameTracker.Interfaces.Services;

public interface IPlayerScoreService : ICrudService<PlayerScore, PlayerScoreFilter> { }
