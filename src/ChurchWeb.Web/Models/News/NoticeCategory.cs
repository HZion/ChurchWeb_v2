namespace ChurchWeb.Web.Models.News;

public class NoticeCategory
{
    public string Key { get; set; } = string.Empty;          // "all", "church", "member"
    public string Label { get; set; } = string.Empty;
    public bool IsVisible { get; set; } = true;
    public int SortOrder { get; set; }
}
