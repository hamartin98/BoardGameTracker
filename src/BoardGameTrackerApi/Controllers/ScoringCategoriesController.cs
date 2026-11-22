using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;
using BoardGameTracker.ViewModels;
using BoardGameTracker.ViewModels.Filters;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameTrackerApi.Controllers;

[Route("[controller]")]
public class ScoringCategoriesController(
    IMapper mapper,
    IScoringCategoryService scoringCategoryService
)
    : CrudController<
        ScoringCategory,
        ScoringCategoryFilter,
        ScoringCategoryCreateVM,
        ScoringCategoryUpdateVM,
        ScoringCategoryResponseVM,
        ScoringCategoriesResponseVM,
        ScoringCategoryFilterVM
    >(mapper, scoringCategoryService);
