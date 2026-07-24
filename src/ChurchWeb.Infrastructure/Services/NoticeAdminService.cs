using ChurchWeb.Core.Entities.News;
using ChurchWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChurchWeb.Infrastructure.Services;

public class NoticeAdminService : INoticeAdminService
{
    private readonly AppDbContext _context;
    private readonly ILogger<NoticeAdminService> _logger;
    private readonly string _webRootPath;
    private const string AttachmentsFolder = "uploads/notices";

    public NoticeAdminService(
        AppDbContext context,
        ILogger<NoticeAdminService> logger,
        string webRootPath)
    {
        _context = context;
        _logger = logger;
        _webRootPath = webRootPath;
    }

    public async Task<(IEnumerable<Notice> notices, int totalCount)> GetPagedNoticesAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        string? categoryKey = null,
        bool? isPinned = null,
        bool? isVisible = null,
        string sortBy = "PostedOn",
        bool sortDescending = true)
    {
        var query = _context.Notices.AsQueryable();

        // Filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(n => n.Title.Contains(searchTerm) || n.BodyHtml.Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(categoryKey))
        {
            query = query.Where(n => n.CategoryKey == categoryKey);
        }

        if (isPinned.HasValue)
        {
            query = query.Where(n => n.IsPinned == isPinned.Value);
        }

        if (isVisible.HasValue)
        {
            query = query.Where(n => n.IsVisible == isVisible.Value);
        }

        // Total count
        var totalCount = await query.CountAsync();

        // Sort
        query = sortBy switch
        {
            "Title" => sortDescending ? query.OrderByDescending(n => n.Title) : query.OrderBy(n => n.Title),
            "Author" => sortDescending ? query.OrderByDescending(n => n.Author) : query.OrderBy(n => n.Author),
            "Views" => sortDescending ? query.OrderByDescending(n => n.Views) : query.OrderBy(n => n.Views),
            "PostedOn" => sortDescending ? query.OrderByDescending(n => n.PostedOn) : query.OrderBy(n => n.PostedOn),
            _ => sortDescending ? query.OrderByDescending(n => n.PostedOn) : query.OrderBy(n => n.PostedOn)
        };

        // Paging with attachments
        var notices = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(n => n.Attachments)
            .ToListAsync();

        return (notices, totalCount);
    }

    public async Task<Notice?> GetNoticeByIdAsync(int id)
    {
        return await _context.Notices.FindAsync(id);
    }

    public async Task<Notice?> GetNoticeByIdWithAttachmentsAsync(int id)
    {
        return await _context.Notices
            .Include(n => n.Attachments.OrderBy(a => a.SortOrder))
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task CreateNoticeAsync(Notice notice)
    {
        // Auto-increment sort order if not set
        if (notice.SortOrder == 0)
        {
            var maxSortOrder = await _context.Notices.MaxAsync(n => (int?)n.SortOrder) ?? 0;
            notice.SortOrder = maxSortOrder + 1;
        }

        notice.CreatedAt = DateTime.UtcNow;
        notice.UpdatedAt = DateTime.UtcNow;

        _context.Notices.Add(notice);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateNoticeAsync(Notice notice)
    {
        notice.UpdatedAt = DateTime.UtcNow;

        _context.Notices.Update(notice);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteNoticeAsync(int id)
    {
        var notice = await GetNoticeByIdWithAttachmentsAsync(id);
        if (notice == null)
            return false;

        // Delete all attachment files
        foreach (var attachment in notice.Attachments)
        {
            await DeleteAttachmentFileAsync(attachment.FileUrl);
        }

        _context.Notices.Remove(notice);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ToggleVisibilityAsync(int id)
    {
        var notice = await GetNoticeByIdAsync(id);
        if (notice == null)
            return false;

        notice.IsVisible = !notice.IsVisible;
        notice.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TogglePinnedAsync(int id)
    {
        var notice = await GetNoticeByIdAsync(id);
        if (notice == null)
            return false;

        notice.IsPinned = !notice.IsPinned;
        notice.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task IncrementViewsAsync(int id)
    {
        var notice = await GetNoticeByIdAsync(id);
        if (notice != null)
        {
            notice.Views++;
            await _context.SaveChangesAsync();
        }
    }

    // Attachment management

    public async Task<NoticeAttachment?> GetAttachmentByIdAsync(int attachmentId)
    {
        return await _context.NoticeAttachments.FindAsync(attachmentId);
    }

    public async Task AddAttachmentAsync(NoticeAttachment attachment)
    {
        // Auto-increment sort order if not set
        if (attachment.SortOrder == 0)
        {
            var maxSortOrder = await _context.NoticeAttachments
                .Where(a => a.NoticeId == attachment.NoticeId)
                .MaxAsync(a => (int?)a.SortOrder) ?? 0;
            attachment.SortOrder = maxSortOrder + 1;
        }

        _context.NoticeAttachments.Add(attachment);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAttachmentAsync(int attachmentId)
    {
        var attachment = await GetAttachmentByIdAsync(attachmentId);
        if (attachment == null)
            return false;

        await DeleteAttachmentFileAsync(attachment.FileUrl);

        _context.NoticeAttachments.Remove(attachment);
        await _context.SaveChangesAsync();

        return true;
    }

    // Utility

    public async Task<string> SaveAttachmentFileAsync(Stream fileStream, string fileName)
    {
        try
        {
            // Create uploads directory if it doesn't exist
            var uploadsPath = Path.Combine(_webRootPath, AttachmentsFolder);
            Directory.CreateDirectory(uploadsPath);

            // Generate unique file name
            var extension = Path.GetExtension(fileName);
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}_{fileNameWithoutExt}{extension}";
            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            // Save file
            using (var fileStreamOut = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(fileStreamOut);
            }

            // Return relative URL
            return $"/{AttachmentsFolder}/{uniqueFileName}".Replace("\\", "/");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving attachment file: {FileName}", fileName);
            throw;
        }
    }

    public Task<bool> DeleteAttachmentFileAsync(string fileUrl)
    {
        try
        {
            if (string.IsNullOrEmpty(fileUrl))
                return Task.FromResult(false);

            // Extract file path from URL
            var relativePath = fileUrl.TrimStart('/');
            var filePath = Path.Combine(_webRootPath, relativePath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting attachment file: {FileUrl}", fileUrl);
            return Task.FromResult(false);
        }
    }
}
