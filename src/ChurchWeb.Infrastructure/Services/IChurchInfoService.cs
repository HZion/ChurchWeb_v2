using ChurchWeb.Core.Entities.Common;

namespace ChurchWeb.Infrastructure.Services;

public interface IChurchInfoService
{
    /// <summary>
    /// 교회 정보 가져오기 (단일 레코드)
    /// </summary>
    Task<ChurchInfo?> GetChurchInfoAsync();

    /// <summary>
    /// 교회 정보 업데이트 (생성 또는 수정)
    /// </summary>
    Task<ChurchInfo> SaveChurchInfoAsync(ChurchInfo churchInfo);
}
