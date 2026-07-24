using ChurchWeb.Core.Entities.Sermons;
using ChurchWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChurchWeb.Infrastructure.Services;

public class SermonAdminService : ISermonAdminService
{
    private readonly AppDbContext _context;

    public SermonAdminService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(List<Sermon> Items, int TotalCount)> GetPagedSermonsAsync(
        int page = 1,
        int pageSize = 20,
        string? searchTerm = null,
        string? category = null,
        bool? isVisible = null,
        string sortBy = "PreachedOn",
        bool sortDescending = true)
    {
        var query = _context.Sermons.AsQueryable();

        // 검색
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(s =>
                s.Title.Contains(searchTerm) ||
                s.Preacher.Contains(searchTerm) ||
                s.Verse.Contains(searchTerm));
        }

        // 분류 필터
        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(s => s.Category == category);
        }

        // 표시 여부 필터
        if (isVisible.HasValue)
        {
            query = query.Where(s => s.IsVisible == isVisible.Value);
        }

        // 정렬
        query = sortBy.ToLower() switch
        {
            "title" => sortDescending ? query.OrderByDescending(s => s.Title) : query.OrderBy(s => s.Title),
            "preacher" => sortDescending ? query.OrderByDescending(s => s.Preacher) : query.OrderBy(s => s.Preacher),
            "category" => sortDescending ? query.OrderByDescending(s => s.Category) : query.OrderBy(s => s.Category),
            "isvisible" => sortDescending ? query.OrderByDescending(s => s.IsVisible) : query.OrderBy(s => s.IsVisible),
            "sortorder" => sortDescending ? query.OrderByDescending(s => s.SortOrder) : query.OrderBy(s => s.SortOrder),
            _ => sortDescending ? query.OrderByDescending(s => s.PreachedOn) : query.OrderBy(s => s.PreachedOn)
        };

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Sermon?> GetSermonByIdAsync(int id)
    {
        return await _context.Sermons.FindAsync(id);
    }

    public async Task<Sermon> CreateSermonAsync(Sermon sermon)
    {
        sermon.CreatedAt = DateTime.UtcNow;
        sermon.UpdatedAt = DateTime.UtcNow;

        // SortOrder가 설정되지 않았으면 자동 설정
        if (sermon.SortOrder == 0)
        {
            var maxSortOrder = await _context.Sermons.MaxAsync(s => (int?)s.SortOrder) ?? 0;
            sermon.SortOrder = maxSortOrder + 1;
        }

        _context.Sermons.Add(sermon);
        await _context.SaveChangesAsync();
        return sermon;
    }

    public async Task<Sermon> UpdateSermonAsync(Sermon sermon)
    {
        sermon.UpdatedAt = DateTime.UtcNow;
        _context.Sermons.Update(sermon);
        await _context.SaveChangesAsync();
        return sermon;
    }

    public async Task<bool> DeleteSermonAsync(int id)
    {
        var sermon = await _context.Sermons.FindAsync(id);
        if (sermon == null)
            return false;

        _context.Sermons.Remove(sermon);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleVisibilityAsync(int id)
    {
        var sermon = await _context.Sermons.FindAsync(id);
        if (sermon == null)
            return false;

        sermon.IsVisible = !sermon.IsVisible;
        sermon.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateSortOrderAsync(int id, int newSortOrder)
    {
        var sermon = await _context.Sermons.FindAsync(id);
        if (sermon == null)
            return false;

        var oldSortOrder = sermon.SortOrder;

        if (oldSortOrder == newSortOrder)
            return true;

        // 범위 내의 다른 항목들의 순서 조정
        if (newSortOrder < oldSortOrder)
        {
            // 위로 이동: 사이에 있는 항목들을 아래로
            var itemsToUpdate = await _context.Sermons
                .Where(s => s.SortOrder >= newSortOrder && s.SortOrder < oldSortOrder && s.Id != id)
                .ToListAsync();

            foreach (var item in itemsToUpdate)
            {
                item.SortOrder++;
                item.UpdatedAt = DateTime.UtcNow;
            }
        }
        else
        {
            // 아래로 이동: 사이에 있는 항목들을 위로
            var itemsToUpdate = await _context.Sermons
                .Where(s => s.SortOrder > oldSortOrder && s.SortOrder <= newSortOrder && s.Id != id)
                .ToListAsync();

            foreach (var item in itemsToUpdate)
            {
                item.SortOrder--;
                item.UpdatedAt = DateTime.UtcNow;
            }
        }

        sermon.SortOrder = newSortOrder;
        sermon.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }
}
