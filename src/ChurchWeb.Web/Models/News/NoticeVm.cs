namespace ChurchWeb.Web.Models.News;

public class NoticeVm
{
    public int Id { get; set; }
    public string Category { get; set; } = string.Empty;     // "church" or "member"
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public int Views { get; set; }
    public bool IsPinned { get; set; }                       // 상단 고정 (필독)
}
