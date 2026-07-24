namespace ChurchWeb.Web.Models.Worship;

public class SermonDetailVm
{
    // 기본 정보
    public int Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Verse { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Preacher { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string YoutubeUrl { get; set; } = string.Empty;

    // 상세 정보
    public string Description { get; set; } = string.Empty;  // 설교 요약
    public List<SermonAttachment> Attachments { get; set; } = new();
    public List<string> Tags { get; set; } = new();

    // 네비게이션
    public List<WorshipTabVm> Tabs { get; set; } = new();
    public SermonCardVm? Prev { get; set; }
    public SermonCardVm? Next { get; set; }
    public List<SermonCardVm> RelatedSermons { get; set; } = new();
}
