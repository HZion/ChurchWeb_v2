namespace ChurchWeb.Web.Models.News;

public class BulletinDetailVm
{
    public string PageTitle { get; set; } = "주보";
    public string PageSubtitle { get; set; } = "주일 예배 주보를 확인하세요";
    public List<NewsTabVm> Tabs { get; set; } = new();

    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string RegDate { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;

    // 주보 각 면 이미지 (다중 페이지)
    public List<string> PageImages { get; set; } = new();

    // 이전/다음 네비게이션
    public BulletinCardVm? Prev { get; set; }
    public BulletinCardVm? Next { get; set; }
}
