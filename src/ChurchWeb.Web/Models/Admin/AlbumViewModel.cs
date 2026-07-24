using System.ComponentModel.DataAnnotations;

namespace ChurchWeb.Web.Models.Admin;

public class AlbumListViewModel
{
    public List<AlbumItemViewModel> Albums { get; set; } = new();
    public PaginationViewModel Pagination { get; set; } = new();
    public AlbumFilterViewModel Filter { get; set; } = new();
    public List<int> AvailableYears { get; set; } = new();
}

public class AlbumItemViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string Category { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    public int PhotoCount { get; set; }
    public bool IsVisible { get; set; }
    public int SortOrder { get; set; }
}

public class AlbumFilterViewModel
{
    public string? SearchTerm { get; set; }
    public int? Year { get; set; }
    public bool? IsVisible { get; set; }
    public string SortBy { get; set; } = "EventDate";
    public bool SortDescending { get; set; } = true;
}

public class AlbumFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "제목을 입력하세요.")]
    [StringLength(200, ErrorMessage = "제목은 200자 이내로 입력하세요.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "이벤트 날짜를 입력하세요.")]
    public DateTime EventDate { get; set; } = DateTime.Today;

    [StringLength(100, ErrorMessage = "카테고리는 100자 이내로 입력하세요.")]
    public string Category { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "설명은 1000자 이내로 입력하세요.")]
    public string Description { get; set; } = string.Empty;

    public IFormFile? CoverImage { get; set; }
    public string? ExistingCoverImageUrl { get; set; }

    public bool IsVisible { get; set; } = true;

    [Range(0, int.MaxValue, ErrorMessage = "순서는 0 이상이어야 합니다.")]
    public int SortOrder { get; set; }

    // For photo management
    public List<AlbumPhotoViewModel> Photos { get; set; } = new();
}

public class AlbumPhotoViewModel
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

public class PhotoUploadViewModel
{
    [Required]
    public int AlbumId { get; set; }

    [Required(ErrorMessage = "이미지 파일을 선택하세요.")]
    public IFormFile PhotoFile { get; set; } = null!;

    [StringLength(500, ErrorMessage = "설명은 500자 이내로 입력하세요.")]
    public string Caption { get; set; } = string.Empty;
}
