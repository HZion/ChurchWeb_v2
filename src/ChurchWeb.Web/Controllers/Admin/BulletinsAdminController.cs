using ChurchWeb.Core.Entities.News;
using ChurchWeb.Infrastructure.Services;
using ChurchWeb.Web.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChurchWeb.Web.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("Admin/Bulletins")]
public class BulletinsAdminController : Controller
{
    private readonly IBulletinAdminService _bulletinService;
    private readonly ILogger<BulletinsAdminController> _logger;
    private const long MaxFileSize = 10 * 1024 * 1024; // 10MB

    public BulletinsAdminController(
        IBulletinAdminService bulletinService,
        ILogger<BulletinsAdminController> logger)
    {
        _bulletinService = bulletinService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(
        int page = 1,
        string? searchTerm = null,
        bool? isVisible = null,
        string sortBy = "PublishedDate",
        bool sortDescending = true)
    {
        var (bulletins, totalCount) = await _bulletinService.GetPagedBulletinsAsync(
            page, 20, searchTerm, isVisible, sortBy, sortDescending);

        var model = new BulletinListViewModel
        {
            Bulletins = bulletins.Select(b => new BulletinItemViewModel
            {
                Id = b.Id,
                Title = b.Title,
                PublishedDate = b.PublishedDate,
                FileName = b.FileName,
                FileSize = b.FileSize,
                IsVisible = b.IsVisible,
                SortOrder = b.SortOrder
            }).ToList(),
            Pagination = new PaginationViewModel
            {
                CurrentPage = page,
                PageSize = 20,
                TotalCount = totalCount
            },
            Filter = new BulletinFilterViewModel
            {
                SearchTerm = searchTerm,
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
        var model = new BulletinFormViewModel();
        return View("Form", model);
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var bulletin = await _bulletinService.GetBulletinByIdWithoutPdfDataAsync(id);
        if (bulletin == null)
            return NotFound();

        var model = new BulletinFormViewModel
        {
            Id = bulletin.Id,
            Title = bulletin.Title,
            PublishedDate = bulletin.PublishedDate,
            ExistingFileName = bulletin.FileName,
            ExistingFileSize = bulletin.FileSize,
            IsVisible = bulletin.IsVisible,
            SortOrder = bulletin.SortOrder
        };

        return View("Form", model);
    }

    [HttpPost("Save")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(MaxFileSize)]
    public async Task<IActionResult> Save(BulletinFormViewModel model)
    {
        // PDF 파일 검증
        if (model.Id == 0 && model.PdfFile == null)
        {
            ModelState.AddModelError("PdfFile", "PDF 파일을 업로드하세요.");
        }

        if (model.PdfFile != null)
        {
            if (model.PdfFile.Length > MaxFileSize)
            {
                ModelState.AddModelError("PdfFile", $"파일 크기는 {MaxFileSize / (1024 * 1024)}MB 이하여야 합니다.");
            }

            if (model.PdfFile.ContentType != "application/pdf")
            {
                ModelState.AddModelError("PdfFile", "PDF 파일만 업로드 가능합니다.");
            }
        }

        if (!ModelState.IsValid)
        {
            return View("Form", model);
        }

        try
        {
            if (model.Id == 0)
            {
                // Create
                var bulletin = new Bulletin
                {
                    Title = model.Title,
                    PublishedDate = model.PublishedDate,
                    IsVisible = model.IsVisible,
                    SortOrder = model.SortOrder
                };

                // PDF 파일 읽기
                if (model.PdfFile != null)
                {
                    using var memoryStream = new MemoryStream();
                    await model.PdfFile.CopyToAsync(memoryStream);
                    bulletin.PdfData = memoryStream.ToArray();
                    bulletin.FileName = model.PdfFile.FileName;
                    bulletin.FileSize = model.PdfFile.Length;
                    bulletin.ContentType = model.PdfFile.ContentType;
                }

                await _bulletinService.CreateBulletinAsync(bulletin);
                TempData["SuccessMessage"] = "주보가 성공적으로 등록되었습니다.";
            }
            else
            {
                // Update
                var bulletin = await _bulletinService.GetBulletinByIdAsync(model.Id);
                if (bulletin == null)
                    return NotFound();

                bulletin.Title = model.Title;
                bulletin.PublishedDate = model.PublishedDate;
                bulletin.IsVisible = model.IsVisible;
                bulletin.SortOrder = model.SortOrder;

                // PDF 파일이 새로 업로드된 경우
                if (model.PdfFile != null)
                {
                    using var memoryStream = new MemoryStream();
                    await model.PdfFile.CopyToAsync(memoryStream);
                    bulletin.PdfData = memoryStream.ToArray();
                    bulletin.FileName = model.PdfFile.FileName;
                    bulletin.FileSize = model.PdfFile.Length;
                    bulletin.ContentType = model.PdfFile.ContentType;
                }

                await _bulletinService.UpdateBulletinAsync(bulletin);
                TempData["SuccessMessage"] = "주보가 성공적으로 수정되었습니다.";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving bulletin");
            ModelState.AddModelError(string.Empty, "저장 중 오류가 발생했습니다.");
            return View("Form", model);
        }
    }

    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _bulletinService.DeleteBulletinAsync(id);
        if (result)
        {
            return Json(new { success = true, message = "주보가 삭제되었습니다." });
        }

        return Json(new { success = false, message = "주보를 찾을 수 없습니다." });
    }

    [HttpPost("ToggleVisibility/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleVisibility(int id)
    {
        var result = await _bulletinService.ToggleVisibilityAsync(id);
        if (result)
        {
            return Json(new { success = true, message = "표시 여부가 변경되었습니다." });
        }

        return Json(new { success = false, message = "주보를 찾을 수 없습니다." });
    }

    [HttpGet("Download/{id}")]
    public async Task<IActionResult> Download(int id)
    {
        var bulletin = await _bulletinService.GetBulletinByIdAsync(id);
        if (bulletin == null || bulletin.PdfData == null || bulletin.PdfData.Length == 0)
            return NotFound();

        return File(bulletin.PdfData, bulletin.ContentType, bulletin.FileName);
    }
}
