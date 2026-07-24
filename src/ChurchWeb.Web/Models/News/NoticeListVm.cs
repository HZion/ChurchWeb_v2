namespace ChurchWeb.Web.Models.News;

public class NoticeListVm
{
    public string PageTitle { get; set; } = "교회소식";
    public string PageSubtitle { get; set; } = "교회와 교우들의 소식을 전합니다";
    public List<NewsTabVm> Tabs { get; set; } = new();

    // 분류 필터 (전체/교회소식/교우소식)
    public List<NoticeCategory> Categories { get; set; } = new();

    public List<NoticeVm> Items { get; set; } = new();
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
}
