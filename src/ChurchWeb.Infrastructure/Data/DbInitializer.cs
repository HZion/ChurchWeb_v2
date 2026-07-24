using ChurchWeb.Core.Entities.Common;
using ChurchWeb.Core.Entities.Home;
using ChurchWeb.Core.Entities.Identity;
using ChurchWeb.Core.Entities.News;
using ChurchWeb.Core.Entities.People;
using ChurchWeb.Core.Entities.Sermons;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ChurchWeb.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<AppDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // 마이그레이션 적용
        await context.Database.MigrateAsync();

        // 관리자 역할 및 계정 생성
        await SeedAdminUserAsync(userManager, roleManager);

        // 이미 데이터가 있으면 시드하지 않음
        if (await context.ChurchInfos.AnyAsync())
        {
            return;
        }

        // 교회 정보
        var churchInfo = new ChurchInfo
        {
            ChurchName = "안디옥 교회",
            Denomination = "대한예수교장로회(합동)",
            Address = "○○시 ○○구 ○○로 1, 304호",
            Phone = "02-000-0000",
            Email = "church@example.com",
            YoutubeUrl = "",
            OnlineOfferingAccount = "○○은행 000-00-00000 (안디옥 교회)",
            MapEmbed = "",
            WorshipTimesJson = "주일 1부 09:00 · 2부 11:00 · 수요 19:30 · 새벽 05:30",
            FooterText = "COPYRIGHT © 안디옥 교회 · 대한예수교장로회(합동) ALL RIGHTS RESERVED.",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.ChurchInfos.Add(churchInfo);

        // 홈 섹션
        var homeSections = new[]
        {
            new HomeSection { Key = "hero", Title = "히어로", IsVisible = true, SortOrder = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new HomeSection { Key = "vision", Title = "올해의 표어", IsVisible = true, SortOrder = 2, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new HomeSection { Key = "sermons", Title = "말씀과 찬양", IsVisible = true, SortOrder = 3, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new HomeSection { Key = "quick", Title = "바로가기", IsVisible = true, SortOrder = 4, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new HomeSection { Key = "media", Title = "소개 영상", IsVisible = true, SortOrder = 5, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new HomeSection { Key = "pastor", Title = "담임목사 인사말", IsVisible = true, SortOrder = 6, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new HomeSection { Key = "news", Title = "교회 소식", IsVisible = true, SortOrder = 7, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new HomeSection { Key = "locate", Title = "오시는 길", IsVisible = true, SortOrder = 8, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        context.HomeSections.AddRange(homeSections);

        // 히어로 슬라이드
        var heroSlides = new[]
        {
            new HeroSlide
            {
                BackgroundType = "gradient",
                Background = "linear-gradient(180deg,var(--hero-o1),var(--hero-o2)),radial-gradient(120% 100% at 65% 20%,#7a5f42,#4a3626 45%,#2c2018)",
                Kicker = "WELCOME TO OUR CHURCH",
                Title = "모든 것이 합력하여<br>선을 이루느니라",
                Subtitle = "말씀 위에 세워진 공동체, 안디옥 교회에 오신 것을 환영합니다.",
                PrimaryBtnText = "예배 안내 ↗",
                PrimaryBtnUrl = "#quick",
                SecondaryBtnText = "교회 소개 영상",
                SecondaryBtnUrl = "#media",
                IsVisible = true,
                SortOrder = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new HeroSlide
            {
                BackgroundType = "gradient",
                Background = "linear-gradient(180deg,var(--hero-o1),var(--hero-o2)),radial-gradient(120% 100% at 28% 30%,#6b5238,#3c2c1e 50%,#241a12)",
                Kicker = "2026 · 올해의 표어",
                Title = "오직 주의 말씀 안에서<br>날마다 새롭게 되라",
                Subtitle = "한 해 동안 함께 붙드는 말씀으로 나아갑니다.",
                PrimaryBtnText = "올해의 비전 보기",
                PrimaryBtnUrl = "#vision",
                IsVisible = true,
                SortOrder = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new HeroSlide
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
                SortOrder = 3,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new HeroSlide
            {
                BackgroundType = "image",
                ImageUrl = "data:image/svg+xml,%3Csvg%20xmlns='http://www.w3.org/2000/svg'%20width='1600'%20height='900'%3E%3Cdefs%3E%3ClinearGradient%20id='g'%20x1='0'%20y1='0'%20x2='0'%20y2='1'%3E%3Cstop%20offset='0'%20stop-color='%23d8c3a0'/%3E%3Cstop%20offset='1'%20stop-color='%236b5236'/%3E%3C/linearGradient%3E%3C/defs%3E%3Crect%20width='1600'%20height='900'%20fill='url(%23g)'/%3E%3Ccircle%20cx='1160'%20cy='210'%20r='120'%20fill='%23e9d9b8'%20opacity='0.45'/%3E%3Crect%20x='770'%20y='260'%20width='60'%20height='380'%20fill='%233a2a1c'/%3E%3Crect%20x='688'%20y='344'%20width='224'%20height='58'%20fill='%233a2a1c'/%3E%3C/svg%3E",
                OverlayOpacity = 0.5,
                Kicker = "사진 배경 예시 · PHOTO",
                Title = "사진 위에 얹힌<br>히어로 문구입니다",
                Subtitle = "배경이 사진이어도 어두운 오버레이 덕분에 글자가 선명하게 보입니다.",
                PrimaryBtnText = "자세히 보기",
                PrimaryBtnUrl = "#",
                IsVisible = true,
                SortOrder = 4,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
        context.HeroSlides.AddRange(heroSlides);

        // 비전
        var vision = new Vision
        {
            Year = "2026",
            MottoText = "오직 주의 말씀 안에서\n날마다 새롭게 되라",
            ScriptureRef = "",
            IsVisible = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Visions.Add(vision);
        await context.SaveChangesAsync();  // Vision ID 생성 위해

        var visionPractices = new[]
        {
            new VisionPractice { VisionId = vision.Id, Number = "01", Text = "매일 말씀 묵상과 기도", SortOrder = 1 },
            new VisionPractice { VisionId = vision.Id, Number = "02", Text = "이웃을 향한 섬김과 나눔", SortOrder = 2 },
            new VisionPractice { VisionId = vision.Id, Number = "03", Text = "한 영혼을 향한 전도", SortOrder = 3 }
        };
        context.VisionPractices.AddRange(visionPractices);

        // 담임목사 인사말
        var pastorGreeting = new PastorGreeting
        {
            Role = "홍길동 담임 목사",
            Name = "홍길동",
            Title = "주님의 이름으로\n환영하고 축복합니다.",
            Message = "주님이 원하시는 교회! 그런 교회가 되기 위해 성경을 바탕으로 성도의 신앙교육과 다음세대의 영적 양육, 그리고 지역 공동체와 깊은 유대감 있는 연계를 이어가고 있습니다.",
            PhotoUrl = "",
            MoreUrl = "#",
            IsVisible = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.PastorGreetings.Add(pastorGreeting);

        // 설교
        var sermons = new[]
        {
            new Sermon
            {
                Category = "sunday",
                Verse = "시편 23:1",
                Title = "여호와는 나의 목자시니",
                Preacher = "홍길동 목사",
                PreachedOn = new DateTime(2026, 7, 12, 0, 0, 0, DateTimeKind.Utc),
                Duration = "42:10",
                ThumbnailUrl = "",
                YoutubeUrl = "",
                Summary = "",
                IsVisible = true,
                SortOrder = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Sermon
            {
                Category = "sunday",
                Verse = "로마서 8:28",
                Title = "모든 것이 합력하여 선을",
                Preacher = "홍길동 목사",
                PreachedOn = new DateTime(2026, 7, 5, 0, 0, 0, DateTimeKind.Utc),
                Duration = "38:55",
                ThumbnailUrl = "",
                YoutubeUrl = "",
                Summary = "",
                IsVisible = true,
                SortOrder = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Sermon
            {
                Category = "special",
                Verse = "마태복음 5:14",
                Title = "너희는 세상의 빛이라",
                Preacher = "홍길동 목사",
                PreachedOn = new DateTime(2026, 6, 28, 0, 0, 0, DateTimeKind.Utc),
                Duration = "45:30",
                ThumbnailUrl = "",
                YoutubeUrl = "",
                Summary = "",
                IsVisible = true,
                SortOrder = 3,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Sermon
            {
                Category = "sunday",
                Verse = "요한복음 3:16",
                Title = "하나님의 사랑",
                Preacher = "홍길동 목사",
                PreachedOn = new DateTime(2026, 6, 21, 0, 0, 0, DateTimeKind.Utc),
                Duration = "40:02",
                ThumbnailUrl = "",
                YoutubeUrl = "",
                Summary = "",
                IsVisible = true,
                SortOrder = 4,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
        context.Sermons.AddRange(sermons);

        // 소식
        var notices = new[]
        {
            new Notice
            {
                CategoryKey = "church",
                Title = "여름 성경학교 신청 안내",
                Author = "관리자",
                PostedOn = new DateTime(2026, 7, 15, 0, 0, 0, DateTimeKind.Utc),
                Views = 0,
                IsPinned = true,
                BodyHtml = "<p>여름 성경학교 신청을 받습니다.</p>",
                IsVisible = true,
                SortOrder = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Notice
            {
                CategoryKey = "church",
                Title = "7월 정기 제직회 안내",
                Author = "관리자",
                PostedOn = new DateTime(2026, 7, 10, 0, 0, 0, DateTimeKind.Utc),
                Views = 0,
                IsPinned = false,
                BodyHtml = "<p>7월 제직회를 안내합니다.</p>",
                IsVisible = true,
                SortOrder = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
        context.Notices.AddRange(notices);

        // 주보 (Bulletins)
        var bulletins = new[]
        {
            new Bulletin
            {
                Title = "2026년 7월 12일 주보",
                PublishedDate = new DateTime(2026, 7, 12, 0, 0, 0, DateTimeKind.Utc),
                FileUrl = "",
                CoverImageUrl = "",
                IsVisible = true,
                SortOrder = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Bulletin
            {
                Title = "2026년 7월 5일 주보",
                PublishedDate = new DateTime(2026, 7, 5, 0, 0, 0, DateTimeKind.Utc),
                FileUrl = "",
                CoverImageUrl = "",
                IsVisible = true,
                SortOrder = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
        context.Bulletins.AddRange(bulletins);
        await context.SaveChangesAsync(); // Bulletin 먼저 저장하여 ID 생성

        // 주보 페이지 (BulletinPages)
        var bulletinPages = new List<BulletinPage>();

        // 첫 번째 주보 (7월 12일) - 4페이지
        for (int i = 1; i <= 4; i++)
        {
            bulletinPages.Add(new BulletinPage
            {
                BulletinId = bulletins[0].Id,
                ImageUrl = $"/images/bulletins/2026-07-12/page{i}.jpg",
                SortOrder = i
            });
        }

        // 두 번째 주보 (7월 5일) - 4페이지
        for (int i = 1; i <= 4; i++)
        {
            bulletinPages.Add(new BulletinPage
            {
                BulletinId = bulletins[1].Id,
                ImageUrl = $"/images/bulletins/2026-07-05/page{i}.jpg",
                SortOrder = i
            });
        }

        context.BulletinPages.AddRange(bulletinPages);

        // 갤러리 앨범 (Albums)
        var albums = new[]
        {
            new Album
            {
                Title = "2026 신년 특별새벽기도회",
                EventDate = new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc),
                Category = "행사",
                Description = "새해를 맞아 특별새벽기도회를 드렸습니다.",
                CoverImageUrl = "",
                Year = 2026,
                IsVisible = true,
                SortOrder = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Album
            {
                Title = "2025 성탄절 축하예배",
                EventDate = new DateTime(2025, 12, 25, 0, 0, 0, DateTimeKind.Utc),
                Category = "예배",
                Description = "성탄절을 맞아 특별 축하예배를 드렸습니다.",
                CoverImageUrl = "",
                Year = 2025,
                IsVisible = true,
                SortOrder = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
        context.Albums.AddRange(albums);
        await context.SaveChangesAsync(); // Album 먼저 저장하여 ID 생성

        // 갤러리 앨범 사진 (AlbumPhotos)
        var albumPhotos = new List<AlbumPhoto>();

        // 첫 번째 앨범 (2026 신년 특별새벽기도회) - 6장의 사진
        for (int i = 1; i <= 6; i++)
        {
            albumPhotos.Add(new AlbumPhoto
            {
                AlbumId = albums[0].Id,
                ImageUrl = $"/images/gallery/2026-newyear/{i}.jpg",
                Caption = i == 1 ? "새벽기도회 전경" : "",
                SortOrder = i
            });
        }

        // 두 번째 앨범 (2025 성탄절 축하예배) - 8장의 사진
        for (int i = 1; i <= 8; i++)
        {
            albumPhotos.Add(new AlbumPhoto
            {
                AlbumId = albums[1].Id,
                ImageUrl = $"/images/gallery/2025-christmas/{i}.jpg",
                Caption = i == 1 ? "성탄절 예배 모습" : "",
                SortOrder = i
            });
        }

        context.AlbumPhotos.AddRange(albumPhotos);

        // 교회 일정 (CalendarEvents)
        var calendarEvents = new[]
        {
            new CalendarEvent
            {
                EventDate = new DateTime(2026, 7, 20, 0, 0, 0, DateTimeKind.Utc),
                Title = "여름 성경학교",
                EventType = "event",
                Time = "오전 9:00 - 오후 3:00",
                Description = "여름 성경학교가 진행됩니다.",
                IsVisible = true,
                SortOrder = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new CalendarEvent
            {
                EventDate = new DateTime(2026, 7, 25, 0, 0, 0, DateTimeKind.Utc),
                Title = "제직회",
                EventType = "event",
                Time = "오후 7:30",
                Description = "정기 제직회가 있습니다.",
                IsVisible = true,
                SortOrder = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new CalendarEvent
            {
                EventDate = new DateTime(2026, 8, 10, 0, 0, 0, DateTimeKind.Utc),
                Title = "여름 수련회",
                EventType = "event",
                Time = "1박 2일",
                Description = "청년부 여름 수련회가 진행됩니다.",
                IsVisible = true,
                SortOrder = 3,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
        context.CalendarEvents.AddRange(calendarEvents);

        // 추가 설교 데이터 (페이징 테스트용)
        var additionalSermons = new List<Sermon>();
        for (int i = 5; i <= 20; i++)
        {
            additionalSermons.Add(new Sermon
            {
                Category = i % 3 == 0 ? "special" : "sunday",
                Verse = $"시편 {100 + i}:{i}",
                Title = $"하나님의 은혜와 사랑 ({i})",
                Preacher = "홍길동 목사",
                PreachedOn = new DateTime(2026, 7, 12, 0, 0, 0, DateTimeKind.Utc).AddDays(-i * 7),
                Duration = $"{35 + i}:00",
                ThumbnailUrl = "",
                YoutubeUrl = "",
                Summary = "",
                IsVisible = true,
                SortOrder = i + 4,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }
        context.Sermons.AddRange(additionalSermons);

        // 추가 소식 데이터
        var additionalNotices = new List<Notice>();
        for (int i = 3; i <= 10; i++)
        {
            additionalNotices.Add(new Notice
            {
                CategoryKey = i % 2 == 0 ? "church" : "member",
                Title = $"교회 소식 {i}",
                Author = "관리자",
                PostedOn = new DateTime(2026, 7, 15, 0, 0, 0, DateTimeKind.Utc).AddDays(-i),
                Views = 0,
                IsPinned = false,
                BodyHtml = $"<p>교회 소식 {i} 내용입니다.</p>",
                IsVisible = true,
                SortOrder = i,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }
        context.Notices.AddRange(additionalNotices);

        // 섬기는 사람들 (People)
        var people = new[]
        {
            // 담임목사 (대표)
            new Person
            {
                IsHead = true,
                Name = "홍길동",
                Title = "목사",
                Role = "담임목사",
                Category = "pastor",
                Ministry = null,
                PhotoUrl = null,
                Intro = "○○신학대학원 졸업, 대한예수교장로회(합동) 목사 안수, 안디옥 교회 부목사 역임",
                Quote = "말씀과 기도로 섬기며, 한 영혼을 귀하게 여기는 목회를 꿈꿉니다.",
                IsVisible = true,
                SortOrder = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            // 교역자
            new Person
            {
                IsHead = false,
                Name = "김철수",
                Title = "부목사",
                Role = null,
                Category = "pastor",
                Ministry = "교육부서",
                PhotoUrl = null,
                Intro = null,
                Quote = null,
                IsVisible = true,
                SortOrder = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Person
            {
                IsHead = false,
                Name = "이영희",
                Title = "전도사",
                Role = null,
                Category = "pastor",
                Ministry = "유아유치부",
                PhotoUrl = null,
                Intro = null,
                Quote = null,
                IsVisible = true,
                SortOrder = 3,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            // 장로
            new Person
            {
                IsHead = false,
                Name = "박민수",
                Title = "장로",
                Role = null,
                Category = "elder",
                Ministry = null,
                PhotoUrl = null,
                Intro = null,
                Quote = null,
                IsVisible = true,
                SortOrder = 4,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Person
            {
                IsHead = false,
                Name = "최정호",
                Title = "장로",
                Role = null,
                Category = "elder",
                Ministry = null,
                PhotoUrl = null,
                Intro = null,
                Quote = null,
                IsVisible = true,
                SortOrder = 5,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            // 안수집사
            new Person
            {
                IsHead = false,
                Name = "정대희",
                Title = "안수집사",
                Role = null,
                Category = "deacon",
                Ministry = null,
                PhotoUrl = null,
                Intro = null,
                Quote = null,
                IsVisible = true,
                SortOrder = 6,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Person
            {
                IsHead = false,
                Name = "강민호",
                Title = "안수집사",
                Role = null,
                Category = "deacon",
                Ministry = null,
                PhotoUrl = null,
                Intro = null,
                Quote = null,
                IsVisible = true,
                SortOrder = 7,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            // 권사
            new Person
            {
                IsHead = false,
                Name = "김순자",
                Title = "권사",
                Role = null,
                Category = "deaconess",
                Ministry = null,
                PhotoUrl = null,
                Intro = null,
                Quote = null,
                IsVisible = true,
                SortOrder = 8,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Person
            {
                IsHead = false,
                Name = "이경숙",
                Title = "권사",
                Role = null,
                Category = "deaconess",
                Ministry = null,
                PhotoUrl = null,
                Intro = null,
                Quote = null,
                IsVisible = true,
                SortOrder = 9,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
        context.People.AddRange(people);

        await context.SaveChangesAsync();
    }

    private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // 관리자 역할 생성
        const string adminRole = "Admin";
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        // 관리자 계정 생성
        const string adminEmail = "admin@church.com";
        const string adminUsername = "admin";
        var adminUser = await userManager.FindByNameAsync(adminUsername);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminUsername,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "관리자",
                CreatedAt = DateTime.UtcNow
            };

            // 초기 비밀번호: Admin@2026!
            var result = await userManager.CreateAsync(adminUser, "Admin@2026!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }
        else
        {
            // 기존 관리자 계정이 있으면 Admin 역할 확인
            var isInRole = await userManager.IsInRoleAsync(adminUser, adminRole);
            if (!isInRole)
            {
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }
    }
}
