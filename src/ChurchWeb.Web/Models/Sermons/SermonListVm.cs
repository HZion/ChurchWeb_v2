namespace ChurchWeb.Web.Models.Sermons;

/// <summary>
/// 설교 목록 페이지 ViewModel (주일설교/특별설교 공통)
/// </summary>
public class SermonListVm
{
    // ===== 페이지 메타데이터 =====
    public string Category { get; set; } = string.Empty;     // "sunday" | "special"
    public string PageTitle { get; set; } = string.Empty;    // "주일설교" | "특별설교"
    public string PageSubtitle { get; set; } = string.Empty;

    // ===== 서브 탭 =====
    public List<SermonTabVm> Tabs { get; set; } = new();

    // ===== 설교 목록 =====
    public int TotalCount { get; set; }
    public List<SermonCardVm> Sermons { get; set; } = new();

    // ===== 페이지네이션 =====
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
