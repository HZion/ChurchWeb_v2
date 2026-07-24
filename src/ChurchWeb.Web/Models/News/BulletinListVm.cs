namespace ChurchWeb.Web.Models.News;

public class BulletinListVm
{
    public string PageTitle { get; set; } = "주보";
    public string PageSubtitle { get; set; } = "주일 예배 주보를 확인하세요";
    public List<NewsTabVm> Tabs { get; set; } = new();

    public List<BulletinCardVm> Items { get; set; } = new();
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
}
