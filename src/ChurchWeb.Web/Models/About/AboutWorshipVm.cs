namespace ChurchWeb.Web.Models.About;

/// <summary>
/// 교회소개 > 예배 안내 페이지 ViewModel
/// </summary>
public class AboutWorshipVm
{
    // ===== 페이지 메타데이터 =====
    public string PageTitle { get; set; } = string.Empty;
    public string PageSubtitle { get; set; } = string.Empty;

    // ===== 서브 탭 =====
    public List<AboutTabVm> Tabs { get; set; } = new();

    // ===== 인트로 =====
    public string IntroEyebrow { get; set; } = string.Empty;    // "WORSHIP"
    public string IntroTitle { get; set; } = string.Empty;      // "함께 모여 드리는 예배"
    public string IntroText { get; set; } = string.Empty;       // 설명

    // ===== 예배 블록들 (블록 추가/삭제/순서 제어) =====
    public List<WorshipBlock> Blocks { get; set; } = new();

    // ===== 온라인 예배 밴드 (블록 숨김 가능) =====
    public bool OnlineVisible { get; set; }
    public string OnlineTitle { get; set; } = string.Empty;
    public string OnlineText { get; set; } = string.Empty;
    public string OnlineUrl { get; set; } = string.Empty;
}
