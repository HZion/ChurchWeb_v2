namespace ChurchWeb.Web.Models.News;

public class NoticeDetailVm
{
    public string PageTitle { get; set; } = "교회소식";
    public string PageSubtitle { get; set; } = "교회와 교우들의 소식을 전합니다";
    public List<NewsTabVm> Tabs { get; set; } = new();

    public int Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public int Views { get; set; }

    // 본문 HTML (관리자 에디터 결과)
    public string BodyHtml { get; set; } = string.Empty;

    // 첨부파일 (없으면 빈 리스트)
    public List<Attachment> Attachments { get; set; } = new();

    // 이전/다음 네비게이션
    public NoticeVm? Prev { get; set; }
    public NoticeVm? Next { get; set; }
}
