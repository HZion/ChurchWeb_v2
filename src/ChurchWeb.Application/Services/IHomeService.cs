namespace ChurchWeb.Application.Services;

public interface IHomeService
{
    Task<object> GetHomeViewModelAsync();
}
