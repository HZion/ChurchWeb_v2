namespace ChurchWeb.Web.Models.Home;

/// <summary>
/// 홈 페이지 전체 ViewModel
/// </summary>
public class HomeViewModel
{
    // 섹션 메타데이터 (표시/숨김, 순서 제어)
    public List<HomeSectionVm> Sections { get; set; } = new();

    // 각 섹션 데이터
    public HeroVm Hero { get; set; } = new();
    public VisionVm Vision { get; set; } = new();
    public SermonsVm Sermons { get; set; } = new();
    public QuickVm Quick { get; set; } = new();
    public MediaVm Media { get; set; } = new();
    public PastorVm Pastor { get; set; } = new();
    public NewsVm News { get; set; } = new();
    public LocateVm Locate { get; set; } = new();
}
