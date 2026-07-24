namespace ChurchWeb.Core.Entities.News;

/// <summary>
/// 소식 첨부파일
/// </summary>
public class NoticeAttachment
{
    public int Id { get; set; }

    public int NoticeId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    // Navigation
    public Notice Notice { get; set; } = null!;
}
