namespace ChurchWeb.Web.Models.News;

public class BulletinCardVm
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;        // 날짜 ex: "2024.01.07"
    public string CoverUrl { get; set; } = string.Empty;      // 표지 이미지
    public string FileUrl { get; set; } = string.Empty;       // PDF 다운로드
    public string RegDate { get; set; } = string.Empty;
}
