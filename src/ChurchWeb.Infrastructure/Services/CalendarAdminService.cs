using ChurchWeb.Core.Entities.News;
using ChurchWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChurchWeb.Infrastructure.Services;

public class CalendarAdminService : ICalendarAdminService
{
    private readonly AppDbContext _context;
    private readonly ILogger<CalendarAdminService> _logger;

    public CalendarAdminService(
        AppDbContext context,
        ILogger<CalendarAdminService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<(IEnumerable<CalendarEvent> events, int totalCount)> GetPagedEventsAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        int? year = null,
        int? month = null,
        string? eventType = null,
        bool? isVisible = null,
        string sortBy = "EventDate",
        bool sortDescending = false)
    {
        var query = _context.CalendarEvents.AsQueryable();

        // Filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(e => e.Title.Contains(searchTerm) || e.Description.Contains(searchTerm));
        }

        if (year.HasValue)
        {
            query = query.Where(e => e.EventDate.Year == year.Value);
        }

        if (month.HasValue)
        {
            query = query.Where(e => e.EventDate.Month == month.Value);
        }

        if (!string.IsNullOrWhiteSpace(eventType))
        {
            query = query.Where(e => e.EventType == eventType);
        }

        if (isVisible.HasValue)
        {
            query = query.Where(e => e.IsVisible == isVisible.Value);
        }

        // Total count
        var totalCount = await query.CountAsync();

        // Sort
        query = sortBy switch
        {
            "Title" => sortDescending ? query.OrderByDescending(e => e.Title) : query.OrderBy(e => e.Title),
            "EventType" => sortDescending ? query.OrderByDescending(e => e.EventType) : query.OrderBy(e => e.EventType),
            "EventDate" => sortDescending ? query.OrderByDescending(e => e.EventDate) : query.OrderBy(e => e.EventDate),
            _ => sortDescending ? query.OrderByDescending(e => e.EventDate) : query.OrderBy(e => e.EventDate)
        };

        // Paging
        var events = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (events, totalCount);
    }

    public async Task<CalendarEvent?> GetEventByIdAsync(int id)
    {
        return await _context.CalendarEvents.FindAsync(id);
    }

    public async Task CreateEventAsync(CalendarEvent calendarEvent)
    {
        // Auto-increment sort order if not set
        if (calendarEvent.SortOrder == 0)
        {
            var maxSortOrder = await _context.CalendarEvents
                .Where(e => e.EventDate.Date == calendarEvent.EventDate.Date)
                .MaxAsync(e => (int?)e.SortOrder) ?? 0;
            calendarEvent.SortOrder = maxSortOrder + 1;
        }

        calendarEvent.CreatedAt = DateTime.UtcNow;
        calendarEvent.UpdatedAt = DateTime.UtcNow;

        _context.CalendarEvents.Add(calendarEvent);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateEventAsync(CalendarEvent calendarEvent)
    {
        calendarEvent.UpdatedAt = DateTime.UtcNow;

        _context.CalendarEvents.Update(calendarEvent);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteEventAsync(int id)
    {
        var calendarEvent = await GetEventByIdAsync(id);
        if (calendarEvent == null)
            return false;

        _context.CalendarEvents.Remove(calendarEvent);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ToggleVisibilityAsync(int id)
    {
        var calendarEvent = await GetEventByIdAsync(id);
        if (calendarEvent == null)
            return false;

        calendarEvent.IsVisible = !calendarEvent.IsVisible;
        calendarEvent.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    // Utility

    public async Task<List<int>> GetAvailableYearsAsync()
    {
        return await _context.CalendarEvents
            .Select(e => e.EventDate.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToListAsync();
    }

    public async Task<List<int>> GetAvailableMonthsAsync(int year)
    {
        return await _context.CalendarEvents
            .Where(e => e.EventDate.Year == year)
            .Select(e => e.EventDate.Month)
            .Distinct()
            .OrderBy(m => m)
            .ToListAsync();
    }
}
