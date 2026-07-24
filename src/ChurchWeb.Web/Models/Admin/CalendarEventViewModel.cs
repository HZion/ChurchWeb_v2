using System.ComponentModel.DataAnnotations;

namespace ChurchWeb.Web.Models.Admin;

public class CalendarEventListViewModel
{
    public List<CalendarEventItemViewModel> Events { get; set; } = new();
    public PaginationViewModel Pagination { get; set; } = new();
    public CalendarEventFilterViewModel Filter { get; set; } = new();
    public List<int> AvailableYears { get; set; } = new();
}

public class CalendarEventItemViewModel
{
    public int Id { get; set; }
    public DateTime EventDate { get; set; }
    public string Title { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public bool IsVisible { get; set; }
}

public class CalendarEventFilterViewModel
{
    public string? SearchTerm { get; set; }
    public int? Year { get; set; }
    public int? Month { get; set; }
    public string? EventType { get; set; }
    public bool? IsVisible { get; set; }
    public string SortBy { get; set; } = "EventDate";
    public bool SortDescending { get; set; } = false;
}

public class CalendarEventFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "일정 제목을 입력하세요.")]
    [StringLength(200, ErrorMessage = "제목은 200자 이내로 입력하세요.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "일정 날짜를 선택하세요.")]
    public DateTime EventDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "일정 유형을 선택하세요.")]
    public string EventType { get; set; } = "event";

    [StringLength(50, ErrorMessage = "시간은 50자 이내로 입력하세요.")]
    public string Time { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "설명은 1000자 이내로 입력하세요.")]
    public string Description { get; set; } = string.Empty;

    public bool IsVisible { get; set; } = true;
    public int SortOrder { get; set; }
}
