using Microsoft.AspNetCore.Mvc;
using ChurchWeb.Application.Services;

namespace ChurchWeb.Web.Controllers;

public class NewsController : Controller
{
    private readonly INewsService _newsService;

    public NewsController(INewsService newsService)
    {
        _newsService = newsService;
    }

    // 주보 목록
    [Route("/news/bulletins")]
    public async Task<IActionResult> Bulletins(int page = 1)
    {
        var vm = await _newsService.GetBulletinListAsync(page, 12);
        return View(vm);
    }

    // 주보 상세
    [Route("/news/bulletins/{id:int}")]
    public async Task<IActionResult> BulletinDetail(int id)
    {
        var vm = await _newsService.GetBulletinDetailAsync(id);
        if (vm == null)
            return NotFound();

        return View(vm);
    }

    // 주보 PDF 다운로드
    [Route("/news/bulletins/{id:int}/download")]
    public async Task<IActionResult> DownloadBulletin(int id)
    {
        var pdfData = await _newsService.GetBulletinPdfAsync(id);
        if (pdfData == null)
            return NotFound();

        return File(pdfData.Value.Data, pdfData.Value.ContentType, pdfData.Value.FileName);
    }

    // 갤러리 목록
    [Route("/news/gallery")]
    public async Task<IActionResult> Gallery(int? year = null, int page = 1)
    {
        var vm = await _newsService.GetGalleryListAsync(year, page, 12);
        return View(vm);
    }

    // 갤러리(앨범) 상세
    [Route("/news/gallery/{id:int}")]
    public async Task<IActionResult> Album(int id)
    {
        var vm = await _newsService.GetAlbumDetailAsync(id);
        if (vm == null)
            return NotFound();

        return View(vm);
    }

    // 교회/교우소식 목록
    [Route("/news/notices")]
    public async Task<IActionResult> Notices(int page = 1)
    {
        var vm = await _newsService.GetNoticeListAsync(page, 20);
        return View(vm);
    }

    // 교회소식 상세
    [Route("/news/notices/{id:int}")]
    public async Task<IActionResult> NoticeDetail(int id)
    {
        var vm = await _newsService.GetNoticeDetailAsync(id);
        if (vm == null)
            return NotFound();

        return View(vm);
    }

    // 교회일정
    [Route("/news/calendar")]
    public async Task<IActionResult> Calendar(int? year = null, int? month = null)
    {
        var today = DateTime.Today;
        var targetYear = year ?? today.Year;
        var targetMonth = month ?? today.Month;

        var vm = await _newsService.GetCalendarAsync(targetYear, targetMonth);
        return View(vm);
    }
}
