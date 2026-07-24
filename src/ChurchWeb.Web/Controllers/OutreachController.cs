using ChurchWeb.Infrastructure.Data;
using ChurchWeb.Infrastructure.Services;
using ChurchWeb.Web.Models.Outreach;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChurchWeb.Web.Controllers;

public class OutreachController : Controller
{
    private readonly AppDbContext _context;
    private readonly IEvangelistAdminService _evangelistService;
    private readonly IChurchInfoService _churchInfoService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OutreachController> _logger;

    public OutreachController(
        AppDbContext context,
        IEvangelistAdminService evangelistService,
        IChurchInfoService churchInfoService,
        IConfiguration configuration,
        ILogger<OutreachController> logger)
    {
        _context = context;
        _evangelistService = evangelistService;
        _churchInfoService = churchInfoService;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet("/outreach/{id}")]
    public async Task<IActionResult> Card(int id)
    {
        // 전도자 정보 조회
        var evangelist = await _evangelistService.GetEvangelistByIdAsync(id);

        // 존재하지 않거나 비활성화된 전도자인 경우
        if (evangelist == null || !evangelist.IsActive)
        {
            return View("Invalid");
        }

        // 교회 정보 조회
        var churchInfo = await _churchInfoService.GetChurchInfoAsync();
        if (churchInfo == null)
        {
            _logger.LogError("ChurchInfo not found");
            return View("Error");
        }

        _logger.LogInformation($"ChurchInfo - Latitude: {churchInfo.Latitude}, Longitude: {churchInfo.Longitude}");

        // 주보 최신 3건
        var bulletins = await _context.Bulletins
            .Where(b => b.IsVisible)
            .OrderByDescending(b => b.PublishedDate)
            .Take(3)
            .Select(b => new BulletinItem
            {
                Id = b.Id,
                Title = b.Title,
                PublishedDate = b.PublishedDate
            })
            .ToListAsync();

        // 갤러리 최신 4건 (2x2)
        var albums = await _context.Albums
            .Where(a => a.IsVisible)
            .OrderByDescending(a => a.EventDate)
            .Take(4)
            .Select(a => new AlbumItem
            {
                Id = a.Id,
                Title = a.Title,
                CoverImageUrl = a.CoverImageUrl
            })
            .ToListAsync();

        // ViewModel 구성
        var model = new OutreachCardVm
        {
            EvangelistId = evangelist.Id,
            EvangelistName = evangelist.Name,
            EvangelistTitle = evangelist.Title,
            EvangelistPhone = evangelist.Phone,
            EvangelistPhotoUrl = evangelist.PhotoUrl,
            EvangelistGreeting = evangelist.Greeting,

            ChurchName = churchInfo.ChurchName,
            Denomination = churchInfo.Denomination,
            ChurchAddress = churchInfo.Address,
            ChurchPhone = churchInfo.Phone,
            HomepageUrl = churchInfo.HomepageUrl,
            Latitude = churchInfo.Latitude,
            Longitude = churchInfo.Longitude,

            PromoVideoUrl = churchInfo.PromoVideoUrl,
            Bulletins = bulletins,
            Albums = albums
        };

        // Kakao Map API 키 전달
        ViewBag.KakaoMapApiKey = _configuration["KakaoMap:ApiKey"] ?? "";

        return View(model);
    }
}
