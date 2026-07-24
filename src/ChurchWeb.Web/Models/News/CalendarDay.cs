namespace ChurchWeb.Web.Models.News;

public class CalendarDay
{
    public DateTime Date { get; set; }
    public bool IsCurrentMonth { get; set; }
    public bool IsToday { get; set; }
    public bool IsSunday { get; set; }
    public bool IsSaturday { get; set; }
    public List<EventVm> Events { get; set; } = new();
}
