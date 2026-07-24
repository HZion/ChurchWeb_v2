namespace ChurchWeb.Web.Models.News;

public class GalleryListVm
{
    public string PageTitle { get; set; } = "갤러리";
    public string PageSubtitle { get; set; } = "교회의 다양한 활동을 사진으로 만나보세요";
    public List<NewsTabVm> Tabs { get; set; } = new();

    // 연도 필터 (데이터 기반)
    public List<int> Years { get; set; } = new();
    public int? SelectedYear { get; set; }

    public List<AlbumCardVm> Albums { get; set; } = new();
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
}
