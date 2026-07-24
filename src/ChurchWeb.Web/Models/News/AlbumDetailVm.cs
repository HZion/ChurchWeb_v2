namespace ChurchWeb.Web.Models.News;

public class AlbumDetailVm
{
    public string PageTitle { get; set; } = "갤러리";
    public string PageSubtitle { get; set; } = "교회의 다양한 활동을 사진으로 만나보세요";
    public List<NewsTabVm> Tabs { get; set; } = new();

    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // 앨범 내 사진들 (여러 장)
    public List<string> Photos { get; set; } = new();

    // 이전/다음 네비게이션
    public AlbumCardVm? Prev { get; set; }
    public AlbumCardVm? Next { get; set; }
}
