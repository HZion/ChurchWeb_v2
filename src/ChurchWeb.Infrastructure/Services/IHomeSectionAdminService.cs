using ChurchWeb.Core.Entities.Common;

namespace ChurchWeb.Infrastructure.Services;

public interface IHomeSectionAdminService
{
    /// <summary>
    /// 모든 홈 섹션 가져오기
    /// </summary>
    Task<List<HomeSection>> GetAllHomeSectionsAsync();

    /// <summary>
    /// ID로 홈 섹션 가져오기
    /// </summary>
    Task<HomeSection?> GetHomeSectionByIdAsync(int id);

    /// <summary>
    /// 홈 섹션 업데이트
    /// </summary>
    Task<HomeSection> UpdateHomeSectionAsync(HomeSection homeSection);

    /// <summary>
    /// 홈 섹션 표시/숨김 토글
    /// </summary>
    Task<bool> ToggleVisibilityAsync(int id);

    /// <summary>
    /// 홈 섹션 정렬 순서 업데이트
    /// </summary>
    Task<bool> UpdateSortOrderAsync(int id, int newSortOrder);

    /// <summary>
    /// 여러 홈 섹션의 정렬 순서를 일괄 업데이트
    /// </summary>
    Task<bool> UpdateSortOrdersBulkAsync(Dictionary<int, int> sortOrders);
}
