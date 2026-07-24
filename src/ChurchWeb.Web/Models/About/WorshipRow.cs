namespace ChurchWeb.Web.Models.About;

/// <summary>
/// 예배 시간표 행
/// </summary>
public class WorshipRow
{
    public string Name { get; set; } = string.Empty;         // "주일 1부 예배"
    public string Time { get; set; } = string.Empty;         // "매주 일요일 오전 9:00"
    public string Place { get; set; } = string.Empty;        // "본당"
    public string Note { get; set; } = string.Empty;         // "실시간 온라인 중계" (선택)
    public int SortOrder { get; set; }                       // 정렬 순서
}
