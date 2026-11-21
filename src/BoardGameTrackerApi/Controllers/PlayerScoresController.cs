using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;
using BoardGameTracker.ViewModels;
using BoardGameTracker.ViewModels.Filters;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameTrackerApi.Controllers;

[Route("[controller]")]
public class PlayerScoresController(IMapper mapper, IPlayerScoreService playerScoreService)
    : CrudController<
        PlayerScore,
        PlayerScoreFilter,
        PlayerScoreCreateVM,
        PlayerScoreUpdateVM,
        PlayerScoreResponseVM,
        PlayerScoresResponseVM,
        PlayerScoreFilterVM
    >(mapper, playerScoreService);
