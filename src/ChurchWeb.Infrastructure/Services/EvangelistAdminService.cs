using ChurchWeb.Core.Entities.Outreach;
using ChurchWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChurchWeb.Infrastructure.Services;

public class EvangelistAdminService : IEvangelistAdminService
{
    private readonly AppDbContext _context;
    private readonly ILogger<EvangelistAdminService> _logger;

    public EvangelistAdminService(AppDbContext context, ILogger<EvangelistAdminService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Evangelist>> GetAllEvangelistsAsync()
    {
        return await _context.Evangelists
            .OrderBy(e => e.SortOrder)
            .ToListAsync();
    }

    public async Task<Evangelist?> GetEvangelistByIdAsync(int id)
    {
        return await _context.Evangelists.FindAsync(id);
    }

    public async Task<Evangelist> CreateEvangelistAsync(Evangelist evangelist)
    {
        evangelist.CreatedAt = DateTime.UtcNow;
        evangelist.UpdatedAt = DateTime.UtcNow;

        // 새 전도자는 맨 마지막 순서로 추가
        var maxSortOrder = await _context.Evangelists.MaxAsync(e => (int?)e.SortOrder) ?? 0;
        evangelist.SortOrder = maxSortOrder + 1;

        _context.Evangelists.Add(evangelist);
        await _context.SaveChangesAsync();

        _logger.LogInformation("전도자 추가: {Name} (ID: {Id})", evangelist.Name, evangelist.Id);
        return evangelist;
    }

    public async Task<Evangelist> UpdateEvangelistAsync(Evangelist evangelist)
    {
        var existing = await _context.Evangelists.FindAsync(evangelist.Id);
        if (existing == null)
        {
            throw new InvalidOperationException($"전도자 ID {evangelist.Id}를 찾을 수 없습니다.");
        }

        existing.Name = evangelist.Name;
        existing.Title = evangelist.Title;
        existing.Phone = evangelist.Phone;
        existing.PhotoUrl = evangelist.PhotoUrl;
        existing.Greeting = evangelist.Greeting;
        existing.IsActive = evangelist.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;

        _context.Evangelists.Update(existing);
        await _context.SaveChangesAsync();

        _logger.LogInformation("전도자 수정: {Name} (ID: {Id})", existing.Name, existing.Id);
        return existing;
    }

    public async Task DeleteEvangelistAsync(int id)
    {
        var evangelist = await _context.Evangelists.FindAsync(id);
        if (evangelist == null)
        {
            throw new InvalidOperationException($"전도자 ID {id}를 찾을 수 없습니다.");
        }

        _context.Evangelists.Remove(evangelist);
        await _context.SaveChangesAsync();

        _logger.LogInformation("전도자 삭제: {Name} (ID: {Id})", evangelist.Name, id);
    }

    public async Task<bool> ToggleActiveStatusAsync(int id)
    {
        var evangelist = await _context.Evangelists.FindAsync(id);
        if (evangelist == null)
        {
            throw new InvalidOperationException($"전도자 ID {id}를 찾을 수 없습니다.");
        }

        evangelist.IsActive = !evangelist.IsActive;
        evangelist.UpdatedAt = DateTime.UtcNow;

        _context.Evangelists.Update(evangelist);
        await _context.SaveChangesAsync();

        _logger.LogInformation("전도자 활성 상태 변경: {Name} (ID: {Id}) → {IsActive}",
            evangelist.Name, id, evangelist.IsActive);

        return evangelist.IsActive;
    }

    public async Task UpdateSortOrderAsync(List<(int Id, int SortOrder)> sortOrders)
    {
        foreach (var (id, sortOrder) in sortOrders)
        {
            var evangelist = await _context.Evangelists.FindAsync(id);
            if (evangelist != null)
            {
                evangelist.SortOrder = sortOrder;
                evangelist.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("전도자 순서 변경 완료 (변경 건수: {Count})", sortOrders.Count);
    }
}
