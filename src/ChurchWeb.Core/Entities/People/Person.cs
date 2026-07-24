namespace ChurchWeb.Core.Entities.People;

/// <summary>
/// 섬기는 사람들 - 교역자 및 직분자 정보
/// </summary>
public class Person
{
    public int Id { get; set; }

    /// <summary>
    /// 대표 소개 여부 (true면 페이지 최상단에 크게 표시)
    /// </summary>
    public bool IsHead { get; set; }

    /// <summary>
    /// 이름
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 직분 (예: 목사, 부목사, 전도사, 장로, 안수집사, 권사)
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 구분 (대표용 - 예: 담임목사, 원로목사)
    /// </summary>
    public string? Role { get; set; }

    /// <summary>
    /// 분류/카테고리 (pastor, elder, deacon, deaconess)
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// 부서 또는 설명 (예: 교육부서, 유아유치부)
    /// </summary>
    public string? Ministry { get; set; }

    /// <summary>
    /// 사진 URL
    /// </summary>
    public string? PhotoUrl { get; set; }

    /// <summary>
    /// 소개 문구 (대표용 - 약력/인사말)
    /// </summary>
    public string? Intro { get; set; }

    /// <summary>
    /// 인용구 (대표용)
    /// </summary>
    public string? Quote { get; set; }

    /// <summary>
    /// 공개 페이지 노출 여부
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// 정렬 순서
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 생성일
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 수정일
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
