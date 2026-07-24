using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChurchWeb.Infrastructure.Services;
using ChurchWeb.Web.Models.Admin;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace ChurchWeb.Web.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class OutreachAdminController : Controller
{
    private readonly IChurchInfoService _churchInfoService;
    private readonly ILogger<OutreachAdminController> _logger;

    public OutreachAdminController(
        IChurchInfoService churchInfoService,
        ILogger<OutreachAdminController> logger)
    {
        _churchInfoService = churchInfoService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var churchInfo = await _churchInfoService.GetChurchInfoAsync();

        var model = new OutreachCardViewModel
        {
            WelcomeMessage = churchInfo.OutreachWelcomeMessage,
            Phone = churchInfo.Phone,
            Address = churchInfo.Address,
            MapLink = churchInfo.OutreachMapLink,
            ShortUrl = churchInfo.OutreachShortUrl,
            ChurchName = churchInfo.ChurchName,
            WorshipTimes = churchInfo.WorshipTimesJson
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(OutreachCardViewModel model)
    {
        try
        {
            var churchInfo = await _churchInfoService.GetChurchInfoAsync();

            churchInfo.OutreachWelcomeMessage = model.WelcomeMessage ?? string.Empty;
            churchInfo.OutreachMapLink = model.MapLink ?? string.Empty;
            churchInfo.OutreachShortUrl = model.ShortUrl ?? string.Empty;

            await _churchInfoService.SaveChurchInfoAsync(churchInfo);

            TempData["SuccessMessage"] = "전도카드 정보가 저장되었습니다.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "전도카드 정보 저장 중 오류 발생");
            TempData["ErrorMessage"] = "저장 중 오류가 발생했습니다.";
            return View("Index", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> DownloadQR()
    {
        try
        {
            var churchInfo = await _churchInfoService.GetChurchInfoAsync();
            var url = string.IsNullOrEmpty(churchInfo.OutreachShortUrl)
                ? Request.Scheme + "://" + Request.Host
                : churchInfo.OutreachShortUrl;

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeBytes = qrCode.GetGraphic(20);

            return File(qrCodeBytes, "image/png", $"qrcode-{DateTime.Now:yyyyMMdd}.png");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "QR 코드 생성 중 오류 발생");
            TempData["ErrorMessage"] = "QR 코드 생성 중 오류가 발생했습니다.";
            return RedirectToAction(nameof(Index));
        }
    }
}
