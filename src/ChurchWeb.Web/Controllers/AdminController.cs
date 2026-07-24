using ChurchWeb.Infrastructure.Data;
using ChurchWeb.Web.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChurchWeb.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<AdminController> _logger;

    public AdminController(AppDbContext context, ILogger<AdminController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var model = new DashboardViewModel
        {
            Statistics = new StatisticsViewModel
            {
                TotalSermons = await _context.Sermons.CountAsync(),
                TotalBulletins = await _context.Bulletins.CountAsync(),
                TotalAlbums = await _context.Albums.CountAsync(),
                TotalNotices = await _context.Notices.CountAsync(),
                TotalEvents = await _context.CalendarEvents.CountAsync(),
                TotalHeroSlides = await _context.HeroSlides.CountAsync()
            },
            RecentSermons = await _context.Sermons
                .OrderByDescending(s => s.PreachedOn)
                .Take(5)
                .Select(s => new RecentItemViewModel
                {
                    Id = s.Id,
                    Title = s.Title,
                    Date = s.PreachedOn,
                    Category = s.Category == "sunday" ? "주일설교" : "특별설교"
                })
                .ToListAsync(),
            RecentBulletins = await _context.Bulletins
                .OrderByDescending(b => b.PublishedDate)
                .Take(5)
                .Select(b => new RecentItemViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    Date = b.PublishedDate
                })
                .ToListAsync(),
            RecentNotices = await _context.Notices
                .OrderByDescending(n => n.PostedOn)
                .Take(5)
                .Select(n => new RecentItemViewModel
                {
                    Id = n.Id,
                    Title = n.Title,
                    Date = n.PostedOn,
                    Category = n.CategoryKey == "church" ? "교회소식" : "교우소식"
                })
                .ToListAsync()
        };

        return View(model);
    }
}
