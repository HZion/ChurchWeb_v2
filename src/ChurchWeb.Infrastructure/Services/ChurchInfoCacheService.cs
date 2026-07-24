using ChurchWeb.Core.Entities.Common;
using ChurchWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ChurchWeb.Infrastructure.Services;

public interface IChurchInfoCacheService : IChurchInfoService
{
    Task RefreshCacheAsync();
}

public class ChurchInfoCacheService : IChurchInfoCacheService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ChurchInfoCacheService> _logger;
    private ChurchInfo? _cachedChurchInfo;
    private readonly SemaphoreSlim _cacheLock = new(1, 1);
    private bool _isInitialized = false;

    public ChurchInfoCacheService(IServiceScopeFactory scopeFactory, ILogger<ChurchInfoCacheService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task<ChurchInfo?> GetChurchInfoAsync()
    {
        if (!_isInitialized)
        {
            await RefreshCacheAsync();
        }

        return _cachedChurchInfo;
    }

    public async Task<ChurchInfo> SaveChurchInfoAsync(ChurchInfo churchInfo)
    {
        await _cacheLock.WaitAsync();
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var existing = await context.ChurchInfos.FirstOrDefaultAsync();

            if (existing == null)
            {
                // 신규 생성
                churchInfo.CreatedAt = DateTime.UtcNow;
                churchInfo.UpdatedAt = DateTime.UtcNow;
                context.ChurchInfos.Add(churchInfo);
            }
            else
            {
                // 기존 레코드 업데이트
                existing.ChurchName = churchInfo.ChurchName;
                existing.Denomination = churchInfo.Denomination;
                existing.Address = churchInfo.Address;
                existing.Phone = churchInfo.Phone;
                existing.Email = churchInfo.Email;
                existing.YoutubeUrl = churchInfo.YoutubeUrl;
                existing.HomepageUrl = churchInfo.HomepageUrl;
                existing.OnlineOfferingAccount = churchInfo.OnlineOfferingAccount;
                existing.MapEmbed = churchInfo.MapEmbed;
                existing.Latitude = churchInfo.Latitude;
                existing.Longitude = churchInfo.Longitude;
                existing.WorshipTimesJson = churchInfo.WorshipTimesJson;
                existing.FooterText = churchInfo.FooterText;

                // Phase 1 필드
                existing.AnnualSlogan = churchInfo.AnnualSlogan;
                existing.PracticesJson = churchInfo.PracticesJson;
                existing.PromoVideoUrl = churchInfo.PromoVideoUrl;
                existing.OutreachCardImageUrl = churchInfo.OutreachCardImageUrl;
                existing.OutreachCardPdfUrl = churchInfo.OutreachCardPdfUrl;

                // Phase 2: 온라인 전도카드 필드
                existing.OutreachWelcomeMessage = churchInfo.OutreachWelcomeMessage;
                existing.OutreachShortUrl = churchInfo.OutreachShortUrl;
                existing.OutreachMapLink = churchInfo.OutreachMapLink;

                existing.UpdatedAt = DateTime.UtcNow;

                context.ChurchInfos.Update(existing);
            }

            await context.SaveChangesAsync();

            // 캐시 업데이트
            _cachedChurchInfo = existing ?? churchInfo;
            _isInitialized = true;

            _logger.LogInformation("ChurchInfo saved and cache updated");

            return _cachedChurchInfo;
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    public async Task RefreshCacheAsync()
    {
        await _cacheLock.WaitAsync();
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            _cachedChurchInfo = await context.ChurchInfos.AsNoTracking().FirstOrDefaultAsync();
            _isInitialized = true;
            _logger.LogInformation("ChurchInfo cache initialized");
        }
        finally
        {
            _cacheLock.Release();
        }
    }
}
