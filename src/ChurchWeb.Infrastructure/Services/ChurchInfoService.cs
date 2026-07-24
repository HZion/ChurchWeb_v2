using ChurchWeb.Core.Entities.Common;
using ChurchWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChurchWeb.Infrastructure.Services;

public class ChurchInfoService : IChurchInfoService
{
    private readonly AppDbContext _context;

    public ChurchInfoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ChurchInfo?> GetChurchInfoAsync()
    {
        // ChurchInfo는 단일 레코드만 존재
        return await _context.ChurchInfos.FirstOrDefaultAsync();
    }

    public async Task<ChurchInfo> SaveChurchInfoAsync(ChurchInfo churchInfo)
    {
        var existing = await GetChurchInfoAsync();

        if (existing == null)
        {
            // 신규 생성
            churchInfo.CreatedAt = DateTime.UtcNow;
            churchInfo.UpdatedAt = DateTime.UtcNow;
            _context.ChurchInfos.Add(churchInfo);
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

            _context.ChurchInfos.Update(existing);
        }

        await _context.SaveChangesAsync();
        return existing ?? churchInfo;
    }
}
