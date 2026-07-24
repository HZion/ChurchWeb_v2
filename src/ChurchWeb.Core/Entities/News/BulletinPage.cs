namespace ChurchWeb.Core.Entities.News;

/// <summary>
/// 주보 페이지 (이미지)
/// </summary>
public class BulletinPage
{
    public int Id { get; set; }

    public int BulletinId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    // Navigation
    public Bulletin Bulletin { get; set; } = null!;
}
