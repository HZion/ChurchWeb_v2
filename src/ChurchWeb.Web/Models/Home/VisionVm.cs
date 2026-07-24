namespace ChurchWeb.Web.Models.Home;

/// <summary>
/// 올해의 표어 섹션 데이터
/// </summary>
public class VisionVm
{
    public string Year { get; set; } = string.Empty;         // "2026"
    public string Motto { get; set; } = string.Empty;        // 표어 본문
    public List<PracticeItem> Practices { get; set; } = new();
}

public class PracticeItem
{
    public string Number { get; set; } = string.Empty;       // "01", "02", "03"
    public string Text { get; set; } = string.Empty;         // 실천 내용
}
