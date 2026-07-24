namespace ChurchWeb.Core.Entities.Home;

/// <summary>
/// 담임목사 인사말 (단일 레코드)
/// </summary>
public class PastorGreeting
{
    public int Id { get; set; }

    public string Role { get; set; } = string.Empty;      // "홍길동 담임 목사"
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;     // "주님의 이름으로\n환영하고 축복합니다."
    public string Message { get; set; } = string.Empty;   // 본문
    public string PhotoUrl { get; set; } = string.Empty;
    public string MoreUrl { get; set; } = string.Empty;   // "인사말 더보기" 링크

    public bool IsVisible { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
