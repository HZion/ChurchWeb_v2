namespace ChurchWeb.Web.Models.About;

/// <summary>
/// 섬기는 사람 카드
/// </summary>
public class PersonVm
{
    public string Category { get; set; } = string.Empty;     // "pastor", "elder", "deacon", ...
    public string Role { get; set; } = string.Empty;         // "부목사", "장로", ...
    public string Name { get; set; } = string.Empty;
    public string Ministry { get; set; } = string.Empty;     // "교육부서" (선택)
    public string Photo { get; set; } = string.Empty;
    public bool IsVisible { get; set; }
    public int SortOrder { get; set; }
}
