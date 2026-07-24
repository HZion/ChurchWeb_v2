using Microsoft.AspNetCore.Mvc;
using ChurchWeb.Application.Services;

namespace ChurchWeb.Web.Controllers;

public class HomeController : Controller
{
    private readonly IHomeService _homeService;
    private readonly IConfiguration _configuration;

    public HomeController(IHomeService homeService, IConfiguration configuration)
    {
        _homeService = homeService;
        _configuration = configuration;
    }

    public async Task<IActionResult> Index()
    {
        var model = await _homeService.GetHomeViewModelAsync();
        ViewBag.KakaoMapApiKey = _configuration["KakaoMap:ApiKey"] ?? "";
        return View(model);
    }

    public IActionResult Error()
    {
        return View();
    }
}

/* OLD CODE - 더미 데이터 (참고용)
    public async Task<IActionResult> Index_OLD()
    {
        var model = new HomeViewModel
        {
            // 섹션 메타데이터 (표시/숨김, 순서 제어)
            Sections = new List<HomeSectionVm>
            {
                new() { Key = "hero", Title = "히어로", IsVisible = true, SortOrder = 1 },
                new() { Key = "vision", Title = "올해의 표어", IsVisible = true, SortOrder = 2 },
                new() { Key = "sermons", Title = "말씀과 찬양", IsVisible = true, SortOrder = 3 },
                new() { Key = "quick", Title = "바로가기", IsVisible = true, SortOrder = 4 },
                new() { Key = "media", Title = "소개 영상", IsVisible = true, SortOrder = 5 },
                new() { Key = "pastor", Title = "담임목사 인사말", IsVisible = true, SortOrder = 6 },
                new() { Key = "news", Title = "교회 소식", IsVisible = true, SortOrder = 7 },
                new() { Key = "locate", Title = "오시는 길", IsVisible = true, SortOrder = 8 }
            },

            // 히어로 섹션 (슬라이드 목록)
            Hero = new HeroVm
            {
                Slides = new List<HeroSlideVm>
                {
                    new()
                    {
                        BackgroundType = "gradient",
                        Background = "linear-gradient(180deg,var(--hero-o1),var(--hero-o2)),radial-gradient(120% 100% at 65% 20%,#7a5f42,#4a3626 45%,#2c2018)",
                        Kicker = "WELCOME TO OUR CHURCH",
                        Title = "모든 것이 합력하여<br>선을 이루느니라",
                        Subtitle = "말씀 위에 세워진 공동체, ○○교회에 오신 것을 환영합니다.",
                        PrimaryBtnText = "예배 안내 ↗",
                        PrimaryBtnUrl = "#quick",
                        SecondaryBtnText = "교회 소개 영상",
                        SecondaryBtnUrl = "#media",
                        IsVisible = true,
                        SortOrder = 1
                    },
                    new()
                    {
                        BackgroundType = "gradient",
                        Background = "linear-gradient(180deg,var(--hero-o1),var(--hero-o2)),radial-gradient(120% 100% at 28% 30%,#6b5238,#3c2c1e 50%,#241a12)",
                        Kicker = "2026 · 올해의 표어",
                        Title = "오직 주의 말씀 안에서<br>날마다 새롭게 되라",
                        Subtitle = "한 해 동안 함께 붙드는 말씀으로 나아갑니다.",
                        PrimaryBtnText = "올해의 비전 보기",
                        PrimaryBtnUrl = "#vision",
                        SecondaryBtnText = "",
                        SecondaryBtnUrl = "",
                        IsVisible = true,
                        SortOrder = 2
                    },
                    new()
                    {
                        BackgroundType = "gradient",
                        Background = "linear-gradient(180deg,var(--hero-o1),var(--hero-o2)),radial-gradient(120% 110% at 55% 80%,#5a4632,#33261a 55%,#201711)",
                        Kicker = "SUNDAY WORSHIP",
                        Title = "주일, 함께 드리는<br>온전한 예배로의 초대",
                        Subtitle = "주일 1부 09:00 · 2부 11:00, 여러분을 기다립니다.",
                        PrimaryBtnText = "예배 시간 안내 ↗",
                        PrimaryBtnUrl = "#quick",
                        SecondaryBtnText = "오시는 길",
                        SecondaryBtnUrl = "#locate",
                        IsVisible = true,
                        SortOrder = 3
                    },
                    new()
                    {
                        BackgroundType = "image",
                        ImageUrl = "data:image/svg+xml,%3Csvg%20xmlns='http://www.w3.org/2000/svg'%20width='1600'%20height='900'%3E%3Cdefs%3E%3ClinearGradient%20id='g'%20x1='0'%20y1='0'%20x2='0'%20y2='1'%3E%3Cstop%20offset='0'%20stop-color='%23d8c3a0'/%3E%3Cstop%20offset='1'%20stop-color='%236b5236'/%3E%3C/linearGradient%3E%3C/defs%3E%3Crect%20width='1600'%20height='900'%20fill='url(%23g)'/%3E%3Ccircle%20cx='1160'%20cy='210'%20r='120'%20fill='%23e9d9b8'%20opacity='0.45'/%3E%3Crect%20x='770'%20y='260'%20width='60'%20height='380'%20fill='%233a2a1c'/%3E%3Crect%20x='688'%20y='344'%20width='224'%20height='58'%20fill='%233a2a1c'/%3E%3C/svg%3E",
                        OverlayOpacity = 0.5,
                        Kicker = "사진 배경 예시 · PHOTO",
                        Title = "사진 위에 얹힌<br>히어로 문구입니다",
                        Subtitle = "배경이 사진이어도 어두운 오버레이 덕분에 글자가 선명하게 보입니다.",
                        PrimaryBtnText = "자세히 보기",
                        PrimaryBtnUrl = "#",
                        SecondaryBtnText = "",
                        SecondaryBtnUrl = "",
                        IsVisible = true,
                        SortOrder = 4
                    }
                }
            },

            // 올해의 표어
            Vision = new VisionVm
            {
                Year = "2026",
                Motto = "오직 주의 말씀 안에서\n날마다 새롭게 되라",
                Practices = new List<PracticeItem>
                {
                    new() { Number = "01", Text = "매일 말씀 묵상과 기도" },
                    new() { Number = "02", Text = "이웃을 향한 섬김과 나눔" },
                    new() { Number = "03", Text = "한 영혼을 향한 전도" }
                }
            },

            // 말씀과 찬양
            Sermons = new SermonsVm
            {
                SectionEyebrow = "SERMONS",
                SectionTitle = "말씀과 찬양",
                SectionDescription = "유튜브 URL만 입력하면 본문·제목·설교자·날짜·썸네일이 자동 연동됩니다.",
                Sermons = new List<SermonCard>
                {
                    new()
                    {
                        Category = "주일설교",
                        Verse = "시편 23:1",
                        Title = "여호와는 나의 목자시니",
                        Preacher = "홍길동 목사",
                        Date = "2026.07.12",
                        Duration = "42:10",
                        ThumbnailUrl = "",
                        VideoUrl = ""
                    },
                    new()
                    {
                        Category = "주일설교",
                        Verse = "로마서 8:28",
                        Title = "모든 것이 합력하여 선을",
                        Preacher = "홍길동 목사",
                        Date = "2026.07.05",
                        Duration = "38:55",
                        ThumbnailUrl = "",
                        VideoUrl = ""
                    },
                    new()
                    {
                        Category = "특별설교",
                        Verse = "마태복음 5:14",
                        Title = "너희는 세상의 빛이라",
                        Preacher = "홍길동 목사",
                        Date = "2026.06.28",
                        Duration = "45:30",
                        ThumbnailUrl = "",
                        VideoUrl = ""
                    },
                    new()
                    {
                        Category = "주일설교",
                        Verse = "요한복음 3:16",
                        Title = "하나님의 사랑",
                        Preacher = "홍길동 목사",
                        Date = "2026.06.21",
                        Duration = "40:02",
                        ThumbnailUrl = "",
                        VideoUrl = ""
                    }
                }
            },

            // 바로가기
            Quick = new QuickVm
            {
                About = new QuickCard
                {
                    Title = "교회소개",
                    Description = "○○교회는 이 시대 속에서 사랑과 믿음,\n그리고 순결한 가치로 세상을 아름답게 만들어가고자 합니다.",
                    LinkUrl = "#"
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
                    Description = "모든 예배는 하나님을 향한 전심의 고백과 영혼을 깨우는 진리의 말씀으로 드려집니다.",
                    LinkUrl = "#"
                },
                Sermons = new QuickCard
                {
                    Title = "설교 말씀",
                    Description = "주일마다 선포된 하나님의 말씀을 영상으로 다시 들으실 수 있습니다.",
                    LinkUrl = "#"
                },
                Location = new QuickCard
                {
                    Title = "오시는 길",
                    Description = "○○교회로 오시는 길을 안내합니다. 예배의 자리에서 따뜻하게 만나겠습니다.",
                    LinkUrl = "#locate"
                }
            },

            // 소개 영상
            Media = new MediaVm
            {
                SectionEyebrow = "CHURCH FILM",
                SectionTitle = "교회 소개 영상",
                SectionDescription = "관리자 페이지에서 유튜브 URL만 입력하면 이 자리에 자동 임베드됩니다.",
                YouTubeUrl = "",
                Caption = "YouTube 임베드 영역 · 16:9"
            },

            // 담임목사 인사말
            Pastor = new PastorVm
            {
                PhotoUrl = "",
                Role = "홍길동 담임 목사",
                Title = "주님의 이름으로\n환영하고 축복합니다.",
                Message = "주님이 원하시는 교회! 그런 교회가 되기 위해 성경을 바탕으로 성도의 신앙교육과 다음세대의 영적 양육, 그리고 지역 공동체와 깊은 유대감 있는 연계를 이어가고 있습니다.",
                MoreUrl = "#"
            },

            // 교회 소식
            News = new NewsVm
            {
                SectionEyebrow = "NEWS & EVENTS",
                SectionTitle = "교회 소식",
                Notices = new List<NoticeItem>
                {
                    new() { IsPinned = true, Title = "여름 성경학교 신청 안내", Date = "07.15", Url = "#" },
                    new() { IsPinned = false, Title = "7월 정기 제직회 안내", Date = "07.10", Url = "#" },
                    new() { IsPinned = false, Title = "주차장 이용 안내 변경", Date = "07.03", Url = "#" },
                    new() { IsPinned = false, Title = "새가족 환영 예배 안내", Date = "06.29", Url = "#" }
                },
                Events = new List<EventItem>
                {
                    new() { Title = "여름 수련회", ThumbnailUrl = "", Url = "#" },
                    new() { Title = "전교인 체육대회", ThumbnailUrl = "", Url = "#" },
                    new() { Title = "봄 심방주간", ThumbnailUrl = "", Url = "#" },
                    new() { Title = "임직 감사예배", ThumbnailUrl = "", Url = "#" }
                }
            },

            // 오시는 길
            Locate = new LocateVm
            {
                Address = "○○시 ○○구 ○○로 1, 304호",
                Phone = "02-000-0000",
                WorshipSchedule = "주일 1부 09:00 · 2부 11:00 · 수요 19:30 · 새벽 05:30",
                BankAccount = "○○은행 000-00-00000 (○○교회)",
                MapCaption = "지도 API 연동 영역 (Kakao / Naver Map)"
            }
        };

        return View(model);
    }

    public IActionResult Error()
    {
        return View();
    }
}
*/
