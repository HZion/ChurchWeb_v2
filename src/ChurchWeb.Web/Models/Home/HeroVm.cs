namespace ChurchWeb.Web.Models.Home;

/// <summary>
/// 히어로 섹션 데이터 (슬라이드 목록 기반)
/// </summary>
public class HeroVm
{
    /// <summary>
    /// 히어로 슬라이드 목록 (관리자에서 슬라이드별 편집 가능)
    /// </summary>
    public List<HeroSlideVm> Slides { get; set; } = new();
}
