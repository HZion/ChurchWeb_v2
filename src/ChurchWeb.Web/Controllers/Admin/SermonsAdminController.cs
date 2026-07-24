using ChurchWeb.Core.Entities.Sermons;
using ChurchWeb.Infrastructure.Services;
using ChurchWeb.Web.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChurchWeb.Web.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("Admin/Sermons")]
public class SermonsAdminController : Controller
{
    private readonly ISermonAdminService _sermonService;
    private readonly IYouTubeService _youtubeService;
    private readonly ILogger<SermonsAdminController> _logger;

    public SermonsAdminController(
        ISermonAdminService sermonService,
        IYouTubeService youtubeService,
        ILogger<SermonsAdminController> logger)
    {
        _sermonService = sermonService;
        _youtubeService = youtubeService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(
        int page = 1,
        string? searchTerm = null,
        string? category = null,
        bool? isVisible = null,
        string sortBy = "PreachedOn",
        bool sortDescending = true)
    {
        var (sermons, totalCount) = await _sermonService.GetPagedSermonsAsync(
            page, 20, searchTerm, category, isVisible, sortBy, sortDescending);

        var model = new SermonListViewModel
        {
            Sermons = sermons.Select(s => new SermonItemViewModel
            {
                Id = s.Id,
                Title = s.Title,
                Verse = s.Verse,
                Preacher = s.Preacher,
                PreachedOn = s.PreachedOn,
                Category = s.Category,
                IsVisible = s.IsVisible,
                SortOrder = s.SortOrder,
                YoutubeUrl = s.YoutubeUrl
            }).ToList(),
            Pagination = new PaginationViewModel
            {
                CurrentPage = page,
                PageSize = 20,
                TotalCount = totalCount
            },
            Filter = new SermonFilterViewModel
            {
                SearchTerm = searchTerm,
                Category = category,
                IsVisible = isVisible,
                SortBy = sortBy,
                SortDescending = sortDescending
            }
        };

        return View(model);
    }

    [HttpGet("Create")]
    public IActionResult Create()
    {
        var model = new SermonFormViewModel();
        return View("Form", model);
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var sermon = await _sermonService.GetSermonByIdAsync(id);
        if (sermon == null)
            return NotFound();

        var model = new SermonFormViewModel
        {
            Id = sermon.Id,
            Title = sermon.Title,
            Verse = sermon.Verse,
            Preacher = sermon.Preacher,
            PreachedOn = sermon.PreachedOn,
            Category = sermon.Category,
            YoutubeUrl = sermon.YoutubeUrl,
            VideoDescription = sermon.VideoDescription,
            Summary = sermon.Summary,
            Duration = sermon.Duration,
            ThumbnailUrl = sermon.ThumbnailUrl,
            IsVisible = sermon.IsVisible,
            SortOrder = sermon.SortOrder
        };

        return View("Form", model);
    }

    [HttpPost("Save")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(SermonFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Form", model);
        }

        try
        {
            if (model.Id == 0)
            {
                // Create
                var sermon = new Sermon
                {
                    Title = model.Title,
                    Verse = model.Verse,
                    Preacher = model.Preacher,
                    PreachedOn = model.PreachedOn,
                    Category = model.Category,
                    YoutubeUrl = model.YoutubeUrl,
                    VideoDescription = model.VideoDescription ?? string.Empty,
                    Summary = model.Summary,
                    Duration = model.Duration,
                    ThumbnailUrl = model.ThumbnailUrl,
                    IsVisible = model.IsVisible,
                    SortOrder = model.SortOrder
                };

                await _sermonService.CreateSermonAsync(sermon);
                TempData["SuccessMessage"] = "설교가 성공적으로 등록되었습니다.";
            }
            else
            {
                // Update
                var sermon = await _sermonService.GetSermonByIdAsync(model.Id);
                if (sermon == null)
                    return NotFound();

                sermon.Title = model.Title;
                sermon.Verse = model.Verse;
                sermon.Preacher = model.Preacher;
                sermon.PreachedOn = model.PreachedOn;
                sermon.Category = model.Category;
                sermon.YoutubeUrl = model.YoutubeUrl;
                sermon.VideoDescription = model.VideoDescription ?? string.Empty;
                sermon.Summary = model.Summary;
                sermon.Duration = model.Duration;
                sermon.ThumbnailUrl = model.ThumbnailUrl;
                sermon.IsVisible = model.IsVisible;
                sermon.SortOrder = model.SortOrder;

                await _sermonService.UpdateSermonAsync(sermon);
                TempData["SuccessMessage"] = "설교가 성공적으로 수정되었습니다.";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving sermon");
            ModelState.AddModelError(string.Empty, "저장 중 오류가 발생했습니다.");
            return View("Form", model);
        }
    }

    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _sermonService.DeleteSermonAsync(id);
        if (result)
        {
            return Json(new { success = true, message = "설교가 삭제되었습니다." });
        }

        return Json(new { success = false, message = "설교를 찾을 수 없습니다." });
    }

    [HttpPost("ToggleVisibility/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleVisibility(int id)
    {
        var result = await _sermonService.ToggleVisibilityAsync(id);
        if (result)
        {
            return Json(new { success = true, message = "표시 여부가 변경되었습니다." });
        }

        return Json(new { success = false, message = "설교를 찾을 수 없습니다." });
    }

    [HttpPost("UpdateSortOrder/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateSortOrder(int id, [FromBody] int newSortOrder)
    {
        var result = await _sermonService.UpdateSortOrderAsync(id, newSortOrder);
        if (result)
        {
            return Json(new { success = true, message = "순서가 변경되었습니다." });
        }

        return Json(new { success = false, message = "순서 변경에 실패했습니다." });
    }

    [HttpGet("FetchYouTubeMetadata")]
    public async Task<IActionResult> FetchYouTubeMetadata(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return Json(new { success = false, message = "URL을 입력하세요." });
        }

        try
        {
            var videoId = _youtubeService.ExtractVideoId(url);
            if (string.IsNullOrEmpty(videoId))
            {
                return Json(new { success = false, message = "올바른 YouTube URL이 아닙니다." });
            }

            var metadata = await _youtubeService.GetVideoMetadataAsync(videoId);
            if (metadata == null)
            {
                return Json(new { success = false, message = "YouTube 메타데이터를 가져올 수 없습니다. API 키를 확인하세요." });
            }

            // Duration을 "HH:MM:SS" 형식으로 변환
            string durationString = string.Empty;
            if (metadata.Duration.HasValue)
            {
                var duration = metadata.Duration.Value;
                durationString = duration.Hours > 0
                    ? $"{duration.Hours}:{duration.Minutes:D2}:{duration.Seconds:D2}"
                    : $"{duration.Minutes}:{duration.Seconds:D2}";
            }

            return Json(new
            {
                success = true,
                data = new
                {
                    title = metadata.Title,
                    description = metadata.Description,
                    thumbnailUrl = metadata.ThumbnailUrl,
                    duration = durationString,
                    publishedAt = metadata.PublishedAt?.ToString("yyyy-MM-dd")
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching YouTube metadata");
            return Json(new { success = false, message = "메타데이터 가져오기 중 오류가 발생했습니다." });
        }
    }
}
