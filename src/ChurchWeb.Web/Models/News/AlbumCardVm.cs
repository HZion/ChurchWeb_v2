namespace ChurchWeb.Web.Models.News;

public class AlbumCardVm
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CoverUrl { get; set; } = string.Empty;
    public int PhotoCount { get; set; }
    public string Date { get; set; } = string.Empty;
    public int Year { get; set; }
}
