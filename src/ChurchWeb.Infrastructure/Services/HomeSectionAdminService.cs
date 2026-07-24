using ChurchWeb.Core.Entities.Common;
using ChurchWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChurchWeb.Infrastructure.Services;

public class HomeSectionAdminService : IHomeSectionAdminService
{
    private readonly AppDbContext _context;

    public HomeSectionAdminService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<HomeSection>> GetAllHomeSectionsAsync()
    {
        return await _context.HomeSections
            .OrderBy(s => s.SortOrder)
            .ToListAsync();
    }

    public async Task<HomeSection?> GetHomeSectionByIdAsync(int id)
    {
        return await _context.HomeSections
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<HomeSection> UpdateHomeSectionAsync(HomeSection homeSection)
    {
        homeSection.UpdatedAt = DateTime.UtcNow;
        _context.HomeSections.Update(homeSection);
        await _context.SaveChangesAsync();
        return homeSection;
    }

    public async Task<bool> ToggleVisibilityAsync(int id)
    {
        var section = await _context.HomeSections.FindAsync(id);
        if (section == null)
            return false;

        section.IsVisible = !section.IsVisible;
        section.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateSortOrderAsync(int id, int newSortOrder)
    {
        var section = await _context.HomeSections.FindAsync(id);
        if (section == null)
            return false;

        var oldSortOrder = section.SortOrder;

        if (oldSortOrder == newSortOrder)
            return true;

        // 다른 섹션들의 순서 조정
        var otherSections = await _context.HomeSections
            .Where(s => s.Id != id)
            .ToListAsync();

        if (newSortOrder < oldSortOrder)
        {
            // 위로 이동
            foreach (var other in otherSections.Where(s => s.SortOrder >= newSortOrder && s.SortOrder < oldSortOrder))
            {
                other.SortOrder++;
                other.UpdatedAt = DateTime.UtcNow;
            }
        }
        else
        {
            // 아래로 이동
            foreach (var other in otherSections.Where(s => s.SortOrder > oldSortOrder && s.SortOrder <= newSortOrder))
            {
                other.SortOrder--;
                other.UpdatedAt = DateTime.UtcNow;
            }
        }

        section.SortOrder = newSortOrder;
        section.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateSortOrdersBulkAsync(Dictionary<int, int> sortOrders)
    {
        foreach (var (id, sortOrder) in sortOrders)
        {
            var section = await _context.HomeSections.FindAsync(id);
            if (section != null)
            {
                section.SortOrder = sortOrder;
                section.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
