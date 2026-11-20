using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;
using BoardGameTracker.ViewModels;
using BoardGameTracker.ViewModels.Filters;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameTrackerApi.Controllers;

[Route("[controller]")]
public class PlayersController(IMapper mapper, IPlayerService playerService)
    : CrudController<
        Player,
        PlayerFilter,
        PlayerCreateVM,
        PlayerUpdateVM,
        PlayerResponseVM,
        PlayersResponseVM,
        PlayerFilterVM
    >(mapper, playerService);
