namespace ChurchWeb.Core.Entities.Home;

/// <summary>
/// 홈 화면 바로가기 버튼
/// </summary>
public class QuickLink
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;      // 버튼 텍스트
    public string Icon { get; set; } = string.Empty;        // 아이콘 (이모지 또는 클래스명)
    public string Url { get; set; } = string.Empty;         // 링크 URL
    public string Description { get; set; } = string.Empty; // 설명 (선택사항)

    public bool IsVisible { get; set; } = true;
    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
