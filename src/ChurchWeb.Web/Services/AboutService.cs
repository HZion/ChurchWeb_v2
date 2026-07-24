using ChurchWeb.Application.Services;
using ChurchWeb.Core.Entities.People;
using ChurchWeb.Infrastructure.Data;
using ChurchWeb.Web.Models.About;
using Microsoft.EntityFrameworkCore;

namespace ChurchWeb.Web.Services;

public class AboutService : IAboutService
{
    private readonly AppDbContext _context;

    public AboutService(AppDbContext context)
    {
        _context = context;
    }

    private List<AboutTabVm> GetAboutTabs()
    {
        return new List<AboutTabVm>
        {
            new() { Key = "vision", Title = "사명과 비전", Url = "/about/vision", IsVisible = true, SortOrder = 1 },
            new() { Key = "worship", Title = "예배 안내", Url = "/about/worship", IsVisible = true, SortOrder = 2 },
            new() { Key = "people", Title = "섬기는 사람들", Url = "/about/people", IsVisible = true, SortOrder = 3 },
            new() { Key = "mission", Title = "후원하는 교회", Url = "#", IsVisible = true, SortOrder = 4 },
            new() { Key = "location", Title = "오시는 길", Url = "/about/location", IsVisible = true, SortOrder = 5 },
            new() { Key = "facilities", Title = "시설 안내", Url = "#", IsVisible = true, SortOrder = 6 }
        };
    }

    public async Task<object> GetVisionViewModelAsync()
    {
        // Vision 정보 가져오기
        var vision = await _context.Visions
            .Include(v => v.Practices)
            .Where(v => v.IsVisible)
            .FirstOrDefaultAsync();

        var model = new AboutVisionVm
        {
            PageTitle = "사명과 비전",
            PageSubtitle = "말씀 위에 세워진 공동체, 안디옥 교회가 나아가는 방향입니다.",
            Tabs = GetAboutTabs(),

            MissionEyebrow = "OUR MISSION",
            MissionLead = "안디옥 교회는 <span class=\"hl\">오직 성경 위에 서서</span>, 예배와 말씀으로 하나님을 높이고 한 영혼을 사랑으로 세우는 공동체입니다.",
            MissionBody = new List<string>
            {
                "본 교회는 하나님의 말씀인 성경을 신앙과 삶의 유일한 기준으로 삼으며, 예수 그리스도를 구주와 주로 고백하는 성도들의 공동체입니다. 대한예수교장로회(합동)의 신앙고백 위에서 바른 예배와 건강한 교제, 다음 세대를 향한 신앙 교육에 힘씁니다.",
                "우리는 이웃과 지역을 향해 하나님의 사랑을 전하고, 복음 위에 굳게 서서 이 땅에 하나님 나라가 이루어지기를 소망합니다."
            },

            VisionItems = new List<VisionItem>
            {
                new() { Label = "VISION 01", Title = "예배하는 교회", Desc = "말씀과 기도로 하나님을 예배하며, 삶으로 드리는 예배를 회복합니다.", SortOrder = 1 },
                new() { Label = "VISION 02", Title = "세우는 교회", Desc = "성도가 말씀 안에서 자라고, 다음 세대가 믿음으로 서도록 함께 세워갑니다.", SortOrder = 2 },
                new() { Label = "VISION 03", Title = "전하는 교회", Desc = "이웃과 지역, 열방을 향해 복음을 전하며 사랑을 실천합니다.", SortOrder = 3 }
            },

            ValueItems = new List<ValueItem>
            {
                new() { No = "01", Title = "말씀 중심", Desc = "성경을 신앙과 삶의 유일한 기준으로 삼습니다.", SortOrder = 1 },
                new() { No = "02", Title = "예배 회복", Desc = "진실한 마음으로 하나님께 온전히 예배합니다.", SortOrder = 2 },
                new() { No = "03", Title = "사랑의 교제", Desc = "서로 섬기고 돌보며 한 몸으로 세워집니다.", SortOrder = 3 },
                new() { No = "04", Title = "복음 전도", Desc = "한 영혼을 귀히 여겨 이웃에게 복음을 전합니다.", SortOrder = 4 }
            },

            MottoYear = vision?.Year ?? "2026",
            MottoText = vision != null ? $"\"{vision.MottoText}\"" : "\"오직 주의 말씀 안에서<br>날마다 새롭게 되라\"",
            ScriptureRef = vision?.ScriptureRef ?? "— 골로새서 3:16"
        };

        return model;
    }

