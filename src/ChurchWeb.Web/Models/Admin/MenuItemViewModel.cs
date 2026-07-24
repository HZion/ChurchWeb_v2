using System.ComponentModel.DataAnnotations;

namespace ChurchWeb.Web.Models.Admin;

public class MenuItemViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "메뉴 제목을 입력하세요")]
    [Display(Name = "메뉴 제목")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "URL을 입력하세요")]
    [Display(Name = "URL")]
    public string Url { get; set; } = string.Empty;

    [Display(Name = "부모 메뉴")]
    public int? ParentId { get; set; }

    [Display(Name = "정렬 순서")]
    public int SortOrder { get; set; }

    [Display(Name = "표시 여부")]
    public bool IsVisible { get; set; } = true;

    [Display(Name = "아이콘 클래스")]
    public string? IconClass { get; set; }

    [Display(Name = "새 창으로 열기")]
    public bool OpenInNewTab { get; set; } = false;

    // 표시용
    public string? ParentTitle { get; set; }
    public int ChildCount { get; set; }
}

public class MenuListViewModel
{
    public List<MenuItemViewModel> MenuItems { get; set; } = new();
}
