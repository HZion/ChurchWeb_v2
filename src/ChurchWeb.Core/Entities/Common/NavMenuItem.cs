namespace ChurchWeb.Core.Entities.Common;

/// <summary>
/// 네비게이션 메뉴 항목
/// </summary>
public class NavMenuItem
{
    public int Id { get; set; }

    /// <summary>
    /// 메뉴 제목
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 메뉴 URL
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// 부모 메뉴 ID (null이면 최상위 메뉴)
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// 정렬 순서
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 표시 여부
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// 아이콘 클래스 (Bootstrap Icons)
    /// </summary>
    public string? IconClass { get; set; }

    /// <summary>
    /// 새 창으로 열기 여부
    /// </summary>
    public bool OpenInNewTab { get; set; } = false;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // 네비게이션 속성
    public NavMenuItem? Parent { get; set; }
    public ICollection<NavMenuItem> Children { get; set; } = new List<NavMenuItem>();
}
