namespace ChurchWeb.Application.Services;

public interface IAboutService
{
    Task<object> GetVisionViewModelAsync();
    Task<object> GetWorshipViewModelAsync();
    Task<object> GetPeopleViewModelAsync();
    Task<object> GetLocationViewModelAsync();
}
