namespace ChurchWeb.Web.Models.Home;

/// <summary>
/// 바로가기 카드 섹션 데이터
/// </summary>
public class QuickVm
{
    public QuickCard About { get; set; } = new();
    public BulletinCard Bulletin { get; set; } = new();
    public QuickCard Worship { get; set; } = new();
    public QuickCard Sermons { get; set; } = new();  // 설교 말씀 (교회학교 대체)
    public QuickCard Location { get; set; } = new();
}

public class QuickCard
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string LinkUrl { get; set; } = "#";
}

public class BulletinCard
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ViewUrl { get; set; } = "#";
    public string DownloadUrl { get; set; } = "#";
    public string ThumbnailUrl { get; set; } = string.Empty;  // 주보 이미지
}
