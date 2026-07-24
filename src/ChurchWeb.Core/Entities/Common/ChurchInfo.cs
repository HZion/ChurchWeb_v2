namespace ChurchWeb.Core.Entities.Common;

/// <summary>
/// 사이트 전역 설정 (단일 레코드)
/// </summary>
public class ChurchInfo
{
    public int Id { get; set; }

    // 기본 정보
    public string ChurchName { get; set; } = string.Empty;
    public string Denomination { get; set; } = string.Empty;  // "대한예수교장로회(합동)"

    // 연락처
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // 온라인
    public string YoutubeUrl { get; set; } = string.Empty;
    public string HomepageUrl { get; set; } = string.Empty;  // 교회 홈페이지 URL
    public string OnlineOfferingAccount { get; set; } = string.Empty;

    // 지도
    public string MapEmbed { get; set; } = string.Empty;  // Kakao/Naver Map 임베드 코드
    public double? Latitude { get; set; }   // 위도
    public double? Longitude { get; set; }  // 경도

    // 예배 시간 (JSON 또는 문자열)
    public string WorshipTimesJson { get; set; } = string.Empty;

    // 추가 정보
    public string FooterText { get; set; } = string.Empty;

    // Phase 1: 교회 정보 확장
    /// <summary>
    /// 연간 슬로건 (예: "2024년 교회 표어")
    /// </summary>
    public string AnnualSlogan { get; set; } = string.Empty;

    /// <summary>
    /// 실천사항 (JSON 배열 형식으로 저장)
    /// </summary>
    public string PracticesJson { get; set; } = string.Empty;

    /// <summary>
    /// 홍보 영상 URL (YouTube 등)
    /// </summary>
    public string PromoVideoUrl { get; set; } = string.Empty;

    /// <summary>
    /// 전도카드 이미지 URL
    /// </summary>
    public string OutreachCardImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// 전도카드 PDF URL
    /// </summary>
    public string OutreachCardPdfUrl { get; set; } = string.Empty;

    // Phase 2: 온라인 전도카드 필드
    /// <summary>
    /// 전도카드 환영 문구
    /// </summary>
    public string OutreachWelcomeMessage { get; set; } = string.Empty;

    /// <summary>
    /// 전도카드 전용 단축 URL (예: antioch.kr/welcome)
    /// </summary>
    public string OutreachShortUrl { get; set; } = string.Empty;

    /// <summary>
    /// 길찾기 링크 (지도 URL)
    /// </summary>
    public string OutreachMapLink { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
