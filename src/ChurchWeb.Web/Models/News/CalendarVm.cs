namespace ChurchWeb.Web.Models.News;

public class CalendarVm
{
    public string PageTitle { get; set; } = "교회일정";
    public string PageSubtitle { get; set; } = "교회의 주요 일정을 확인하세요";
    public List<NewsTabVm> Tabs { get; set; } = new();

    public int Year { get; set; }
    public int Month { get; set; }

    // 달력 그리드용 (주 시작 일요일)
    public List<CalendarDay> Days { get; set; } = new();

    // 오늘의 일정
    public List<EventVm> TodayEvents { get; set; } = new();

    // 이달 전체 일정
    public List<EventVm> MonthEvents { get; set; } = new();
}
