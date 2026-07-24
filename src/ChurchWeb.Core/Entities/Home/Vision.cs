namespace ChurchWeb.Core.Entities.Home;

/// <summary>
/// 올해의 표어 (단일 레코드)
/// </summary>
public class Vision
{
    public int Id { get; set; }

    public string Year { get; set; } = string.Empty;           // "2026"
    public string MottoText { get; set; } = string.Empty;      // 표어 텍스트 (줄바꿈: \n 또는 <br>)
    public string ScriptureRef { get; set; } = string.Empty;   // 성경 구절 (선택사항)

    public bool IsVisible { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public ICollection<VisionPractice> Practices { get; set; } = new List<VisionPractice>();
}
