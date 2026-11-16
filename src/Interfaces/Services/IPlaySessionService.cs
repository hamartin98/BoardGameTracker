using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;

namespace BoardGameTracker.Interfaces.Services;

public interface IPlaySessionService : ICrudService<PlaySession, PlaySessionFilter> { }
