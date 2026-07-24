namespace ChurchWeb.Application.Services;

public interface INewsService
{
    Task<object> GetBulletinListAsync(int page, int pageSize);
    Task<object?> GetBulletinDetailAsync(int id);
    Task<(byte[] Data, string ContentType, string FileName)?> GetBulletinPdfAsync(int id);
    Task<object> GetGalleryListAsync(int? year, int page, int pageSize);
    Task<object?> GetAlbumDetailAsync(int id);
    Task<object> GetNoticeListAsync(int page, int pageSize);
    Task<object?> GetNoticeDetailAsync(int id);
    Task<object> GetCalendarAsync(int year, int month);
}
