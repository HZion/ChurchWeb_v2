namespace ChurchWeb.Web.Models.About;

/// <summary>
/// 교회소개 서브 탭 메타데이터 (표시/숨김, 순서 제어)
/// </summary>
public class AboutTabVm
{
    public string Key { get; set; } = string.Empty;      // "vision", "worship", "pastors", ...
    public string Title { get; set; } = string.Empty;    // "사명과 비전", "예배 안내", ...
    public string Url { get; set; } = string.Empty;      // "/about/vision", "/about/worship", ...
    public bool IsVisible { get; set; }                  // On/Off
    public int SortOrder { get; set; }                   // 표시 순서
}
