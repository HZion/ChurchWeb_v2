using ChurchWeb.Core.Entities.Outreach;

namespace ChurchWeb.Infrastructure.Services;

public interface IEvangelistAdminService
{
    Task<List<Evangelist>> GetAllEvangelistsAsync();
    Task<Evangelist?> GetEvangelistByIdAsync(int id);
    Task<Evangelist> CreateEvangelistAsync(Evangelist evangelist);
    Task<Evangelist> UpdateEvangelistAsync(Evangelist evangelist);
    Task DeleteEvangelistAsync(int id);
    Task<bool> ToggleActiveStatusAsync(int id);
    Task UpdateSortOrderAsync(List<(int Id, int SortOrder)> sortOrders);
}
