using ChurchWeb.Core.Entities.News;
using ChurchWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChurchWeb.Infrastructure.Services;

public class BulletinAdminService : IBulletinAdminService
{
    private readonly AppDbContext _context;

    public BulletinAdminService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Bulletin> bulletins, int totalCount)> GetPagedBulletinsAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        bool? isVisible = null,
        string sortBy = "PublishedDate",
        bool sortDescending = true)
    {
        var query = _context.Bulletins.AsQueryable();

        // 검색
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(b => b.Title.Contains(searchTerm) || b.FileName.Contains(searchTerm));
        }

        // 필터
        if (isVisible.HasValue)
        {
            query = query.Where(b => b.IsVisible == isVisible.Value);
        }

        // 정렬
        query = sortBy switch
        {
            "Title" => sortDescending ? query.OrderByDescending(b => b.Title) : query.OrderBy(b => b.Title),
            "SortOrder" => sortDescending ? query.OrderByDescending(b => b.SortOrder) : query.OrderBy(b => b.SortOrder),
            _ => sortDescending ? query.OrderByDescending(b => b.PublishedDate) : query.OrderBy(b => b.PublishedDate)
        };

        var totalCount = await query.CountAsync();

        // PDF 데이터를 제외하고 조회 (성능 최적화)
        var bulletins = await query
            .Select(b => new Bulletin
            {
                Id = b.Id,
                Title = b.Title,
                PublishedDate = b.PublishedDate,
                FileName = b.FileName,
                FileSize = b.FileSize,
                ContentType = b.ContentType,
                CoverImageUrl = b.CoverImageUrl,
                IsVisible = b.IsVisible,
                SortOrder = b.SortOrder,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt,
                PdfData = Array.Empty<byte>() // PDF 데이터 제외
            })
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (bulletins, totalCount);
    }

    public async Task<Bulletin?> GetBulletinByIdAsync(int id)
    {
        return await _context.Bulletins.FindAsync(id);
    }

    public async Task<Bulletin?> GetBulletinByIdWithoutPdfDataAsync(int id)
    {
        return await _context.Bulletins
            .Where(b => b.Id == id)
            .Select(b => new Bulletin
            {
                Id = b.Id,
                Title = b.Title,
                PublishedDate = b.PublishedDate,
                FileName = b.FileName,
                FileSize = b.FileSize,
                ContentType = b.ContentType,
                CoverImageUrl = b.CoverImageUrl,
                IsVisible = b.IsVisible,
                SortOrder = b.SortOrder,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt,
                PdfData = Array.Empty<byte>()
            })
            .FirstOrDefaultAsync();
    }

    public async Task CreateBulletinAsync(Bulletin bulletin)
    {
        bulletin.CreatedAt = DateTime.UtcNow;
        bulletin.UpdatedAt = DateTime.UtcNow;

        // SortOrder 자동 설정
        if (bulletin.SortOrder == 0)
        {
            var maxSortOrder = await _context.Bulletins.MaxAsync(b => (int?)b.SortOrder) ?? 0;
            bulletin.SortOrder = maxSortOrder + 1;
        }

        _context.Bulletins.Add(bulletin);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateBulletinAsync(Bulletin bulletin)
    {
        bulletin.UpdatedAt = DateTime.UtcNow;
        _context.Bulletins.Update(bulletin);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteBulletinAsync(int id)
    {
        var bulletin = await _context.Bulletins.FindAsync(id);
        if (bulletin == null)
            return false;

        _context.Bulletins.Remove(bulletin);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleVisibilityAsync(int id)
    {
        var bulletin = await _context.Bulletins.FindAsync(id);
        if (bulletin == null)
            return false;

        bulletin.IsVisible = !bulletin.IsVisible;
        bulletin.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<byte[]?> GetPdfDataAsync(int id)
    {
        var bulletin = await _context.Bulletins
            .Where(b => b.Id == id)
            .Select(b => b.PdfData)
            .FirstOrDefaultAsync();

        return bulletin;
    }
}
