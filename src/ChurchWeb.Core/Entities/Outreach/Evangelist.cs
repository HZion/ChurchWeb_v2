namespace ChurchWeb.Core.Entities.Outreach;

/// <summary>
/// 전도자 (온라인 전도카드용)
/// </summary>
public class Evangelist
{
    public int Id { get; set; }

    /// <summary>
    /// 전도자 이름
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 직분 (전도사/집사/성도 등)
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 전화번호
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 사진 URL
    /// </summary>
    public string PhotoUrl { get; set; } = string.Empty;

    /// <summary>
    /// 인사 문구
    /// </summary>
    public string Greeting { get; set; } = string.Empty;

    /// <summary>
    /// 활성 여부
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 정렬 순서
    /// </summary>
    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
