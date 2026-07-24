using ChurchWeb.Core.Entities.News;
using ChurchWeb.Infrastructure.Services;
using ChurchWeb.Web.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChurchWeb.Web.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("Admin/Albums")]
public class AlbumsAdminController : Controller
{
    private readonly IAlbumAdminService _albumService;
    private readonly ILogger<AlbumsAdminController> _logger;
    private const long MaxFileSize = 10 * 1024 * 1024; // 10MB
    private static readonly string[] AllowedImageTypes = { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };

    public AlbumsAdminController(
        IAlbumAdminService albumService,
        ILogger<AlbumsAdminController> logger)
    {
        _albumService = albumService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(
        int page = 1,
        string? searchTerm = null,
        int? year = null,
        bool? isVisible = null,
        string sortBy = "EventDate",
        bool sortDescending = true)
    {
        var (albums, totalCount) = await _albumService.GetPagedAlbumsAsync(
            page, 20, searchTerm, year, isVisible, sortBy, sortDescending);

        var availableYears = await _albumService.GetAvailableYearsAsync();

        var model = new AlbumListViewModel
        {
            Albums = albums.Select(a => new AlbumItemViewModel
            {
                Id = a.Id,
                Title = a.Title,
                EventDate = a.EventDate,
                Category = a.Category,
                CoverImageUrl = a.CoverImageUrl,
                PhotoCount = a.Photos.Count,
                IsVisible = a.IsVisible,
                SortOrder = a.SortOrder
            }).ToList(),
            Pagination = new PaginationViewModel
            {
                CurrentPage = page,
                PageSize = 20,
                TotalCount = totalCount
            },
            Filter = new AlbumFilterViewModel
            {
                SearchTerm = searchTerm,
                Year = year,
                IsVisible = isVisible,
                SortBy = sortBy,
                SortDescending = sortDescending
            },
            AvailableYears = availableYears
        };

        return View(model);
    }

    [HttpGet("Create")]
    public IActionResult Create()
    {
        var model = new AlbumFormViewModel();
        return View("Form", model);
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var album = await _albumService.GetAlbumByIdWithPhotosAsync(id);
        if (album == null)
            return NotFound();

        var model = new AlbumFormViewModel
        {
            Id = album.Id,
            Title = album.Title,
            EventDate = album.EventDate,
            Category = album.Category,
            Description = album.Description,
            ExistingCoverImageUrl = album.CoverImageUrl,
            IsVisible = album.IsVisible,
            SortOrder = album.SortOrder,
            Photos = album.Photos.Select(p => new AlbumPhotoViewModel
            {
                Id = p.Id,
                ImageUrl = p.ImageUrl,
                Caption = p.Caption,
                SortOrder = p.SortOrder
            }).ToList()
        };

        return View("Form", model);
    }

    [HttpPost("Save")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(MaxFileSize)]
    public async Task<IActionResult> Save(AlbumFormViewModel model)
    {
        // Cover image validation
        if (model.CoverImage != null)
        {
            if (model.CoverImage.Length > MaxFileSize)
            {
                ModelState.AddModelError("CoverImage", $"파일 크기는 {MaxFileSize / (1024 * 1024)}MB 이하여야 합니다.");
            }

            if (!AllowedImageTypes.Contains(model.CoverImage.ContentType.ToLower()))
            {
                ModelState.AddModelError("CoverImage", "이미지 파일만 업로드 가능합니다 (JPEG, PNG, GIF, WebP).");
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
                var album = new Album
                {
                    Title = model.Title,
                    EventDate = model.EventDate,
                    Category = model.Category,
                    Description = model.Description,
                    IsVisible = model.IsVisible,
                    SortOrder = model.SortOrder
                };

                // Save cover image
                if (model.CoverImage != null)
                {
                    using var stream = model.CoverImage.OpenReadStream();
                    album.CoverImageUrl = await _albumService.SavePhotoFileAsync(stream, model.CoverImage.FileName);
                }

                await _albumService.CreateAlbumAsync(album);
                TempData["SuccessMessage"] = "앨범이 성공적으로 등록되었습니다.";
                return RedirectToAction("Edit", new { id = album.Id });
            }
            else
            {
                // Update
                var album = await _albumService.GetAlbumByIdAsync(model.Id);
                if (album == null)
                    return NotFound();

                album.Title = model.Title;
                album.EventDate = model.EventDate;
                album.Category = model.Category;
                album.Description = model.Description;
                album.IsVisible = model.IsVisible;
                album.SortOrder = model.SortOrder;

                // Update cover image if new one uploaded
                if (model.CoverImage != null)
                {
                    // Delete old cover image
                    if (!string.IsNullOrEmpty(album.CoverImageUrl))
                    {
                        await _albumService.DeletePhotoFileAsync(album.CoverImageUrl);
                    }

                    using var stream = model.CoverImage.OpenReadStream();
                    album.CoverImageUrl = await _albumService.SavePhotoFileAsync(stream, model.CoverImage.FileName);
                }

                await _albumService.UpdateAlbumAsync(album);
                TempData["SuccessMessage"] = "앨범이 성공적으로 수정되었습니다.";
                return RedirectToAction("Edit", new { id = album.Id });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving album");
            ModelState.AddModelError(string.Empty, "저장 중 오류가 발생했습니다.");
            return View("Form", model);
        }
    }

    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _albumService.DeleteAlbumAsync(id);
        if (result)
        {
            return Json(new { success = true, message = "앨범이 삭제되었습니다." });
        }

        return Json(new { success = false, message = "앨범을 찾을 수 없습니다." });
    }

    [HttpPost("ToggleVisibility/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleVisibility(int id)
    {
        var result = await _albumService.ToggleVisibilityAsync(id);
        if (result)
        {
            return Json(new { success = true, message = "표시 여부가 변경되었습니다." });
        }

        return Json(new { success = false, message = "앨범을 찾을 수 없습니다." });
    }

    // Photo management

    [HttpPost("UploadPhoto")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(MaxFileSize)]
    public async Task<IActionResult> UploadPhoto(PhotoUploadViewModel model)
    {
        if (model.PhotoFile == null || model.PhotoFile.Length == 0)
        {
            return Json(new { success = false, message = "파일을 선택하세요." });
        }

        if (model.PhotoFile.Length > MaxFileSize)
        {
            return Json(new { success = false, message = $"파일 크기는 {MaxFileSize / (1024 * 1024)}MB 이하여야 합니다." });
        }

        if (!AllowedImageTypes.Contains(model.PhotoFile.ContentType.ToLower()))
        {
            return Json(new { success = false, message = "이미지 파일만 업로드 가능합니다." });
        }

        try
        {
            using var stream = model.PhotoFile.OpenReadStream();
            var imageUrl = await _albumService.SavePhotoFileAsync(stream, model.PhotoFile.FileName);

            var photo = new AlbumPhoto
            {
                AlbumId = model.AlbumId,
                ImageUrl = imageUrl,
                Caption = model.Caption ?? string.Empty
            };

            await _albumService.AddPhotoAsync(photo);

            return Json(new
            {
                success = true,
                message = "사진이 업로드되었습니다.",
                photo = new
                {
                    id = photo.Id,
                    imageUrl = photo.ImageUrl,
                    caption = photo.Caption,
                    sortOrder = photo.SortOrder
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading photo");
            return Json(new { success = false, message = "업로드 중 오류가 발생했습니다." });
        }
    }

    [HttpPost("DeletePhoto/{photoId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePhoto(int photoId)
    {
        var result = await _albumService.DeletePhotoAsync(photoId);
        if (result)
        {
            return Json(new { success = true, message = "사진이 삭제되었습니다." });
        }

        return Json(new { success = false, message = "사진을 찾을 수 없습니다." });
    }

    [HttpPost("ReorderPhotos")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReorderPhotos([FromBody] ReorderPhotosRequest request)
    {
        try
        {
            await _albumService.ReorderPhotosAsync(request.AlbumId, request.PhotoIds);
            return Json(new { success = true, message = "순서가 변경되었습니다." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering photos");
            return Json(new { success = false, message = "순서 변경 중 오류가 발생했습니다." });
        }
    }

    public class ReorderPhotosRequest
    {
        public int AlbumId { get; set; }
        public List<int> PhotoIds { get; set; } = new();
    }
}
