namespace ChurchWeb.Web.Models.Admin;

public class DashboardViewModel
{
    public StatisticsViewModel Statistics { get; set; } = new();
    public List<RecentItemViewModel> RecentSermons { get; set; } = new();
    public List<RecentItemViewModel> RecentBulletins { get; set; } = new();
    public List<RecentItemViewModel> RecentNotices { get; set; } = new();
}

public class StatisticsViewModel
{
    public int TotalSermons { get; set; }
    public int TotalBulletins { get; set; }
    public int TotalAlbums { get; set; }
    public int TotalNotices { get; set; }
    public int TotalEvents { get; set; }
    public int TotalHeroSlides { get; set; }
}

public class RecentItemViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string? Category { get; set; }
}
