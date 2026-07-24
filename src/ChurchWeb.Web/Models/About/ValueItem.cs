namespace ChurchWeb.Web.Models.About;

/// <summary>
/// 핵심 가치 아이템 (추가/삭제/정렬 가능)
/// </summary>
public class ValueItem
{
    public string No { get; set; } = string.Empty;       // "01", "02", ...
    public string Title { get; set; } = string.Empty;    // "말씀 중심"
    public string Desc { get; set; } = string.Empty;     // 설명
    public int SortOrder { get; set; }                   // 정렬 순서
}
