namespace ChurchWeb.Core.Entities.Sermons;

/// <summary>
/// 설교
/// </summary>
public class Sermon
{
    public int Id { get; set; }

    public string Category { get; set; } = "sunday";  // "sunday" | "special"
    public string Verse { get; set; } = string.Empty;         // 본문
    public string Title { get; set; } = string.Empty;
    public string Preacher { get; set; } = string.Empty;
    public DateTime PreachedOn { get; set; }
    public string Duration { get; set; } = string.Empty;      // "42:10"
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string YoutubeUrl { get; set; } = string.Empty;
    public string VideoDescription { get; set; } = string.Empty;  // YouTube 동영상 설명
    public string Summary { get; set; } = string.Empty;       // 요약 (선택사항)

    public bool IsVisible { get; set; } = true;
    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
