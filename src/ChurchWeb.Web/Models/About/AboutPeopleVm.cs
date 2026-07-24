namespace ChurchWeb.Web.Models.About;

/// <summary>
/// 교회소개 > 섬기는 사람들 페이지 ViewModel
/// </summary>
public class AboutPeopleVm
{
    // ===== 페이지 메타데이터 =====
    public string PageTitle { get; set; } = string.Empty;
    public string PageSubtitle { get; set; } = string.Empty;

    // ===== 서브 탭 =====
    public List<AboutTabVm> Tabs { get; set; } = new();

    // ===== 담임목사 대표 카드 =====
    public LeadPastorVm LeadPastor { get; set; } = new();

    // ===== 필터 탭 (전체, 교역자, 장로, 안수집사, 권사) =====
    public List<PeopleCategory> Categories { get; set; } = new();

    // ===== 인물 카드들 =====
    public List<PersonVm> People { get; set; } = new();
}
