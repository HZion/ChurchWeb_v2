using ChurchWeb.Core.Entities.Common;

namespace ChurchWeb.Infrastructure.Services;

public interface IMenuAdminService
{
    /// <summary>
    /// 모든 메뉴 항목 가져오기 (계층 구조 유지)
    /// </summary>
    Task<List<NavMenuItem>> GetAllMenuItemsAsync();

    /// <summary>
    /// 최상위 메뉴만 가져오기
    /// </summary>
    Task<List<NavMenuItem>> GetTopLevelMenuItemsAsync();

    /// <summary>
    /// ID로 메뉴 항목 가져오기
    /// </summary>
    Task<NavMenuItem?> GetMenuItemByIdAsync(int id);

    /// <summary>
    /// 메뉴 항목 생성
    /// </summary>
    Task<NavMenuItem> CreateMenuItemAsync(NavMenuItem menuItem);

    /// <summary>
    /// 메뉴 항목 업데이트
    /// </summary>
    Task<NavMenuItem> UpdateMenuItemAsync(NavMenuItem menuItem);

    /// <summary>
    /// 메뉴 항목 삭제
    /// </summary>
    Task<bool> DeleteMenuItemAsync(int id);

    /// <summary>
    /// 메뉴 항목 표시/숨김 토글
    /// </summary>
    Task<bool> ToggleVisibilityAsync(int id);

    /// <summary>
    /// 메뉴 항목 정렬 순서 업데이트
    /// </summary>
    Task<bool> UpdateSortOrderAsync(int id, int newSortOrder);

    /// <summary>
    /// 여러 메뉴 항목의 정렬 순서를 일괄 업데이트
    /// </summary>
    Task<bool> UpdateSortOrdersBulkAsync(Dictionary<int, int> sortOrders);
}
