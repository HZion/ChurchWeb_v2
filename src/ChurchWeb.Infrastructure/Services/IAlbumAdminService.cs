using ChurchWeb.Core.Entities.News;

namespace ChurchWeb.Infrastructure.Services;

public interface IAlbumAdminService
{
    Task<(IEnumerable<Album> albums, int totalCount)> GetPagedAlbumsAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        int? year = null,
        bool? isVisible = null,
        string sortBy = "EventDate",
        bool sortDescending = true);

    Task<Album?> GetAlbumByIdAsync(int id);
    Task<Album?> GetAlbumByIdWithPhotosAsync(int id);
    Task CreateAlbumAsync(Album album);
    Task UpdateAlbumAsync(Album album);
    Task<bool> DeleteAlbumAsync(int id);
    Task<bool> ToggleVisibilityAsync(int id);

    // Photo management
    Task<AlbumPhoto?> GetPhotoByIdAsync(int photoId);
    Task AddPhotoAsync(AlbumPhoto photo);
    Task UpdatePhotoAsync(AlbumPhoto photo);
    Task<bool> DeletePhotoAsync(int photoId);
    Task ReorderPhotosAsync(int albumId, List<int> photoIds);

    // Utility
    Task<List<int>> GetAvailableYearsAsync();
    Task<string> SavePhotoFileAsync(Stream fileStream, string fileName);
    Task<bool> DeletePhotoFileAsync(string imageUrl);
}
