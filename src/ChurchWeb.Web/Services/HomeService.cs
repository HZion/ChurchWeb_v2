using ChurchWeb.Application.Services;
using ChurchWeb.Infrastructure.Data;
using ChurchWeb.Infrastructure.Services;
using ChurchWeb.Web.Models.Home;
using Microsoft.EntityFrameworkCore;

namespace ChurchWeb.Web.Services;

public class HomeService : IHomeService
{
    private readonly AppDbContext _context;
    private readonly IChurchInfoService _churchInfoService;
    private readonly ILogger<HomeService> _logger;

    public HomeService(AppDbContext context, IChurchInfoService churchInfoService, ILogger<HomeService> logger)
    {
        _context = context;
        _churchInfoService = churchInfoService;
        _logger = logger;
    }

    public async Task<object> GetHomeViewModelAsync()
    {
        // 섹션 메타데이터
        var sections = await _context.HomeSections
            .OrderBy(s => s.SortOrder)
            .Select(s => new HomeSectionVm
            {
                Key = s.Key,
                Title = s.Title,
                IsVisible = s.IsVisible,
                SortOrder = s.SortOrder
            })
            .ToListAsync();

        // 히어로 슬라이드
        var heroSlides = await _context.HeroSlides
            .Where(h => h.IsVisible)
            .OrderBy(h => h.SortOrder)
            .Select(h => new HeroSlideVm
            {
                BackgroundType = h.BackgroundType,
                ImageUrl = h.ImageUrl,
                Background = h.Background,
                OverlayOpacity = h.OverlayOpacity,
                Kicker = h.Kicker,
                Title = h.Title,
                Subtitle = h.Subtitle,
                PrimaryBtnText = h.PrimaryBtnText,
                PrimaryBtnUrl = h.PrimaryBtnUrl,
                SecondaryBtnText = h.SecondaryBtnText,
                SecondaryBtnUrl = h.SecondaryBtnUrl,
                IsVisible = h.IsVisible,
                SortOrder = h.SortOrder
            })
            .ToListAsync();

        // 비전
        var vision = await _context.Visions
            .Include(v => v.Practices)
            .Where(v => v.IsVisible)
            .FirstOrDefaultAsync();

        var visionVm = vision != null ? new VisionVm
        {
            Year = vision.Year,
            Motto = vision.MottoText,
            Practices = vision.Practices
                .OrderBy(p => p.SortOrder)
                .Select(p => new PracticeItem
                {
                    Number = p.Number,
                    Text = p.Text
                })
                .ToList()
        } : new VisionVm
        {
            Year = "2026",
            Motto = "오직 주의 말씀 안에서\n날마다 새롭게 되라",
            Practices = new List<PracticeItem>()
        };

        // 설교
        var sermons = await _context.Sermons
            .Where(s => s.IsVisible)
            .OrderByDescending(s => s.PreachedOn)
            .Take(4)
            .Select(s => new SermonCard
            {
                Category = s.Category == "sunday" ? "주일설교" : "특별설교",
                Verse = s.Verse,
                Title = s.Title,
                Preacher = s.Preacher,
                Date = s.PreachedOn.ToString("yyyy.MM.dd"),
                Duration = s.Duration,
                ThumbnailUrl = s.ThumbnailUrl,
                VideoUrl = s.YoutubeUrl
            })
            .ToListAsync();

        // 담임목사 인사말
        var pastorGreeting = await _context.PastorGreetings
            .Where(p => p.IsVisible)
            .FirstOrDefaultAsync();

        var pastorVm = pastorGreeting != null ? new PastorVm
        {
            PhotoUrl = pastorGreeting.PhotoUrl,
            Role = pastorGreeting.Role,
            Title = pastorGreeting.Title,
            Message = pastorGreeting.Message,
            MoreUrl = pastorGreeting.MoreUrl
        } : new PastorVm
        {
            Role = "담임 목사",
            Title = "주님의 이름으로 환영합니다",
            Message = ""
        };

        // 소식
        var notices = await _context.Notices
            .Where(n => n.IsVisible && n.CategoryKey == "church")
            .OrderByDescending(n => n.IsPinned)
            .ThenByDescending(n => n.PostedOn)
            .Take(4)
            .Select(n => new NoticeItem
            {
                IsPinned = n.IsPinned,
                Title = n.Title,
                Date = n.PostedOn.ToString("MM.dd"),
                Url = "#"
            })
            .ToListAsync();

        // 교회 정보 (캐시된 ChurchInfo에서 가져오기)
        _logger.LogInformation("=== HomeService GetChurchInfo 시작 ===");
        _logger.LogInformation($"ChurchInfoService Type: {_churchInfoService.GetType().Name}");
        var churchInfo = await _churchInfoService.GetChurchInfoAsync();

        if (churchInfo != null)
        {
            _logger.LogInformation($"ChurchInfo ID: {churchInfo.Id}");
            _logger.LogInformation($"ChurchName: {churchInfo.ChurchName}");
            _logger.LogInformation($"AnnualSlogan: {churchInfo.AnnualSlogan}");
            _logger.LogInformation($"PromoVideoUrl: {churchInfo.PromoVideoUrl}");
        }
        else
        {
            _logger.LogWarning("ChurchInfo is NULL!");
        }
        _logger.LogInformation("=== HomeService GetChurchInfo 완료 ===");

        return new HomeViewModel
        {
            Sections = sections,
            Hero = new HeroVm { Slides = heroSlides },
            Vision = visionVm,
            Sermons = new SermonsVm
            {
                SectionEyebrow = "SERMONS",
                SectionTitle = "말씀과 찬양",
                SectionDescription = "유튜브 URL만 입력하면 본문·제목·설교자·날짜·썸네일이 자동 연동됩니다.",
                Sermons = sermons
            },
            Quick = new QuickVm
            {
                About = new QuickCard
                {
                    Title = "교회소개",
                    Description = churchInfo != null && !string.IsNullOrWhiteSpace(churchInfo.AnnualSlogan)
                        ? churchInfo.AnnualSlogan
                        : "○○교회는 이 시대 속에서 사랑과 믿음,\n그리고 순결한 가치로 세상을 아름답게 만들어가고자 합니다.",
                    LinkUrl = "/about/vision"
                },
                Bulletin = new BulletinCard
                {
                    Title = "이번주 주보",
                    Description = "사랑과 믿음으로 성도와 이웃을 섬기며,\n함께 성장하는 교회를 세워갑니다.",
                    ViewUrl = "#",
                    DownloadUrl = "#",
                    ThumbnailUrl = ""
                },
                Worship = new QuickCard
                {
                    Title = "예배안내",
                    Description = churchInfo != null && !string.IsNullOrWhiteSpace(churchInfo.WorshipTimesJson)
                        ? $"예배 시간: {churchInfo.WorshipTimesJson}"
                        : "모든 예배는 하나님을 향한 전심의 고백과 영혼을 깨우는 진리의 말씀으로 드려집니다.",
                    LinkUrl = "/about/worship"
                },
                Sermons = new QuickCard
                {
                    Title = "설교 말씀",
                    Description = "주일마다 선포된 하나님의 말씀을 영상으로 다시 들으실 수 있습니다.",
                    LinkUrl = "/sermons/sunday"
                },
                Location = new QuickCard
                {
                    Title = "오시는 길",
                    Description = churchInfo != null
                        ? $"{churchInfo.ChurchName}로 오시는 길을 안내합니다. 예배의 자리에서 따뜻하게 만나겠습니다."
                        : "○○교회로 오시는 길을 안내합니다. 예배의 자리에서 따뜻하게 만나겠습니다.",
                    LinkUrl = "/about/location"
                }
            },
            Media = new MediaVm
            {
                SectionEyebrow = "CHURCH FILM",
                SectionTitle = "교회 소개 영상",
                SectionDescription = "관리자 페이지에서 유튜브 URL만 입력하면 이 자리에 자동 임베드됩니다.",
                YouTubeUrl = churchInfo?.PromoVideoUrl ?? "",
                Caption = "YouTube 임베드 영역 · 16:9"
            },
            Pastor = pastorVm,
            News = new NewsVm
            {
                SectionEyebrow = "NEWS & EVENTS",
                SectionTitle = "교회 소식",
                Notices = notices,
                Events = new List<EventItem>
                {
                    new() { Title = "여름 수련회", ThumbnailUrl = "", Url = "#" },
                    new() { Title = "전교인 체육대회", ThumbnailUrl = "", Url = "#" },
                    new() { Title = "봄 심방주간", ThumbnailUrl = "", Url = "#" },
                    new() { Title = "임직 감사예배", ThumbnailUrl = "", Url = "#" }
                }
            },
            Locate = new LocateVm
            {
                Address = churchInfo?.Address ?? "전라남도 해남군 화산면 관동리 441-1",
                Phone = churchInfo?.Phone ?? "02-000-0000",
                WorshipSchedule = churchInfo?.WorshipTimesJson ?? "주일 1부 09:00 · 2부 11:00 · 수요 19:30 · 새벽 05:30",
                BankAccount = churchInfo?.OnlineOfferingAccount ?? "○○은행 000-00-00000 (○○교회)",
                MapCaption = "지도 API 연동 영역 (Kakao / Naver Map)",
                Latitude = churchInfo?.Latitude ?? 34.47583905316223,
                Longitude = churchInfo?.Longitude ?? 126.47554034813862
            }
        };
    }
}
