namespace ChurchWeb.Web.Models.Home;

/// <summary>
/// 교회 소식 섹션 데이터
/// </summary>
public class NewsVm
{
    public string SectionEyebrow { get; set; } = "NEWS & EVENTS";
    public string SectionTitle { get; set; } = "교회 소식";
    public List<NoticeItem> Notices { get; set; } = new();
    public List<EventItem> Events { get; set; } = new();
}

public class NoticeItem
{
    public bool IsPinned { get; set; }                       // 필독 여부
    public string Title { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;         // "07.15"
    public string Url { get; set; } = "#";
}

public class EventItem
{
    public string Title { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty; // 행사 썸네일
    public string Url { get; set; } = "#";
}
