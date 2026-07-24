namespace ChurchWeb.Web.Models.Home;

/// <summary>
/// 오시는 길 섹션 데이터
/// </summary>
public class LocateVm
{
    public string Address { get; set; } = string.Empty;      // "○○시 ○○구 ○○로 1, 304호"
    public string Phone { get; set; } = string.Empty;        // "02-000-0000"
    public string WorshipSchedule { get; set; } = string.Empty; // 예배 시간
    public string BankAccount { get; set; } = string.Empty;  // 온라인 헌금 계좌
    public string MapCaption { get; set; } = "지도 API 연동 영역 (Kakao / Naver Map)";
    public double? Latitude { get; set; }   // 위도
    public double? Longitude { get; set; }  // 경도
}
