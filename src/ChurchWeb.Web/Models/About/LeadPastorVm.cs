namespace ChurchWeb.Web.Models.About;

/// <summary>
/// 담임목사 대표 카드
/// </summary>
public class LeadPastorVm
{
    public string Name { get; set; } = string.Empty;         // "홍길동"
    public string Title { get; set; } = string.Empty;        // "담임목사"
    public string Quote { get; set; } = string.Empty;        // 인용 문구
    public string Desc { get; set; } = string.Empty;         // 설명
    public string Photo { get; set; } = string.Empty;        // 사진 URL
}
