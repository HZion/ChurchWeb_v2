namespace ChurchWeb.Application.Services;

public interface ISermonsService
{
    Task<object> GetSermonListAsync(string category, int page, int pageSize, string? search = null);
    Task<object?> GetSermonDetailAsync(int id);
}
