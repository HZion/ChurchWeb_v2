namespace ChurchWeb.Web.Models.Worship;

public class SermonListVm
{
    public string PageTitle { get; set; } = string.Empty;
    public string PageSubtitle { get; set; } = string.Empty;
    public List<WorshipTabVm> Tabs { get; set; } = new();
    public List<SermonCardVm> Sermons { get; set; } = new();
    public string? SearchQuery { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
}
