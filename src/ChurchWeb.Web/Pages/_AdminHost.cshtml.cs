using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChurchWeb.Web.Pages;

[Authorize(Roles = "Admin")]
public class AdminHostModel : PageModel
{
    public IActionResult OnGet()
    {
        // 인증된 경우에만 페이지를 표시
        return Page();
    }
}
