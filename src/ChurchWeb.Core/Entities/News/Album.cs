namespace ChurchWeb.Core.Entities.News;

/// <summary>
/// 갤러리 앨범
/// </summary>
public class Album
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string Category { get; set; } = string.Empty;      // 카테고리 (선택사항)
    public string Description { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    public int Year { get; set; }  // 연도별 필터용

    public bool IsVisible { get; set; } = true;
    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public ICollection<AlbumPhoto> Photos { get; set; } = new List<AlbumPhoto>();
}
