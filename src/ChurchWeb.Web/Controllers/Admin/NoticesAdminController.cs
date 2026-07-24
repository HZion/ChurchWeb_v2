using ChurchWeb.Core.Entities.News;
using ChurchWeb.Infrastructure.Services;
using ChurchWeb.Web.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChurchWeb.Web.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("Admin/Notices")]
public class NoticesAdminController : Controller
{
    private readonly INoticeAdminService _noticeService;
    private readonly ILogger<NoticesAdminController> _logger;
    private const long MaxFileSize = 50 * 1024 * 1024; // 50MB
    private static readonly string[] AllowedFileTypes = {
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "application/vnd.ms-powerpoint",
        "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/gif",
        "image/webp",
        "text/plain",
        "application/zip",
        "application/x-zip-compressed"
    };

    public NoticesAdminController(
        INoticeAdminService noticeService,
        ILogger<NoticesAdminController> logger)
    {
        _noticeService = noticeService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(
        int page = 1,
        string? searchTerm = null,
        string? categoryKey = null,
        bool? isPinned = null,
        bool? isVisible = null,
        string sortBy = "PostedOn",
        bool sortDescending = true)
    {
        var (notices, totalCount) = await _noticeService.GetPagedNoticesAsync(
            page, 20, searchTerm, categoryKey, isPinned, isVisible, sortBy, sortDescending);

        var model = new NoticeListViewModel
        {
            Notices = notices.Select(n => new NoticeItemViewModel
            {
                Id = n.Id,
                CategoryKey = n.CategoryKey,
                Title = n.Title,
                Author = n.Author,
                PostedOn = n.PostedOn,
                Views = n.Views,
                IsPinned = n.IsPinned,
                IsVisible = n.IsVisible,
                AttachmentCount = n.Attachments.Count
            }).ToList(),
            Pagination = new PaginationViewModel
            {
                CurrentPage = page,
                PageSize = 20,
                TotalCount = totalCount
            },
            Filter = new NoticeFilterViewModel
            {
                SearchTerm = searchTerm,
                CategoryKey = categoryKey,
                IsPinned = isPinned,
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
        var model = new NoticeFormViewModel();
        return View("Form", model);
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var notice = await _noticeService.GetNoticeByIdWithAttachmentsAsync(id);
        if (notice == null)
            return NotFound();

        var model = new NoticeFormViewModel
        {
            Id = notice.Id,
            CategoryKey = notice.CategoryKey,
            Title = notice.Title,
            Author = notice.Author,
            PostedOn = notice.PostedOn,
            BodyHtml = notice.BodyHtml,
            IsPinned = notice.IsPinned,
            IsVisible = notice.IsVisible,
            SortOrder = notice.SortOrder,
            Attachments = notice.Attachments.Select(a => new NoticeAttachmentViewModel
            {
                Id = a.Id,
                FileName = a.FileName,
                FileUrl = a.FileUrl,
                SortOrder = a.SortOrder
            }).ToList()
        };

        return View("Form", model);
    }

    [HttpPost("Save")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(NoticeFormViewModel model)
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
                var notice = new Notice
                {
                    CategoryKey = model.CategoryKey,
                    Title = model.Title,
                    Author = model.Author,
                    PostedOn = model.PostedOn,
                    BodyHtml = model.BodyHtml,
                    IsPinned = model.IsPinned,
                    IsVisible = model.IsVisible,
                    SortOrder = model.SortOrder
                };

                await _noticeService.CreateNoticeAsync(notice);
                TempData["SuccessMessage"] = "공지사항이 성공적으로 등록되었습니다.";
                return RedirectToAction("Edit", new { id = notice.Id });
            }
            else
            {
                // Update
                var notice = await _noticeService.GetNoticeByIdAsync(model.Id);
                if (notice == null)
                    return NotFound();

                notice.CategoryKey = model.CategoryKey;
                notice.Title = model.Title;
                notice.Author = model.Author;
                notice.PostedOn = model.PostedOn;
                notice.BodyHtml = model.BodyHtml;
                notice.IsPinned = model.IsPinned;
                notice.IsVisible = model.IsVisible;
                notice.SortOrder = model.SortOrder;

                await _noticeService.UpdateNoticeAsync(notice);
                TempData["SuccessMessage"] = "공지사항이 성공적으로 수정되었습니다.";
                return RedirectToAction("Edit", new { id = notice.Id });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving notice");
            ModelState.AddModelError(string.Empty, "저장 중 오류가 발생했습니다.");
            return View("Form", model);
        }
    }

    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _noticeService.DeleteNoticeAsync(id);
        if (result)
        {
            return Json(new { success = true, message = "공지사항이 삭제되었습니다." });
        }

        return Json(new { success = false, message = "공지사항을 찾을 수 없습니다." });
    }

    [HttpPost("ToggleVisibility/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleVisibility(int id)
    {
        var result = await _noticeService.ToggleVisibilityAsync(id);
        if (result)
        {
            return Json(new { success = true, message = "표시 여부가 변경되었습니다." });
        }

        return Json(new { success = false, message = "공지사항을 찾을 수 없습니다." });
    }

    [HttpPost("TogglePinned/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TogglePinned(int id)
    {
        var result = await _noticeService.TogglePinnedAsync(id);
        if (result)
        {
            return Json(new { success = true, message = "필독 여부가 변경되었습니다." });
        }

        return Json(new { success = false, message = "공지사항을 찾을 수 없습니다." });
    }

    // Attachment management

    [HttpPost("UploadAttachment")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(MaxFileSize)]
    public async Task<IActionResult> UploadAttachment(AttachmentUploadViewModel model)
    {
        if (model.AttachmentFile == null || model.AttachmentFile.Length == 0)
        {
            return Json(new { success = false, message = "파일을 선택하세요." });
        }

        if (model.AttachmentFile.Length > MaxFileSize)
        {
            return Json(new { success = false, message = $"파일 크기는 {MaxFileSize / (1024 * 1024)}MB 이하여야 합니다." });
        }

        if (!AllowedFileTypes.Contains(model.AttachmentFile.ContentType.ToLower()))
        {
            return Json(new { success = false, message = "허용되지 않는 파일 형식입니다." });
        }

        try
        {
            using var stream = model.AttachmentFile.OpenReadStream();
            var fileUrl = await _noticeService.SaveAttachmentFileAsync(stream, model.AttachmentFile.FileName);

            var attachment = new NoticeAttachment
            {
                NoticeId = model.NoticeId,
                FileName = model.AttachmentFile.FileName,
                FileUrl = fileUrl
            };

            await _noticeService.AddAttachmentAsync(attachment);

            return Json(new
            {
                success = true,
                message = "첨부파일이 업로드되었습니다.",
                attachment = new
                {
                    id = attachment.Id,
                    fileName = attachment.FileName,
                    fileUrl = attachment.FileUrl,
                    sortOrder = attachment.SortOrder
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading attachment");
            return Json(new { success = false, message = "업로드 중 오류가 발생했습니다." });
        }
    }

    [HttpPost("DeleteAttachment/{attachmentId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAttachment(int attachmentId)
    {
        var result = await _noticeService.DeleteAttachmentAsync(attachmentId);
        if (result)
        {
            return Json(new { success = true, message = "첨부파일이 삭제되었습니다." });
        }

        return Json(new { success = false, message = "첨부파일을 찾을 수 없습니다." });
    }
}
