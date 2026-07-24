namespace ChurchWeb.Core.Entities.News;

/// <summary>
/// 앨범 사진
/// </summary>
public class AlbumPhoto
{
    public int Id { get; set; }

    public int AlbumId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;  // 사진 설명 (선택사항)
    public int SortOrder { get; set; }

    // Navigation
    public Album Album { get; set; } = null!;
}
