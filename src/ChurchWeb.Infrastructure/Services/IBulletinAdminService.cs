using ChurchWeb.Core.Entities.News;

namespace ChurchWeb.Infrastructure.Services;

public interface IBulletinAdminService
{
    Task<(IEnumerable<Bulletin> bulletins, int totalCount)> GetPagedBulletinsAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        bool? isVisible = null,
        string sortBy = "PublishedDate",
        bool sortDescending = true);

    Task<Bulletin?> GetBulletinByIdAsync(int id);
    Task<Bulletin?> GetBulletinByIdWithoutPdfDataAsync(int id); // PDF 데이터 제외 (목록 조회 시)
    Task CreateBulletinAsync(Bulletin bulletin);
    Task UpdateBulletinAsync(Bulletin bulletin);
    Task<bool> DeleteBulletinAsync(int id);
    Task<bool> ToggleVisibilityAsync(int id);
    Task<byte[]?> GetPdfDataAsync(int id); // PDF 다운로드용
}
