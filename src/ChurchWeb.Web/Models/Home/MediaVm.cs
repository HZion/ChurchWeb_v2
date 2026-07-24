namespace ChurchWeb.Web.Models.Home;

/// <summary>
/// 교회 소개 영상 섹션 데이터
/// </summary>
public class MediaVm
{
    public string SectionEyebrow { get; set; } = "CHURCH FILM";
    public string SectionTitle { get; set; } = "교회 소개 영상";
    public string SectionDescription { get; set; } = string.Empty;
    public string YouTubeUrl { get; set; } = string.Empty;   // 유튜브 임베드 URL
    public string Caption { get; set; } = "YouTube 임베드 영역 · 16:9";
}
