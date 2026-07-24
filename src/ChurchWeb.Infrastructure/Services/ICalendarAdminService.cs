using ChurchWeb.Core.Entities.News;

namespace ChurchWeb.Infrastructure.Services;

public interface ICalendarAdminService
{
    // Calendar Event CRUD
    Task<(IEnumerable<CalendarEvent> events, int totalCount)> GetPagedEventsAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        int? year = null,
        int? month = null,
        string? eventType = null,
        bool? isVisible = null,
        string sortBy = "EventDate",
        bool sortDescending = false);

    Task<CalendarEvent?> GetEventByIdAsync(int id);
    Task CreateEventAsync(CalendarEvent calendarEvent);
    Task UpdateEventAsync(CalendarEvent calendarEvent);
    Task<bool> DeleteEventAsync(int id);
    Task<bool> ToggleVisibilityAsync(int id);

    // Utility
    Task<List<int>> GetAvailableYearsAsync();
    Task<List<int>> GetAvailableMonthsAsync(int year);
}
