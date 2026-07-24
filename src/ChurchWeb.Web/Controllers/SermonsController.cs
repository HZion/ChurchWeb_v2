using ChurchWeb.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChurchWeb.Web.Controllers;

public class SermonsController : Controller
{
    private readonly ISermonsService _sermonsService;

    public SermonsController(ISermonsService sermonsService)
    {
        _sermonsService = sermonsService;
    }

    [Route("/sermons/sunday")]
    public async Task<IActionResult> Sunday(int page = 1, string? search = null)
    {
        var model = await _sermonsService.GetSermonListAsync("sunday", page, 12, search);
        return View("List", model);
    }

    [Route("/sermons/special")]
    public async Task<IActionResult> Special(int page = 1, string? search = null)
    {
        var model = await _sermonsService.GetSermonListAsync("special", page, 12, search);
        return View("List", model);
    }

    [Route("/sermons/{id:int}")]
    public async Task<IActionResult> Detail(int id)
    {
        var model = await _sermonsService.GetSermonDetailAsync(id);
        if (model == null)
            return NotFound();

        return View(model);
    }
}
