namespace ChurchWeb.Web.Models.Sermons;

/// <summary>
/// 설교 카드 (목록 및 상세에서 공통 사용)
/// </summary>
public class SermonCardVm
{
    public int Id { get; set; }
    public string Category { get; set; } = string.Empty;     // "주일설교", "특별설교"
    public string Verse { get; set; } = string.Empty;        // "시편 23:1"
    public string Title { get; set; } = string.Empty;
    public string Preacher { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;         // "2026.07.12"
    public string Duration { get; set; } = string.Empty;     // "42:10"
    public string ThumbUrl { get; set; } = string.Empty;     // 썸네일 (유튜브 자동 또는 수동)
    public string YoutubeUrl { get; set; } = string.Empty;   // 유튜브 임베드용 ID
}
