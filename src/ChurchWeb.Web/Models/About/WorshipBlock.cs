namespace ChurchWeb.Web.Models.About;

/// <summary>
/// 예배 블록 (정기 예배, 교육부서 예배 등)
/// </summary>
public class WorshipBlock
{
    public string Title { get; set; } = string.Empty;       // "정기 예배", "교육부서 예배"
    public bool IsVisible { get; set; }                      // 블록 숨김 제어
    public int SortOrder { get; set; }                       // 정렬 순서
    public List<WorshipRow> Rows { get; set; } = new();      // 테이블 행들
}
