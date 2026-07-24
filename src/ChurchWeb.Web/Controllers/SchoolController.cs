using Microsoft.AspNetCore.Mvc;

namespace ChurchWeb.Web.Controllers;

public class SchoolController : Controller
{
    // 유아부·유치부
    public IActionResult Kids()
    {
        return View();
    }

    // 초등부·청소년부
    public IActionResult Youth()
    {
        return View();
    }
}
