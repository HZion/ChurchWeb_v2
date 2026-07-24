namespace ChurchWeb.Web.Models.Home;

/// <summary>
/// 홈 섹션 메타데이터 (표시/숨김, 순서 제어)
/// </summary>
public class HomeSectionVm
{
    public string Key { get; set; } = string.Empty;      // "hero", "vision", "sermons", ...
    public string Title { get; set; } = string.Empty;    // 관리자용 표시 이름
    public bool IsVisible { get; set; }                  // On/Off
    public int SortOrder { get; set; }                   // 표시 순서
}
