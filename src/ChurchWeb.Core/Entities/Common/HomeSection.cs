namespace ChurchWeb.Core.Entities.Common;

/// <summary>
/// 홈 섹션 표시/순서 제어
/// </summary>
public class HomeSection
{
    public int Id { get; set; }

    public string Key { get; set; } = string.Empty;  // "hero", "vision", "sermons", "quick" 등
    public string Title { get; set; } = string.Empty;  // 관리자 표시용

    public bool IsVisible { get; set; } = true;
    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
