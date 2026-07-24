namespace ChurchWeb.Core.Entities.News;

/// <summary>
/// 주보
/// </summary>
public class Bulletin
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public DateTime PublishedDate { get; set; }

    // PDF 파일 데이터 (DB 저장)
    public byte[] PdfData { get; set; } = Array.Empty<byte>();
    public string FileName { get; set; } = string.Empty;  // 원본 파일명 (예: "2024-01-07.pdf")
    public long FileSize { get; set; }  // 파일 크기 (바이트)
    public string ContentType { get; set; } = "application/pdf";

    public string FileUrl { get; set; } = string.Empty;       // PDF 다운로드 URL (deprecated, 하위 호환성)
    public string CoverImageUrl { get; set; } = string.Empty; // 표지 이미지

    public bool IsVisible { get; set; } = true;
    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public ICollection<BulletinPage> Pages { get; set; } = new List<BulletinPage>();
}
