namespace ChurchWeb.Web.Models.Home;

public class NavMenuItemVm
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = "#";
    public List<NavMenuItemVm> SubItems { get; set; } = new();
}
