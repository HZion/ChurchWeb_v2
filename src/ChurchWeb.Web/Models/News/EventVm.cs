namespace ChurchWeb.Web.Models.News;

public class EventVm
{
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;         // "worship" or "event"
    public string? Time { get; set; }
    public DateTime Date { get; set; }
}
