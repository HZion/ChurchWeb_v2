using Microsoft.AspNetCore.Mvc;
using ChurchWeb.Application.Services;

namespace ChurchWeb.Web.Controllers;

public class AboutController : Controller
{
    private readonly IAboutService _aboutService;
    private readonly IConfiguration _configuration;

    public AboutController(IAboutService aboutService, IConfiguration configuration)
    {
        _aboutService = aboutService;
        _configuration = configuration;
    }

    // 사명과 비전
    public async Task<IActionResult> Vision()
    {
        var model = await _aboutService.GetVisionViewModelAsync();
        return View(model);
    }

    // 예배 안내
    public async Task<IActionResult> Worship()
    {
        var model = await _aboutService.GetWorshipViewModelAsync();
        return View(model);
    }

    // 섬기는 사람들
    public async Task<IActionResult> People()
    {
        var model = await _aboutService.GetPeopleViewModelAsync();
        return View(model);
    }

    // 오시는 길
    public async Task<IActionResult> Location()
    {
        var model = await _aboutService.GetLocationViewModelAsync();
        ViewBag.KakaoMapApiKey = _configuration["KakaoMap:ApiKey"] ?? "";
        return View(model);
    }

    // 교회 역사 (이번 범위 외)
    public IActionResult History()
    {
        return View();
    }
}
