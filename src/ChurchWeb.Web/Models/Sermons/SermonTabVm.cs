namespace ChurchWeb.Web.Models.Sermons;

/// <summary>
/// 설교 서브 탭 메타데이터
/// </summary>
public class SermonTabVm
{
    public string Key { get; set; } = string.Empty;          // "sunday", "special"
    public string Title { get; set; } = string.Empty;        // "주일설교", "특별설교"
    public string Url { get; set; } = string.Empty;          // "/sermons/sunday"
    public bool IsVisible { get; set; }
    public int SortOrder { get; set; }
}
