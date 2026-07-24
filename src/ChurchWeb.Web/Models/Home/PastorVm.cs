namespace ChurchWeb.Web.Models.Home;

/// <summary>
/// 담임목사 인사말 섹션 데이터
/// </summary>
public class PastorVm
{
    public string PhotoUrl { get; set; } = string.Empty;     // 목사 사진
    public string Role { get; set; } = string.Empty;         // "홍길동 담임 목사"
    public string Title { get; set; } = string.Empty;        // "주님의 이름으로 환영하고 축복합니다."
    public string Message { get; set; } = string.Empty;      // 인사말 본문
    public string MoreUrl { get; set; } = "#";               // 더보기 링크
}
