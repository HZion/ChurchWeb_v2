using System.ComponentModel.DataAnnotations;

namespace ChurchWeb.Web.Models.Admin;

public class ChurchInfoViewModel
{
    public int Id { get; set; }

    // 기본 정보
    [Required(ErrorMessage = "교회명을 입력하세요")]
    [Display(Name = "교회명")]
    public string ChurchName { get; set; } = string.Empty;

    [Display(Name = "교단")]
    public string Denomination { get; set; } = string.Empty;

    // 연락처
    [Required(ErrorMessage = "주소를 입력하세요")]
    [Display(Name = "주소")]
    public string Address { get; set; } = string.Empty;

    [Display(Name = "전화번호")]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "올바른 이메일 형식이 아닙니다")]
    [Display(Name = "이메일")]
    public string Email { get; set; } = string.Empty;

    // 온라인
    [Display(Name = "유튜브 URL")]
    public string YoutubeUrl { get; set; } = string.Empty;

    [Display(Name = "온라인 헌금 계좌")]
    public string OnlineOfferingAccount { get; set; } = string.Empty;

    // 지도
    [Display(Name = "지도 임베드 코드")]
    public string MapEmbed { get; set; } = string.Empty;

    [Display(Name = "위도 (Latitude)")]
    public double? Latitude { get; set; }

    [Display(Name = "경도 (Longitude)")]
    public double? Longitude { get; set; }

    // 예배 시간
    [Display(Name = "예배 시간 (JSON)")]
    public string WorshipTimesJson { get; set; } = string.Empty;

    // 추가 정보
    [Display(Name = "Footer 텍스트")]
    public string FooterText { get; set; } = string.Empty;

    // Phase 1: 교회 정보 확장
    [Display(Name = "연간 슬로건")]
    public string AnnualSlogan { get; set; } = string.Empty;

    [Display(Name = "실천사항 (한 줄씩 입력)")]
    public string Practices { get; set; } = string.Empty;

    [Display(Name = "홍보 영상 URL")]
    public string PromoVideoUrl { get; set; } = string.Empty;

    [Display(Name = "전도카드 이미지 URL")]
    public string OutreachCardImageUrl { get; set; } = string.Empty;

    [Display(Name = "전도카드 PDF URL")]
    public string OutreachCardPdfUrl { get; set; } = string.Empty;

    // Vision 섹션 (메인 화면 "올해의 표어"에 표시)
    [Display(Name = "연도")]
    public string Year { get; set; } = DateTime.Now.Year.ToString();

    [Display(Name = "올해의 표어")]
    public string VisionMotto { get; set; } = string.Empty;

    [Display(Name = "성경 구절 (선택사항)")]
    public string VisionScripture { get; set; } = string.Empty;

    // 담임목사 인사말
    [Display(Name = "목사 직함")]
    public string PastorRole { get; set; } = string.Empty;

    [Display(Name = "목사 이름")]
    public string PastorName { get; set; } = string.Empty;

    [Display(Name = "인사말 제목")]
    public string PastorTitle { get; set; } = string.Empty;

    [Display(Name = "인사말 내용")]
    public string PastorMessage { get; set; } = string.Empty;

    [Display(Name = "목사 사진 URL")]
    public string PastorPhotoUrl { get; set; } = string.Empty;

    [Display(Name = "인사말 더보기 URL")]
    public string PastorMoreUrl { get; set; } = string.Empty;
}
