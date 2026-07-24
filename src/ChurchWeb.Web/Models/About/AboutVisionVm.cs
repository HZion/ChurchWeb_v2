namespace ChurchWeb.Web.Models.About;

/// <summary>
/// 교회소개 > 사명과 비전 페이지 ViewModel
/// </summary>
public class AboutVisionVm
{
    // ===== 페이지 메타데이터 =====
    public string PageTitle { get; set; } = string.Empty;        // "사명과 비전"
    public string PageSubtitle { get; set; } = string.Empty;     // 배너 부제

    // ===== 서브 탭 (6개, 데이터 기반) =====
    public List<AboutTabVm> Tabs { get; set; } = new();

    // ===== 사명 섹션 =====
    public string MissionEyebrow { get; set; } = string.Empty;   // "OUR MISSION"
    public string MissionLead { get; set; } = string.Empty;      // 큰 리드 문장 (HTML 포함 가능)
    public List<string> MissionBody { get; set; } = new();       // 본문 문단들

    // ===== 비전 카드 (3개 기본, 추가/삭제/정렬 가능) =====
    public List<VisionItem> VisionItems { get; set; } = new();

    // ===== 핵심 가치 (4개 기본, 추가/삭제/정렬 가능) =====
    public List<ValueItem> ValueItems { get; set; } = new();

    // ===== 표어 밴드 =====
    public string MottoYear { get; set; } = string.Empty;        // "2026"
    public string MottoText { get; set; } = string.Empty;        // 표어 문구
    public string ScriptureRef { get; set; } = string.Empty;     // 성경 구절 출처
}
