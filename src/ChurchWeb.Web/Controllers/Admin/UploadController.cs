using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChurchWeb.Web.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("api/admin/[controller]")]
[ApiController]
public class UploadController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<UploadController> _logger;
    private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    public UploadController(IWebHostEnvironment environment, ILogger<UploadController> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    [HttpPost("image")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { error = "파일이 선택되지 않았습니다." });
            }

            // 파일 크기 검증
            if (file.Length > _maxFileSize)
            {
                return BadRequest(new { error = "파일 크기는 5MB를 초과할 수 없습니다." });
            }

            // 확장자 검증
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
            {
                return BadRequest(new { error = "허용되지 않는 파일 형식입니다. (jpg, jpeg, png, gif, webp만 가능)" });
            }

            // 업로드 폴더 확인 및 생성
            var uploadPath = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // 고유 파일명 생성
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadPath, fileName);

            // 파일 저장
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // URL 반환
            var fileUrl = $"/uploads/{fileName}";
            _logger.LogInformation("Image uploaded successfully: {FileUrl}", fileUrl);

            return Ok(new { url = fileUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image");
            return StatusCode(500, new { error = "이미지 업로드 중 오류가 발생했습니다." });
        }
    }
}
