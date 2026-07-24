namespace ChurchWeb.Core.Entities.Home;

/// <summary>
/// 비전 실천 항목
/// </summary>
public class VisionPractice
{
    public int Id { get; set; }

    public int VisionId { get; set; }
    public string Number { get; set; } = string.Empty;  // "01", "02", "03"
    public string Text { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    // Navigation
    public Vision Vision { get; set; } = null!;
}
