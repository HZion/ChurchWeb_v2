using ChurchWeb.Application.Services;
using ChurchWeb.Infrastructure.Data;
using ChurchWeb.Web.Models.Worship;
using Microsoft.EntityFrameworkCore;

namespace ChurchWeb.Web.Services;

public class SermonsService : ISermonsService
{
    private readonly AppDbContext _context;

    public SermonsService(AppDbContext context)
    {
        _context = context;
    }

    private List<WorshipTabVm> GetWorshipTabs()
    {
        return new List<WorshipTabVm>
        {
            new WorshipTabVm { Key = "sunday", Title = "주일 설교", Url = "/sermons/sunday", IsVisible = true, SortOrder = 1 },
            new WorshipTabVm { Key = "special", Title = "특별 설교", Url = "/sermons/special", IsVisible = true, SortOrder = 2 }
        };
    }

    public async Task<object> GetSermonListAsync(string category, int page, int pageSize, string? search = null)
    {
        var query = _context.Sermons
            .Where(s => s.IsVisible && s.Category == category);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(s =>
                s.Title.Contains(search) ||
                s.Verse.Contains(search) ||
                s.Preacher.Contains(search));
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var sermons = await query
            .OrderByDescending(s => s.PreachedOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new SermonCardVm
            {
                Id = s.Id,
                Category = s.Category,
                Verse = s.Verse,
                Title = s.Title,
                Preacher = s.Preacher,
                Date = s.PreachedOn.ToString("yyyy.MM.dd"),
                Duration = s.Duration,
                ThumbUrl = s.ThumbnailUrl,
                YoutubeUrl = s.YoutubeUrl
            })
            .ToListAsync();

        var model = new SermonListVm
        {
            PageTitle = category == "sunday" ? "주일 설교" : "특별 설교",
            PageSubtitle = category == "sunday" ? "주일 예배 설교 영상을 시청하세요" : "특별 예배 및 부흥회 설교 영상을 시청하세요",
            Tabs = GetWorshipTabs(),
            Sermons = sermons,
            SearchQuery = search,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages,
            TotalCount = totalCount
        };

        return model;
    }

    public async Task<object?> GetSermonDetailAsync(int id)
    {
        var sermon = await _context.Sermons
            .Where(s => s.IsVisible && s.Id == id)
            .FirstOrDefaultAsync();

        if (sermon == null)
            return null;

        // 같은 카테고리의 다른 설교들 가져오기 (이전/다음/관련 설교용)
        var sameCategory = await _context.Sermons
            .Where(s => s.IsVisible && s.Category == sermon.Category && s.Id != id)
            .OrderByDescending(s => s.PreachedOn)
            .Select(s => new SermonCardVm
            {
                Id = s.Id,
                Category = s.Category,
                Verse = s.Verse,
                Title = s.Title,
                Preacher = s.Preacher,
                Date = s.PreachedOn.ToString("yyyy.MM.dd"),
                Duration = s.Duration,
                ThumbUrl = s.ThumbnailUrl,
                YoutubeUrl = s.YoutubeUrl
            })
            .ToListAsync();

        var currentIndex = sameCategory.FindIndex(s => s.Id == id);

        var model = new SermonDetailVm
        {
            Id = sermon.Id,
            Category = sermon.Category,
            Verse = sermon.Verse,
            Title = sermon.Title,
            Preacher = sermon.Preacher,
            Date = sermon.PreachedOn.ToString("yyyy.MM.dd"),
            Duration = sermon.Duration,
            YoutubeUrl = sermon.YoutubeUrl,
            Description = !string.IsNullOrEmpty(sermon.Summary)
                ? sermon.Summary
                : $"이 설교는 {sermon.Verse}을 본문으로 하나님의 말씀을 전합니다. 우리 삶 속에서 하나님의 뜻을 발견하고 실천할 수 있도록 이끌어주는 귀한 메시지입니다.",
            Attachments = new List<SermonAttachment>(),  // TODO: 나중에 첨부파일 기능 추가
            Tags = new List<string>(),  // TODO: 나중에 태그 기능 추가
            Tabs = GetWorshipTabs(),
            Prev = currentIndex > 0 ? sameCategory[currentIndex - 1] : null,
            Next = currentIndex < sameCategory.Count - 1 ? sameCategory[currentIndex + 1] : null,
            RelatedSermons = sameCategory.Where(s => s.Id != id).Take(3).ToList()
        };

        return model;
    }
}
