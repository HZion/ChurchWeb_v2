using ChurchWeb.Core.Entities.Sermons;

namespace ChurchWeb.Infrastructure.Services;

public interface ISermonAdminService
{
    Task<(List<Sermon> Items, int TotalCount)> GetPagedSermonsAsync(
        int page = 1,
        int pageSize = 20,
        string? searchTerm = null,
        string? category = null,
        bool? isVisible = null,
        string sortBy = "PreachedOn",
        bool sortDescending = true);

    Task<Sermon?> GetSermonByIdAsync(int id);
    Task<Sermon> CreateSermonAsync(Sermon sermon);
    Task<Sermon> UpdateSermonAsync(Sermon sermon);
    Task<bool> DeleteSermonAsync(int id);
    Task<bool> ToggleVisibilityAsync(int id);
    Task<bool> UpdateSortOrderAsync(int id, int newSortOrder);
}
