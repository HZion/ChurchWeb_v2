namespace ChurchWeb.Web.Models.About;

/// <summary>
/// 비전 카드 아이템 (추가/삭제/정렬 가능)
/// </summary>
public class VisionItem
{
    public string Label { get; set; } = string.Empty;    // "VISION 01"
    public string Title { get; set; } = string.Empty;    // "예배하는 교회"
    public string Desc { get; set; } = string.Empty;     // 설명
    public int SortOrder { get; set; }                   // 정렬 순서
}
