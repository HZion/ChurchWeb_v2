using ChurchWeb.Core.Entities.News;
using ChurchWeb.Infrastructure.Services;
using ChurchWeb.Web.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChurchWeb.Web.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("Admin/Calendar")]
public class CalendarAdminController : Controller
{
    private readonly ICalendarAdminService _calendarService;
    private readonly ILogger<CalendarAdminController> _logger;

    public CalendarAdminController(
        ICalendarAdminService calendarService,
        ILogger<CalendarAdminController> logger)
    {
        _calendarService = calendarService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(
        int page = 1,
        string? searchTerm = null,
        int? year = null,
        int? month = null,
        string? eventType = null,
        bool? isVisible = null,
        string sortBy = "EventDate",
        bool sortDescending = false)
    {
        var (events, totalCount) = await _calendarService.GetPagedEventsAsync(
            page, 20, searchTerm, year, month, eventType, isVisible, sortBy, sortDescending);

        var availableYears = await _calendarService.GetAvailableYearsAsync();

        var model = new CalendarEventListViewModel
        {
            Events = events.Select(e => new CalendarEventItemViewModel
            {
                Id = e.Id,
                EventDate = e.EventDate,
                Title = e.Title,
                EventType = e.EventType,
                Time = e.Time,
                IsVisible = e.IsVisible
            }).ToList(),
            Pagination = new PaginationViewModel
            {
                CurrentPage = page,
                PageSize = 20,
                TotalCount = totalCount
            },
            Filter = new CalendarEventFilterViewModel
            {
                SearchTerm = searchTerm,
                Year = year,
                Month = month,
                EventType = eventType,
                IsVisible = isVisible,
                SortBy = sortBy,
                SortDescending = sortDescending
            },
            AvailableYears = availableYears
        };

        return View(model);
    }

    [HttpGet("Create")]
    public IActionResult Create()
    {
        var model = new CalendarEventFormViewModel();
        return View("Form", model);
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var calendarEvent = await _calendarService.GetEventByIdAsync(id);
        if (calendarEvent == null)
            return NotFound();

        var model = new CalendarEventFormViewModel
        {
            Id = calendarEvent.Id,
            Title = calendarEvent.Title,
            EventDate = calendarEvent.EventDate,
            EventType = calendarEvent.EventType,
            Time = calendarEvent.Time,
            Description = calendarEvent.Description,
            IsVisible = calendarEvent.IsVisible,
            SortOrder = calendarEvent.SortOrder
        };

        return View("Form", model);
    }

    [HttpPost("Save")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(CalendarEventFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Form", model);
        }

        try
        {
            if (model.Id == 0)
            {
                // Create
                var calendarEvent = new CalendarEvent
                {
                    Title = model.Title,
                    EventDate = model.EventDate,
                    EventType = model.EventType,
                    Time = model.Time,
                    Description = model.Description,
                    IsVisible = model.IsVisible,
                    SortOrder = model.SortOrder
                };

                await _calendarService.CreateEventAsync(calendarEvent);
                TempData["SuccessMessage"] = "일정이 성공적으로 등록되었습니다.";
                return RedirectToAction("Index");
            }
            else
            {
                // Update
                var calendarEvent = await _calendarService.GetEventByIdAsync(model.Id);
                if (calendarEvent == null)
                    return NotFound();

                calendarEvent.Title = model.Title;
                calendarEvent.EventDate = model.EventDate;
                calendarEvent.EventType = model.EventType;
                calendarEvent.Time = model.Time;
                calendarEvent.Description = model.Description;
                calendarEvent.IsVisible = model.IsVisible;
                calendarEvent.SortOrder = model.SortOrder;

                await _calendarService.UpdateEventAsync(calendarEvent);
                TempData["SuccessMessage"] = "일정이 성공적으로 수정되었습니다.";
                return RedirectToAction("Index");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving calendar event");
            ModelState.AddModelError(string.Empty, "저장 중 오류가 발생했습니다.");
            return View("Form", model);
        }
    }

    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _calendarService.DeleteEventAsync(id);
        if (result)
        {
            return Json(new { success = true, message = "일정이 삭제되었습니다." });
        }

        return Json(new { success = false, message = "일정을 찾을 수 없습니다." });
    }

    [HttpPost("ToggleVisibility/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleVisibility(int id)
    {
        var result = await _calendarService.ToggleVisibilityAsync(id);
        if (result)
        {
            return Json(new { success = true, message = "표시 여부가 변경되었습니다." });
        }

        return Json(new { success = false, message = "일정을 찾을 수 없습니다." });
    }
}
