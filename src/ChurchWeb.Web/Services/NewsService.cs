using ChurchWeb.Application.Services;
using ChurchWeb.Infrastructure.Data;
using ChurchWeb.Web.Models.News;
using Microsoft.EntityFrameworkCore;

namespace ChurchWeb.Web.Services;

public class NewsService : INewsService
{
    private readonly AppDbContext _context;

    public NewsService(AppDbContext context)
    {
        _context = context;
    }

    private List<NewsTabVm> GetNewsTabs()
    {
        return new List<NewsTabVm>
        {
            new() { Key = "bulletins", Title = "주보", Url = "/news/bulletins", IsVisible = true, SortOrder = 1 },
            new() { Key = "gallery", Title = "갤러리", Url = "/news/gallery", IsVisible = true, SortOrder = 2 },
            new() { Key = "notices", Title = "교회소식", Url = "/news/notices", IsVisible = true, SortOrder = 3 },
            new() { Key = "calendar", Title = "교회일정", Url = "/news/calendar", IsVisible = true, SortOrder = 4 }
        };
    }

    // ========== 주보 ==========

    public async Task<object> GetBulletinListAsync(int page, int pageSize)
    {
        var query = _context.Bulletins.Where(b => b.IsVisible);

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var bulletins = await query
            .OrderByDescending(b => b.PublishedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BulletinCardVm
            {
                Id = b.Id,
                Title = b.Title,
                CoverUrl = !string.IsNullOrEmpty(b.CoverImageUrl) ? b.CoverImageUrl : "/images/bulletin-default-cover.png",
                FileUrl = $"/news/bulletins/{b.Id}/download",
                RegDate = b.PublishedDate.ToString("yyyy.MM.dd")
            })
            .ToListAsync();

        var model = new BulletinListVm
        {
            Tabs = GetNewsTabs(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            Items = bulletins
        };

        return model;
    }

    public async Task<(byte[] Data, string ContentType, string FileName)?> GetBulletinPdfAsync(int id)
    {
        var bulletin = await _context.Bulletins
            .Where(b => b.IsVisible && b.Id == id)
            .Select(b => new { b.PdfData, b.ContentType, b.FileName })
            .FirstOrDefaultAsync();

        if (bulletin == null || bulletin.PdfData == null || bulletin.PdfData.Length == 0)
            return null;

        return (bulletin.PdfData, bulletin.ContentType, bulletin.FileName);
    }

    public async Task<object?> GetBulletinDetailAsync(int id)
    {
        var bulletin = await _context.Bulletins
            .Include(b => b.Pages)
            .Where(b => b.IsVisible && b.Id == id)
            .FirstOrDefaultAsync();

        if (bulletin == null)
            return null;

        // 이전/다음 주보
        var allBulletins = await _context.Bulletins
            .Where(b => b.IsVisible)
            .OrderByDescending(b => b.PublishedDate)
            .Select(b => new { b.Id, b.Title, b.PublishedDate })
            .ToListAsync();

        var currentIndex = allBulletins.FindIndex(b => b.Id == id);

        var model = new BulletinDetailVm
        {
            Tabs = GetNewsTabs(),
            Id = bulletin.Id,
            Title = bulletin.Title,
            RegDate = bulletin.PublishedDate.ToString("yyyy.MM.dd"),
            FileUrl = $"/news/bulletins/{bulletin.Id}/download",
            PageImages = bulletin.Pages
                .OrderBy(p => p.SortOrder)
                .Select(p => p.ImageUrl)
                .ToList(),
            Prev = currentIndex > 0 ? new BulletinCardVm
            {
                Id = allBulletins[currentIndex - 1].Id,
                Title = allBulletins[currentIndex - 1].Title
            } : null,
            Next = currentIndex < allBulletins.Count - 1 ? new BulletinCardVm
            {
                Id = allBulletins[currentIndex + 1].Id,
                Title = allBulletins[currentIndex + 1].Title
            } : null
        };

        return model;
    }

    // ========== 갤러리 ==========

    public async Task<object> GetGalleryListAsync(int? year, int page, int pageSize)
    {
        var query = _context.Albums.Where(a => a.IsVisible);

        if (year.HasValue)
        {
            query = query.Where(a => a.Year == year.Value);
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var albums = await query
            .OrderByDescending(a => a.EventDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new AlbumCardVm
            {
                Id = a.Id,
                Title = a.Title,
                CoverUrl = a.CoverImageUrl,
                PhotoCount = _context.AlbumPhotos.Count(p => p.AlbumId == a.Id),
                Date = a.EventDate.ToString("yyyy.MM.dd"),
                Year = a.Year
            })
            .ToListAsync();

        // 사용 가능한 연도 목록
        var years = await _context.Albums
            .Where(a => a.IsVisible)
            .Select(a => a.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToListAsync();

        var model = new GalleryListVm
        {
            Tabs = GetNewsTabs(),
            Years = years,
            SelectedYear = year,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            Albums = albums
        };

        return model;
    }

    public async Task<object?> GetAlbumDetailAsync(int id)
    {
        var album = await _context.Albums
            .Include(a => a.Photos)
            .Where(a => a.IsVisible && a.Id == id)
            .FirstOrDefaultAsync();

        if (album == null)
            return null;

        // 이전/다음 앨범
        var allAlbums = await _context.Albums
            .Where(a => a.IsVisible)
            .OrderByDescending(a => a.EventDate)
            .Select(a => new { a.Id, a.Title })
            .ToListAsync();

        var currentIndex = allAlbums.FindIndex(a => a.Id == id);

        var model = new AlbumDetailVm
        {
            Tabs = GetNewsTabs(),
            Id = album.Id,
            Title = album.Title,
            Date = album.EventDate.ToString("yyyy.MM.dd"),
            Category = album.Category,
            Description = album.Description,
            Photos = album.Photos
                .OrderBy(p => p.SortOrder)
                .Select(p => p.ImageUrl)
                .ToList(),
            Prev = currentIndex > 0 ? new AlbumCardVm
            {
                Id = allAlbums[currentIndex - 1].Id,
                Title = allAlbums[currentIndex - 1].Title
            } : null,
            Next = currentIndex < allAlbums.Count - 1 ? new AlbumCardVm
            {
                Id = allAlbums[currentIndex + 1].Id,
                Title = allAlbums[currentIndex + 1].Title
            } : null
        };

        return model;
    }

    // ========== 교회/교우 소식 ==========

    public async Task<object> GetNoticeListAsync(int page, int pageSize)
    {
        var query = _context.Notices.Where(n => n.IsVisible);

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var notices = await query
            .OrderByDescending(n => n.IsPinned)
            .ThenByDescending(n => n.PostedOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(n => new NoticeVm
            {
                Id = n.Id,
                Category = n.CategoryKey,
                Title = n.Title,
                Author = n.Author,
                Date = n.PostedOn.ToString("yyyy.MM.dd"),
                Views = n.Views,
                IsPinned = n.IsPinned
            })
            .ToListAsync();

        var model = new NoticeListVm
        {
            Tabs = GetNewsTabs(),
            Categories = new List<NoticeCategory>
            {
                new() { Key = "all", Label = "전체", IsVisible = true, SortOrder = 1 },
                new() { Key = "church", Label = "교회소식", IsVisible = true, SortOrder = 2 },
                new() { Key = "member", Label = "교우소식", IsVisible = true, SortOrder = 3 }
            },
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            Items = notices
        };

        return model;
    }

    public async Task<object?> GetNoticeDetailAsync(int id)
    {
        var notice = await _context.Notices
            .Include(n => n.Attachments)
            .Where(n => n.IsVisible && n.Id == id)
            .FirstOrDefaultAsync();

        if (notice == null)
            return null;

        // 조회수 증가
        notice.Views++;
        await _context.SaveChangesAsync();

        // 이전/다음 소식
        var allNotices = await _context.Notices
            .Where(n => n.IsVisible)
            .OrderByDescending(n => n.IsPinned)
            .ThenByDescending(n => n.PostedOn)
            .Select(n => new { n.Id, n.Title, n.CategoryKey })
            .ToListAsync();

        var currentIndex = allNotices.FindIndex(n => n.Id == id);

        var model = new NoticeDetailVm
        {
            Tabs = GetNewsTabs(),
            Id = notice.Id,
            Category = notice.CategoryKey == "church" ? "교회소식" : "교우소식",
            Title = notice.Title,
            Author = notice.Author,
            Date = notice.PostedOn.ToString("yyyy.MM.dd"),
            Views = notice.Views,
            BodyHtml = notice.BodyHtml,
            Attachments = notice.Attachments
                .OrderBy(a => a.SortOrder)
                .Select(a => new Attachment
                {
                    Name = a.FileName,
                    Url = a.FileUrl
                })
                .ToList(),
            Prev = currentIndex > 0 ? new NoticeVm
            {
                Id = allNotices[currentIndex - 1].Id,
                Title = allNotices[currentIndex - 1].Title,
                Category = allNotices[currentIndex - 1].CategoryKey
            } : null,
            Next = currentIndex < allNotices.Count - 1 ? new NoticeVm
            {
                Id = allNotices[currentIndex + 1].Id,
                Title = allNotices[currentIndex + 1].Title,
                Category = allNotices[currentIndex + 1].CategoryKey
            } : null
        };

        return model;
    }

    // ========== 교회 일정 ==========

    public async Task<object> GetCalendarAsync(int year, int month)
    {
        var targetDate = new DateTime(year, month, 1);

        // 해당 월의 이벤트 가져오기
        var monthStart = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        var events = await _context.CalendarEvents
            .Where(e => e.IsVisible &&
                        e.EventDate >= monthStart &&
                        e.EventDate <= monthEnd)
            .Select(e => new EventVm
            {
                Title = e.Title,
                Type = e.EventType,
                Time = e.Time,
                Date = e.EventDate
            })
            .ToListAsync();

        var model = new CalendarVm
        {
            Tabs = GetNewsTabs(),
            Year = year,
            Month = month,
            Days = GenerateCalendarDays(year, month, events),
            TodayEvents = events.Where(e => e.Date.Date == DateTime.Today).ToList(),
            MonthEvents = events
        };

        return model;
    }

    private List<CalendarDay> GenerateCalendarDays(int year, int month, List<EventVm> events)
    {
        var days = new List<CalendarDay>();
        var firstDay = new DateTime(year, month, 1);
        var lastDay = firstDay.AddMonths(1).AddDays(-1);
        var today = DateTime.Today;

        // 이전 달의 날짜들 (주 시작: 일요일)
        var startDayOfWeek = (int)firstDay.DayOfWeek;
        for (int i = startDayOfWeek - 1; i >= 0; i--)
        {
            var date = firstDay.AddDays(-i - 1);
            days.Add(new CalendarDay
            {
                Date = date,
                IsCurrentMonth = false,
                IsToday = false,
                IsSunday = date.DayOfWeek == DayOfWeek.Sunday,
                IsSaturday = date.DayOfWeek == DayOfWeek.Saturday
            });
        }

        // 현재 달의 날짜들
        for (int day = 1; day <= lastDay.Day; day++)
        {
            var date = new DateTime(year, month, day);
            var calDay = new CalendarDay
            {
                Date = date,
                IsCurrentMonth = true,
                IsToday = date == today,
                IsSunday = date.DayOfWeek == DayOfWeek.Sunday,
                IsSaturday = date.DayOfWeek == DayOfWeek.Saturday,
                Events = events.Where(e => e.Date.Date == date).ToList()
            };

            days.Add(calDay);
        }

        // 다음 달의 날짜들 (6주 완성)
        var remainingDays = 42 - days.Count; // 6주 = 42일
        for (int i = 1; i <= remainingDays; i++)
        {
            var date = lastDay.AddDays(i);
            days.Add(new CalendarDay
            {
                Date = date,
                IsCurrentMonth = false,
                IsToday = false,
                IsSunday = date.DayOfWeek == DayOfWeek.Sunday,
                IsSaturday = date.DayOfWeek == DayOfWeek.Saturday
            });
        }

        return days;
    }
}
