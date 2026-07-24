using ChurchWeb.Core.Entities.Common;
using ChurchWeb.Core.Entities.Home;
using ChurchWeb.Infrastructure.Data;
using ChurchWeb.Infrastructure.Services;
using ChurchWeb.Web.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ChurchWeb.Web.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("Admin/ChurchInfo")]
public class ChurchInfoAdminController : Controller
{
    private readonly IChurchInfoService _churchInfoService;
    private readonly ILogger<ChurchInfoAdminController> _logger;
    private readonly AppDbContext _context;

    public ChurchInfoAdminController(
        IChurchInfoService churchInfoService,
        ILogger<ChurchInfoAdminController> logger,
        AppDbContext context)
    {
        _churchInfoService = churchInfoService;
        _logger = logger;
        _context = context;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var churchInfo = await _churchInfoService.GetChurchInfoAsync();
        var vision = await _context.Visions
            .Include(v => v.Practices)
            .Where(v => v.IsVisible)
            .FirstOrDefaultAsync();

        ChurchInfoViewModel model;

        if (churchInfo == null)
        {
            // 초기 빈 폼 표시
            model = new ChurchInfoViewModel();
        }
        else
        {
            // 기존 데이터 로드
            model = new ChurchInfoViewModel
            {
                Id = churchInfo.Id,
                ChurchName = churchInfo.ChurchName,
                Denomination = churchInfo.Denomination,
                Address = churchInfo.Address,
                Phone = churchInfo.Phone,
                Email = churchInfo.Email,
                YoutubeUrl = churchInfo.YoutubeUrl,
                OnlineOfferingAccount = churchInfo.OnlineOfferingAccount,
                MapEmbed = churchInfo.MapEmbed,
                Latitude = churchInfo.Latitude,
                Longitude = churchInfo.Longitude,
                WorshipTimesJson = churchInfo.WorshipTimesJson,
                FooterText = churchInfo.FooterText,
                AnnualSlogan = churchInfo.AnnualSlogan,
                Practices = ConvertJsonToPractices(churchInfo.PracticesJson),
                PromoVideoUrl = churchInfo.PromoVideoUrl,
                OutreachCardImageUrl = churchInfo.OutreachCardImageUrl,
                OutreachCardPdfUrl = churchInfo.OutreachCardPdfUrl
            };
        }

        // Vision 데이터 로드
        if (vision != null)
        {
            model.Year = vision.Year;
            model.VisionMotto = vision.MottoText;
            model.VisionScripture = vision.ScriptureRef;

            // Practices를 Practices 필드에 합침
            if (vision.Practices != null && vision.Practices.Any())
            {
                model.Practices = string.Join(Environment.NewLine,
                    vision.Practices.OrderBy(p => p.SortOrder).Select(p => p.Text));
            }
        }

        return View(model);
    }

    [HttpPost("Save")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(ChurchInfoViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        try
        {
            _logger.LogInformation("=== ChurchInfo Save 시작 ===");
            _logger.LogInformation($"ChurchInfoService Type: {_churchInfoService.GetType().Name}");
            _logger.LogInformation($"ChurchName: {model.ChurchName}");
            _logger.LogInformation($"AnnualSlogan: {model.AnnualSlogan}");
            _logger.LogInformation($"PromoVideoUrl: {model.PromoVideoUrl}");

            var churchInfo = new ChurchInfo
            {
                Id = model.Id,
                ChurchName = model.ChurchName,
                Denomination = model.Denomination,
                Address = model.Address,
                Phone = model.Phone,
                Email = model.Email,
                YoutubeUrl = model.YoutubeUrl,
                OnlineOfferingAccount = model.OnlineOfferingAccount,
                MapEmbed = model.MapEmbed,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                WorshipTimesJson = model.WorshipTimesJson,
                FooterText = model.FooterText,
                AnnualSlogan = model.AnnualSlogan,
                PracticesJson = ConvertPracticesToJson(model.Practices),
                PromoVideoUrl = model.PromoVideoUrl,
                OutreachCardImageUrl = model.OutreachCardImageUrl,
                OutreachCardPdfUrl = model.OutreachCardPdfUrl
            };

            var savedInfo = await _churchInfoService.SaveChurchInfoAsync(churchInfo);

            _logger.LogInformation($"저장 완료 - ID: {savedInfo.Id}");
            _logger.LogInformation($"저장된 ChurchName: {savedInfo.ChurchName}");
            _logger.LogInformation($"저장된 AnnualSlogan: {savedInfo.AnnualSlogan}");

            // Vision 저장 (올해의 표어 섹션)
            await SaveVisionAsync(model);

            _logger.LogInformation("=== ChurchInfo 및 Vision Save 완료 ===");

            TempData["SuccessMessage"] = "교회 정보가 성공적으로 저장되었습니다.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving church info");
            ModelState.AddModelError(string.Empty, "저장 중 오류가 발생했습니다.");
            return View("Index", model);
        }
    }

    private string ConvertPracticesToJson(string practices)
    {
        if (string.IsNullOrWhiteSpace(practices))
            return "[]";

        var lines = practices
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();

        return JsonSerializer.Serialize(lines);
    }

    private string ConvertJsonToPractices(string practicesJson)
    {
        if (string.IsNullOrWhiteSpace(practicesJson))
            return string.Empty;

        try
        {
            var practices = JsonSerializer.Deserialize<List<string>>(practicesJson);
            return practices != null ? string.Join(Environment.NewLine, practices) : string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    private async Task SaveVisionAsync(ChurchInfoViewModel model)
    {
        // 기존 Vision 찾기 또는 새로 생성
        var vision = await _context.Visions
            .Include(v => v.Practices)
            .FirstOrDefaultAsync();

        if (vision == null)
        {
            // 새로 생성
            vision = new Vision
            {
                Year = model.Year,
                MottoText = model.VisionMotto,
                ScriptureRef = model.VisionScripture,
                IsVisible = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Visions.Add(vision);
            await _context.SaveChangesAsync();
        }
        else
        {
            // 기존 업데이트
            vision.Year = model.Year;
            vision.MottoText = model.VisionMotto;
            vision.ScriptureRef = model.VisionScripture;
            vision.UpdatedAt = DateTime.UtcNow;
        }

        // 기존 Practices 삭제
        if (vision.Practices != null && vision.Practices.Any())
        {
            _context.VisionPractices.RemoveRange(vision.Practices);
        }

        // 새 Practices 추가
        if (!string.IsNullOrWhiteSpace(model.Practices))
        {
            var lines = model.Practices
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToList();

            for (int i = 0; i < lines.Count; i++)
            {
                var practice = new VisionPractice
                {
                    VisionId = vision.Id,
                    Number = $"{i + 1:D2}",
                    Text = lines[i],
                    SortOrder = i + 1
                };
                _context.VisionPractices.Add(practice);
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation($"Vision 저장 완료 - Year: {model.Year}, Motto: {model.VisionMotto}");
    }
}
