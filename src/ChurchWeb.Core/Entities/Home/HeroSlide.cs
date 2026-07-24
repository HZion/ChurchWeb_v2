namespace ChurchWeb.Core.Entities.Home;

/// <summary>
/// 히어로 슬라이더
/// </summary>
public class HeroSlide
{
    public int Id { get; set; }

    // 배경 설정
    public string BackgroundType { get; set; } = "gradient";  // "image" | "gradient" | "color"
    public string ImageUrl { get; set; } = string.Empty;      // BackgroundType이 "image"일 때
    public string Background { get; set; } = string.Empty;     // BackgroundType이 "gradient" 또는 "color"일 때 CSS
    public double OverlayOpacity { get; set; } = 0.5;          // 사진 위 오버레이 진하기 (0~1)

    // 콘텐츠
    public string Kicker { get; set; } = string.Empty;         // 상단 작은 텍스트
    public string Title { get; set; } = string.Empty;          // 메인 제목 (HTML 허용: <br> 등)
    public string Subtitle { get; set; } = string.Empty;       // 부제

    // 버튼
    public string PrimaryBtnText { get; set; } = string.Empty;
    public string PrimaryBtnUrl { get; set; } = string.Empty;
    public string SecondaryBtnText { get; set; } = string.Empty;
    public string SecondaryBtnUrl { get; set; } = string.Empty;

    // 표시 제어
    public bool IsVisible { get; set; } = true;
    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
