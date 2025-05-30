using Microsoft.AspNetCore.Mvc;


namespace CircleApp.UI.Controllers;

using Base;
using Infrastructure.Persistence.Constants;
using Microsoft.AspNetCore.Authorization;
using Services.Interfaces;


[Authorize(Roles = AppRoles.User)]
public class SearchController : BaseController {

    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    // GET
    public async Task<IActionResult> Index(string searchString)
    {
        int userId = GetUserId();
        var response = await _searchService.Search(searchString, userId);

        return View(response);
    }

}
