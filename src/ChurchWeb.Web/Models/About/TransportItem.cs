namespace ChurchWeb.Web.Models.About;

/// <summary>
/// 교통편 안내 항목
/// </summary>
public class TransportItem
{
    public string Icon { get; set; } = string.Empty;         // "🚌", "🚇", "🚗"
    public string Title { get; set; } = string.Empty;        // "버스"
    public string Body { get; set; } = string.Empty;         // 경로 설명
    public int SortOrder { get; set; }
}
