namespace ChurchWeb.Web.Models.Home;

/// <summary>
/// 말씀과 찬양 섹션 데이터
/// </summary>
public class SermonsVm
{
    public string SectionEyebrow { get; set; } = "SERMONS";
    public string SectionTitle { get; set; } = "말씀과 찬양";
    public string SectionDescription { get; set; } = string.Empty;
    public List<SermonCard> Sermons { get; set; } = new();
}

public class SermonCard
{
    public string Category { get; set; } = string.Empty;     // "주일설교", "특별설교"
    public string Verse { get; set; } = string.Empty;        // "시편 23:1"
    public string Title { get; set; } = string.Empty;        // 설교 제목
    public string Preacher { get; set; } = string.Empty;     // "홍길동 목사"
    public string Date { get; set; } = string.Empty;         // "2026.07.12"
    public string Duration { get; set; } = string.Empty;     // "42:10"
    public string ThumbnailUrl { get; set; } = string.Empty; // 썸네일 URL (자리표시자)
    public string VideoUrl { get; set; } = string.Empty;     // 유튜브 URL
}
