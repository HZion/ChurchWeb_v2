using ChurchWeb.Core.Entities.Common;
using ChurchWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChurchWeb.Infrastructure.Services;

public class MenuAdminService : IMenuAdminService
{
    private readonly AppDbContext _context;

    public MenuAdminService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<NavMenuItem>> GetAllMenuItemsAsync()
    {
        return await _context.NavMenuItems
            .Include(m => m.Children)
            .OrderBy(m => m.ParentId)
            .ThenBy(m => m.SortOrder)
            .ToListAsync();
    }

    public async Task<List<NavMenuItem>> GetTopLevelMenuItemsAsync()
    {
        return await _context.NavMenuItems
            .Where(m => m.ParentId == null)
            .Include(m => m.Children.OrderBy(c => c.SortOrder))
            .OrderBy(m => m.SortOrder)
            .ToListAsync();
    }

    public async Task<NavMenuItem?> GetMenuItemByIdAsync(int id)
    {
        return await _context.NavMenuItems
            .Include(m => m.Children)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<NavMenuItem> CreateMenuItemAsync(NavMenuItem menuItem)
    {
        menuItem.CreatedAt = DateTime.UtcNow;
        menuItem.UpdatedAt = DateTime.UtcNow;

        // SortOrder가 설정되지 않았으면 자동 설정
        if (menuItem.SortOrder == 0)
        {
            var maxSortOrder = await _context.NavMenuItems
                .Where(m => m.ParentId == menuItem.ParentId)
                .MaxAsync(m => (int?)m.SortOrder) ?? 0;
            menuItem.SortOrder = maxSortOrder + 1;
        }

        _context.NavMenuItems.Add(menuItem);
        await _context.SaveChangesAsync();
        return menuItem;
    }

    public async Task<NavMenuItem> UpdateMenuItemAsync(NavMenuItem menuItem)
    {
        menuItem.UpdatedAt = DateTime.UtcNow;
        _context.NavMenuItems.Update(menuItem);
        await _context.SaveChangesAsync();
        return menuItem;
    }

    public async Task<bool> DeleteMenuItemAsync(int id)
    {
        var menuItem = await _context.NavMenuItems
            .Include(m => m.Children)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (menuItem == null)
            return false;

        // 하위 메뉴가 있으면 삭제 불가
        if (menuItem.Children.Any())
            return false;

        _context.NavMenuItems.Remove(menuItem);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleVisibilityAsync(int id)
    {
        var menuItem = await _context.NavMenuItems.FindAsync(id);
        if (menuItem == null)
            return false;

        menuItem.IsVisible = !menuItem.IsVisible;
        menuItem.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateSortOrderAsync(int id, int newSortOrder)
    {
        var menuItem = await _context.NavMenuItems.FindAsync(id);
        if (menuItem == null)
            return false;

        var oldSortOrder = menuItem.SortOrder;

        if (oldSortOrder == newSortOrder)
            return true;

        // 같은 부모를 가진 항목들의 순서 조정
        var siblings = await _context.NavMenuItems
            .Where(m => m.ParentId == menuItem.ParentId && m.Id != id)
            .ToListAsync();

        if (newSortOrder < oldSortOrder)
        {
            // 위로 이동
            foreach (var sibling in siblings.Where(s => s.SortOrder >= newSortOrder && s.SortOrder < oldSortOrder))
            {
                sibling.SortOrder++;
                sibling.UpdatedAt = DateTime.UtcNow;
            }
        }
        else
        {
            // 아래로 이동
            foreach (var sibling in siblings.Where(s => s.SortOrder > oldSortOrder && s.SortOrder <= newSortOrder))
            {
                sibling.SortOrder--;
                sibling.UpdatedAt = DateTime.UtcNow;
            }
        }

        menuItem.SortOrder = newSortOrder;
        menuItem.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateSortOrdersBulkAsync(Dictionary<int, int> sortOrders)
    {
        foreach (var (id, sortOrder) in sortOrders)
        {
            var menuItem = await _context.NavMenuItems.FindAsync(id);
            if (menuItem != null)
            {
                menuItem.SortOrder = sortOrder;
                menuItem.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
