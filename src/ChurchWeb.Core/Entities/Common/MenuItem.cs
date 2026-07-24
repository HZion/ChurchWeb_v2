namespace ChurchWeb.Core.Entities.Common;

/// <summary>
/// 메가 헤더 메뉴 구조 (계층형)
/// </summary>
public class MenuItem
{
    public int Id { get; set; }

    public string Key { get; set; } = string.Empty;  // "about", "sermons", "news", "about-vision" 등
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;

    // 계층 구조
    public string? ParentKey { get; set; }  // null이면 최상위 메뉴

    // 표시 제어
    public bool IsVisible { get; set; } = true;
    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
