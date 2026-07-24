using ChurchWeb.Core.Entities.News;

namespace ChurchWeb.Web.Models.Outreach;

public class OutreachCardVm
{
    // 전도자 정보
    public int EvangelistId { get; set; }
    public string EvangelistName { get; set; } = string.Empty;
    public string EvangelistTitle { get; set; } = string.Empty;
    public string EvangelistPhone { get; set; } = string.Empty;
    public string EvangelistPhotoUrl { get; set; } = string.Empty;
    public string EvangelistGreeting { get; set; } = string.Empty;

    // 교회 정보
    public string ChurchName { get; set; } = string.Empty;
    public string Denomination { get; set; } = string.Empty;
    public string ChurchAddress { get; set; } = string.Empty;
    public string ChurchPhone { get; set; } = string.Empty;
    public string HomepageUrl { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    // 홍보 영상
    public string PromoVideoUrl { get; set; } = string.Empty;

    // 주보 (최신 3건)
    public List<BulletinItem> Bulletins { get; set; } = new();

    // 갤러리 (최신 4건)
    public List<AlbumItem> Albums { get; set; } = new();
}

public class BulletinItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime PublishedDate { get; set; }
}

public class AlbumItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
}
