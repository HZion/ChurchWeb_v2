namespace ChurchWeb.Web.Models.About;

/// <summary>
/// 교회소개 > 오시는 길 페이지 ViewModel
/// </summary>
public class AboutLocationVm
{
    // ===== 페이지 메타데이터 =====
    public string PageTitle { get; set; } = string.Empty;
    public string PageSubtitle { get; set; } = string.Empty;

    // ===== 서브 탭 =====
    public List<AboutTabVm> Tabs { get; set; } = new();

    // ===== 지도 =====
    public string MapEmbed { get; set; } = string.Empty;     // 지도 임베드 HTML/좌표 (자리표시자)
    public double? Latitude { get; set; }   // 위도
    public double? Longitude { get; set; }  // 경도

    // ===== 교회 정보 =====
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // ===== 교통편 (버스, 지하철, 자가용) =====
    public List<TransportItem> Transport { get; set; } = new();
}
