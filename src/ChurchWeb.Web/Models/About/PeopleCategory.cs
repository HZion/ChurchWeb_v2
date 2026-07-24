namespace ChurchWeb.Web.Models.About;

/// <summary>
/// 섬기는 사람들 필터 카테고리
/// </summary>
public class PeopleCategory
{
    public string Key { get; set; } = string.Empty;          // "all", "pastor", "elder", ...
    public string Label { get; set; } = string.Empty;        // "전체", "교역자", ...
    public bool IsVisible { get; set; }
    public int SortOrder { get; set; }
}
