namespace ChurchWeb.Web.Models.Home;

/// <summary>
/// 히어로 슬라이드 단위 데이터 (관리자에서 개별 편집 가능)
/// </summary>
public class HeroSlideVm
{
    public string BackgroundType { get; set; } = "gradient";  // "image" | "gradient" | "color"
    public string ImageUrl { get; set; } = string.Empty;      // image일 때 (예: /uploads/hero/xxx.jpg)
    public string Background { get; set; } = string.Empty;     // gradient(CSS) 또는 color(#hex)일 때
    public double OverlayOpacity { get; set; } = 0.5;          // 사진 위 가독성용 오버레이 진하기(0~1)

    public string Kicker { get; set; } = string.Empty;         // "WELCOME TO OUR CHURCH"
    public string Title { get; set; } = string.Empty;          // 메인 제목 (줄바꿈은 \n 또는 <br>)
    public string Subtitle { get; set; } = string.Empty;       // 부제

    public string PrimaryBtnText { get; set; } = string.Empty;
    public string PrimaryBtnUrl { get; set; } = string.Empty;
    public string SecondaryBtnText { get; set; } = string.Empty;
    public string SecondaryBtnUrl { get; set; } = string.Empty;

    public bool IsVisible { get; set; } = true;
    public int SortOrder { get; set; } = 0;
}
