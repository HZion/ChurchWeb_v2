using System.ComponentModel.DataAnnotations;

namespace ChurchWeb.Web.Models.Admin;

public class BulletinListViewModel
{
    public List<BulletinItemViewModel> Bulletins { get; set; } = new();
    public PaginationViewModel Pagination { get; set; } = new();
    public BulletinFilterViewModel Filter { get; set; } = new();
}

public class BulletinItemViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime PublishedDate { get; set; }
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FileSizeDisplay => FormatFileSize(FileSize);
    public bool IsVisible { get; set; }
    public int SortOrder { get; set; }

    private static string FormatFileSize(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
        return $"{bytes / (1024.0 * 1024.0):F1} MB";
    }
}

public class BulletinFilterViewModel
{
    public string? SearchTerm { get; set; }
    public bool? IsVisible { get; set; }
    public string SortBy { get; set; } = "PublishedDate";
    public bool SortDescending { get; set; } = true;
}

public class BulletinFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "제목을 입력하세요.")]
    [StringLength(200, ErrorMessage = "제목은 200자 이내로 입력하세요.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "발행일을 입력하세요.")]
    public DateTime PublishedDate { get; set; } = DateTime.Today;

    public IFormFile? PdfFile { get; set; }

    public string? ExistingFileName { get; set; }
    public long ExistingFileSize { get; set; }

    public bool IsVisible { get; set; } = true;

    [Range(0, int.MaxValue, ErrorMessage = "순서는 0 이상이어야 합니다.")]
    public int SortOrder { get; set; }
}
