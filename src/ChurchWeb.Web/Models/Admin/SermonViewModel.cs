using System.ComponentModel.DataAnnotations;

namespace ChurchWeb.Web.Models.Admin;

public class SermonListViewModel
{
    public List<SermonItemViewModel> Sermons { get; set; } = new();
    public PaginationViewModel Pagination { get; set; } = new();
    public SermonFilterViewModel Filter { get; set; } = new();
}

public class SermonItemViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Verse { get; set; } = string.Empty;
    public string Preacher { get; set; } = string.Empty;
    public DateTime PreachedOn { get; set; }
    public string Category { get; set; } = string.Empty;
    public string CategoryDisplay => Category == "sunday" ? "주일설교" : "특별설교";
    public bool IsVisible { get; set; }
    public int SortOrder { get; set; }
    public string? YoutubeUrl { get; set; }
}

public class SermonFilterViewModel
{
    public string? SearchTerm { get; set; }
    public string? Category { get; set; }
    public bool? IsVisible { get; set; }
    public string SortBy { get; set; } = "PreachedOn";
    public bool SortDescending { get; set; } = true;
}

public class PaginationViewModel
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
}

public class SermonFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "제목을 입력하세요.")]
    [StringLength(200, ErrorMessage = "제목은 200자 이내로 입력하세요.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "본문(성구)을 입력하세요.")]
    [StringLength(100, ErrorMessage = "본문은 100자 이내로 입력하세요.")]
    public string Verse { get; set; } = string.Empty;

    [Required(ErrorMessage = "설교자를 입력하세요.")]
    [StringLength(100, ErrorMessage = "설교자는 100자 이내로 입력하세요.")]
    public string Preacher { get; set; } = string.Empty;

    [Required(ErrorMessage = "설교일을 입력하세요.")]
    public DateTime PreachedOn { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "분류를 선택하세요.")]
    public string Category { get; set; } = "sunday";

    [Url(ErrorMessage = "올바른 URL 형식이 아닙니다.")]
    public string? YoutubeUrl { get; set; }

    public string? VideoDescription { get; set; }

    public string? Summary { get; set; }

    [RegularExpression(@"^\d{1,3}:\d{2}$", ErrorMessage = "재생시간은 mm:ss 형식으로 입력하세요. (예: 42:10)")]
    public string? Duration { get; set; }

    [Url(ErrorMessage = "올바른 URL 형식이 아닙니다.")]
    public string? ThumbnailUrl { get; set; }

    public bool IsVisible { get; set; } = true;

    [Range(0, int.MaxValue, ErrorMessage = "순서는 0 이상이어야 합니다.")]
    public int SortOrder { get; set; }
}
