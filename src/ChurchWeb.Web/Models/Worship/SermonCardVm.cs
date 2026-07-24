namespace ChurchWeb.Web.Models.Worship;

public class SermonCardVm
{
    public int Id { get; set; }
    public string Category { get; set; } = string.Empty;  // "sunday" 또는 "special"
    public string Verse { get; set; } = string.Empty;      // 본문 (예: "시편 16:5-6")
    public string Title { get; set; } = string.Empty;
    public string Preacher { get; set; } = string.Empty;   // 설교자
    public string Date { get; set; } = string.Empty;       // 설교일
    public string Duration { get; set; } = string.Empty;   // 재생시간 (예: "45:23")
    public string ThumbUrl { get; set; } = string.Empty;   // 썸네일 (유튜브 자동 추출 예정)
    public string YoutubeUrl { get; set; } = string.Empty; // 원본 URL
}