    public async Task<object> GetWorshipViewModelAsync()
    {
        var churchInfo = await _context.ChurchInfos.FirstOrDefaultAsync();

        var model = new AboutWorshipVm
        {
            PageTitle = "예배 안내",
            PageSubtitle = "하나님께 드리는 예배 시간을 안내합니다.",
            Tabs = GetAboutTabs(),

            IntroEyebrow = "WORSHIP",
            IntroTitle = "함께 모여 드리는 예배",
            IntroText = "안디옥 교회는 말씀과 성령 안에서 하나님을 예배합니다. 모든 예배는 온라인으로도 실시간 중계됩니다.",

            Blocks = new List<WorshipBlock>
            {
                new() {
                    Title = "정기 예배",
                    IsVisible = true,
                    SortOrder = 1,
                    Rows = new List<WorshipRow>
                    {
                        new() { Name = "주일 1부 예배", Time = "매주 일요일 오전 9:00", Place = "본당", Note = "실시간 온라인 중계", SortOrder = 1 },
                        new() { Name = "주일 2부 예배", Time = "매주 일요일 오전 11:00", Place = "본당", Note = "실시간 온라인 중계", SortOrder = 2 },
                        new() { Name = "수요 예배", Time = "매주 수요일 저녁 7:30", Place = "본당", Note = "", SortOrder = 3 },
                        new() { Name = "금요 기도회", Time = "매주 금요일 저녁 7:30", Place = "본당", Note = "", SortOrder = 4 },
                        new() { Name = "새벽 기도회", Time = "매일 오전 5:30", Place = "본당", Note = "", SortOrder = 5 }
                    }
                },
                new() {
                    Title = "교육부서 예배",
                    IsVisible = true,
                    SortOrder = 2,
                    Rows = new List<WorshipRow>
                    {
                        new() { Name = "유아부", Time = "매주 일요일 오전 11:00", Place = "유아부실", Note = "", SortOrder = 1 },
                        new() { Name = "유치부", Time = "매주 일요일 오전 11:00", Place = "유치부실", Note = "", SortOrder = 2 },
                        new() { Name = "유년부", Time = "매주 일요일 오전 11:00", Place = "유년부실", Note = "", SortOrder = 3 },
                        new() { Name = "초등부", Time = "매주 일요일 오전 11:00", Place = "초등부실", Note = "", SortOrder = 4 },
                        new() { Name = "중고등부", Time = "매주 일요일 오전 11:00", Place = "중고등부실", Note = "", SortOrder = 5 }
                    }
                }
            },

            OnlineVisible = true,
            OnlineTitle = "온라인 예배 안내",
            OnlineText = "온라인으로도 실시간 예배에 참여하실 수 있습니다. 주일 1·2부 예배는 유튜브 채널을 통해 생중계됩니다.",
            OnlineUrl = churchInfo?.YoutubeUrl ?? "https://youtube.com/@example"
        };

        return model;
    }

    public async Task<object> GetPeopleViewModelAsync()
    {
        // DB에서 데이터 조회
        var allPeople = await _context.People
            .Where(p => p.IsVisible)
            .OrderBy(p => p.SortOrder)
            .ToListAsync();

        // 담임목사 (대표)
        var headPastor = allPeople.FirstOrDefault(p => p.IsHead);

        var model = new AboutPeopleVm
        {
            PageTitle = "섬기는 사람들",
            PageSubtitle = "안디옥 교회를 섬기는 교역자와 직분자를 소개합니다.",
            Tabs = GetAboutTabs(),

            LeadPastor = headPastor != null ? new LeadPastorVm
            {
                Name = headPastor.Name,
                Title = headPastor.Role ?? headPastor.Title,
                Quote = !string.IsNullOrEmpty(headPastor.Quote) ? $"\"{headPastor.Quote}\"" : "",
                Desc = headPastor.Intro ?? "",
                Photo = headPastor.PhotoUrl ?? ""
            } : new LeadPastorVm
            {
                Name = "홍길동",
                Title = "담임목사",
                Quote = "\"말씀과 기도로 섬기며, 한 영혼을 귀하게 여기는 목회를 꿈꿉니다.\"",
                Desc = "○○신학대학원 졸업, 대한예수교장로회(합동) 목사 안수, 안디옥 교회 부목사 역임",
                Photo = ""
            },

            Categories = new List<PeopleCategory>
            {
                new() { Key = "all", Label = "전체", IsVisible = true, SortOrder = 1 },
                new() { Key = "pastor", Label = "교역자", IsVisible = true, SortOrder = 2 },
                new() { Key = "elder", Label = "장로", IsVisible = true, SortOrder = 3 },
                new() { Key = "deacon", Label = "안수집사", IsVisible = true, SortOrder = 4 },
                new() { Key = "deaconess", Label = "권사", IsVisible = true, SortOrder = 5 }
            },

            People = allPeople
                .Where(p => !p.IsHead)
                .Select(p => new PersonVm
                {
                    Category = p.Category,
                    Role = p.Title,
                    Name = p.Name,
                    Ministry = p.Ministry ?? "",
                    Photo = p.PhotoUrl ?? "",
                    IsVisible = p.IsVisible,
                    SortOrder = p.SortOrder
                })
                .ToList()
        };

        return model;
    }

    public async Task<object> GetLocationViewModelAsync()
    {
        var churchInfo = await _context.ChurchInfos.FirstOrDefaultAsync();

        var model = new AboutLocationVm
        {
            PageTitle = "오시는 길",
            PageSubtitle = "안디옥 교회를 찾아오시는 길을 안내합니다.",
            Tabs = GetAboutTabs(),

            MapEmbed = churchInfo?.MapEmbed ?? "[지도 자리표시자]",
            Latitude = churchInfo?.Latitude ?? 34.47583905316223,
            Longitude = churchInfo?.Longitude ?? 126.47554034813862,
            Address = churchInfo?.Address ?? "전라남도 해남군 화산면 관동리 441-1",
            Phone = churchInfo?.Phone ?? "02-1234-5678",
            Email = churchInfo?.Email ?? "contact@example-church.com",

            Transport = new List<TransportItem>
            {
                new() { Icon = "🚌", Title = "버스", Body = "○○정류장 하차 (간선 123, 456 / 지선 7890)", SortOrder = 1 },
                new() { Icon = "🚇", Title = "지하철", Body = "2호선 ○○역 3번 출구에서 도보 5분", SortOrder = 2 },
                new() { Icon = "🚗", Title = "자가용", Body = "건물 지하 1층 주차장 이용 가능 (주일 무료)", SortOrder = 3 }
            }
        };

        return model;
    }
}
