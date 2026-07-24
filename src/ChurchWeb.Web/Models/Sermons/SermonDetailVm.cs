namespace ChurchWeb.Web.Models.Sermons;

/// <summary>
/// 설교 상세 페이지 ViewModel
/// </summary>
public class SermonDetailVm
{
    // ===== 현재 설교 =====
    public SermonCardVm Sermon { get; set; } = new();
    public string Summary { get; set; } = string.Empty;      // 설교 개요 (선택)

    // ===== 이전/다음 (없으면 null) =====
    public SermonCardVm? Prev { get; set; }
    public SermonCardVm? Next { get; set; }

    // ===== 관련 설교 (같은 분류 최근 3개) =====
    public List<SermonCardVm> Related { get; set; } = new();
}
