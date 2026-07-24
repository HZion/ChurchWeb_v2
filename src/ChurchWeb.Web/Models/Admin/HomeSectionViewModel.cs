using System.ComponentModel.DataAnnotations;

namespace ChurchWeb.Web.Models.Admin;

public class HomeSectionViewModel
{
    public int Id { get; set; }

    [Display(Name = "섹션 키")]
    public string Key { get; set; } = string.Empty;

    [Display(Name = "섹션 제목")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "표시 여부")]
    public bool IsVisible { get; set; } = true;

    [Display(Name = "정렬 순서")]
    public int SortOrder { get; set; }
}
