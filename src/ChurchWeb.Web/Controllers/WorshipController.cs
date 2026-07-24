using Microsoft.AspNetCore.Mvc;

namespace ChurchWeb.Web.Controllers;

public class WorshipController : Controller
{
    // 주일 설교
    public IActionResult Sermons()
    {
        return View();
    }

    // 영상 모음
    public IActionResult Media()
    {
        return View();
    }
}
