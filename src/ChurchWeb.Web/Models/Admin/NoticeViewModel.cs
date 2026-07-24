using System.ComponentModel.DataAnnotations;

namespace ChurchWeb.Web.Models.Admin;

public class NoticeListViewModel
{
    public List<NoticeItemViewModel> Notices { get; set; } = new();
    public PaginationViewModel Pagination { get; set; } = new();
    public NoticeFilterViewModel Filter { get; set; } = new();
}

public class NoticeItemViewModel
{
    public int Id { get; set; }
    public string CategoryKey { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime PostedOn { get; set; }
    public int Views { get; set; }
    public bool IsPinned { get; set; }
    public bool IsVisible { get; set; }
    public int AttachmentCount { get; set; }
}

public class NoticeFilterViewModel
{
    public string? SearchTerm { get; set; }
    public string? CategoryKey { get; set; }
    public bool? IsPinned { get; set; }
    public bool? IsVisible { get; set; }
    public string SortBy { get; set; } = "PostedOn";
    public bool SortDescending { get; set; } = true;
}

public class NoticeFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "카테고리를 선택하세요.")]
    public string CategoryKey { get; set; } = "church";

    [Required(ErrorMessage = "제목을 입력하세요.")]
    [StringLength(200, ErrorMessage = "제목은 200자 이내로 입력하세요.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "작성자를 입력하세요.")]
    [StringLength(100, ErrorMessage = "작성자는 100자 이내로 입력하세요.")]
    public string Author { get; set; } = string.Empty;

    [Required(ErrorMessage = "작성일을 선택하세요.")]
    public DateTime PostedOn { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "내용을 입력하세요.")]
    public string BodyHtml { get; set; } = string.Empty;

    public bool IsPinned { get; set; }
    public bool IsVisible { get; set; } = true;
    public int SortOrder { get; set; }

    public List<NoticeAttachmentViewModel> Attachments { get; set; } = new();
}

public class NoticeAttachmentViewModel
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

public class AttachmentUploadViewModel
{
    [Required]
    public int NoticeId { get; set; }

    [Required(ErrorMessage = "파일을 선택하세요.")]
    public IFormFile AttachmentFile { get; set; } = null!;
}
