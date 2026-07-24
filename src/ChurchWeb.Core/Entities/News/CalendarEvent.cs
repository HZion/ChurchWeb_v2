namespace ChurchWeb.Core.Entities.News;

/// <summary>
/// 교회 일정
/// </summary>
public class CalendarEvent
{
    public int Id { get; set; }

    public DateTime EventDate { get; set; }
    public string Title { get; set; } = string.Empty;
    public string EventType { get; set; } = "event";  // "worship" | "event"
    public string Time { get; set; } = string.Empty;  // 시간 (선택사항)
    public string Description { get; set; } = string.Empty;

    public bool IsVisible { get; set; } = true;
    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
