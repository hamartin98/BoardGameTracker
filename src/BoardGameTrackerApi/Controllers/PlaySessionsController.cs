using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;
using BoardGameTracker.ViewModels;
using BoardGameTracker.ViewModels.Filters;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameTrackerApi.Controllers;

[Route("[controller]")]
public class PlaySessionsController(IMapper mapper, IPlaySessionService playSessionService)
    : CrudController<
        PlaySession,
        PlaySessionFilter,
        PlaySessionCreateVM,
        PlaySessionUpdateVM,
        PlaySessionResponseVM,
        PlaySessionsResponseVM,
        PlaySessionFilterVM
    >(mapper, playSessionService);
