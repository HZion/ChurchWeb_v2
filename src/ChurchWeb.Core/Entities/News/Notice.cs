namespace ChurchWeb.Core.Entities.News;

/// <summary>
/// 교회/교우 소식
/// </summary>
public class Notice
{
    public int Id { get; set; }

    public string CategoryKey { get; set; } = "church";  // "church" | "member"
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime PostedOn { get; set; }
    public int Views { get; set; }
    public bool IsPinned { get; set; }  // 필독 여부
    public string BodyHtml { get; set; } = string.Empty;  // HTML 본문

    public bool IsVisible { get; set; } = true;
    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public ICollection<NoticeAttachment> Attachments { get; set; } = new List<NoticeAttachment>();
}
