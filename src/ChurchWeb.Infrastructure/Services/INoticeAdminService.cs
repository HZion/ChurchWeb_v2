using ChurchWeb.Core.Entities.News;

namespace ChurchWeb.Infrastructure.Services;

public interface INoticeAdminService
{
    // Notice CRUD
    Task<(IEnumerable<Notice> notices, int totalCount)> GetPagedNoticesAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        string? categoryKey = null,
        bool? isPinned = null,
        bool? isVisible = null,
        string sortBy = "PostedOn",
        bool sortDescending = true);

    Task<Notice?> GetNoticeByIdAsync(int id);
    Task<Notice?> GetNoticeByIdWithAttachmentsAsync(int id);
    Task CreateNoticeAsync(Notice notice);
    Task UpdateNoticeAsync(Notice notice);
    Task<bool> DeleteNoticeAsync(int id);
    Task<bool> ToggleVisibilityAsync(int id);
    Task<bool> TogglePinnedAsync(int id);
    Task IncrementViewsAsync(int id);

    // Attachment management
    Task<NoticeAttachment?> GetAttachmentByIdAsync(int attachmentId);
    Task AddAttachmentAsync(NoticeAttachment attachment);
    Task<bool> DeleteAttachmentAsync(int attachmentId);

    // Utility
    Task<string> SaveAttachmentFileAsync(Stream fileStream, string fileName);
    Task<bool> DeleteAttachmentFileAsync(string fileUrl);
}
