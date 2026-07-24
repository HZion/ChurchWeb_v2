using ChurchWeb.Core.Entities.News;
using ChurchWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChurchWeb.Infrastructure.Services;

public class AlbumAdminService : IAlbumAdminService
{
    private readonly AppDbContext _context;
    private readonly ILogger<AlbumAdminService> _logger;
    private readonly string _webRootPath;
    private const string PhotosFolder = "uploads/gallery";

    public AlbumAdminService(
        AppDbContext context,
        ILogger<AlbumAdminService> logger,
        string webRootPath)
    {
        _context = context;
        _logger = logger;
        _webRootPath = webRootPath;
    }

    public async Task<(IEnumerable<Album> albums, int totalCount)> GetPagedAlbumsAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        int? year = null,
        bool? isVisible = null,
        string sortBy = "EventDate",
        bool sortDescending = true)
    {
        var query = _context.Albums.AsQueryable();

        // Filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(a => a.Title.Contains(searchTerm) || a.Description.Contains(searchTerm));
        }

        if (year.HasValue)
        {
            query = query.Where(a => a.Year == year.Value);
        }

        if (isVisible.HasValue)
        {
            query = query.Where(a => a.IsVisible == isVisible.Value);
        }

        // Total count
        var totalCount = await query.CountAsync();

        // Sort
        query = sortBy switch
        {
            "Title" => sortDescending ? query.OrderByDescending(a => a.Title) : query.OrderBy(a => a.Title),
            "EventDate" => sortDescending ? query.OrderByDescending(a => a.EventDate) : query.OrderBy(a => a.EventDate),
            _ => sortDescending ? query.OrderByDescending(a => a.EventDate) : query.OrderBy(a => a.EventDate)
        };

        // Paging with photo count
        var albums = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(a => a.Photos)
            .ToListAsync();

        return (albums, totalCount);
    }

    public async Task<Album?> GetAlbumByIdAsync(int id)
    {
        return await _context.Albums.FindAsync(id);
    }

    public async Task<Album?> GetAlbumByIdWithPhotosAsync(int id)
    {
        return await _context.Albums
            .Include(a => a.Photos.OrderBy(p => p.SortOrder))
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task CreateAlbumAsync(Album album)
    {
        // Set year from event date
        album.Year = album.EventDate.Year;

        // Auto-increment sort order if not set
        if (album.SortOrder == 0)
        {
            var maxSortOrder = await _context.Albums.MaxAsync(a => (int?)a.SortOrder) ?? 0;
            album.SortOrder = maxSortOrder + 1;
        }

        album.CreatedAt = DateTime.UtcNow;
        album.UpdatedAt = DateTime.UtcNow;

        _context.Albums.Add(album);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAlbumAsync(Album album)
    {
        album.Year = album.EventDate.Year;
        album.UpdatedAt = DateTime.UtcNow;

        _context.Albums.Update(album);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAlbumAsync(int id)
    {
        var album = await GetAlbumByIdWithPhotosAsync(id);
        if (album == null)
            return false;

        // Delete all photo files
        foreach (var photo in album.Photos)
        {
            await DeletePhotoFileAsync(photo.ImageUrl);
        }

        // Delete cover image if exists
        if (!string.IsNullOrEmpty(album.CoverImageUrl))
        {
            await DeletePhotoFileAsync(album.CoverImageUrl);
        }

        _context.Albums.Remove(album);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ToggleVisibilityAsync(int id)
    {
        var album = await GetAlbumByIdAsync(id);
        if (album == null)
            return false;

        album.IsVisible = !album.IsVisible;
        album.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    // Photo management

    public async Task<AlbumPhoto?> GetPhotoByIdAsync(int photoId)
    {
        return await _context.AlbumPhotos.FindAsync(photoId);
    }

    public async Task AddPhotoAsync(AlbumPhoto photo)
    {
        // Auto-increment sort order if not set
        if (photo.SortOrder == 0)
        {
            var maxSortOrder = await _context.AlbumPhotos
                .Where(p => p.AlbumId == photo.AlbumId)
                .MaxAsync(p => (int?)p.SortOrder) ?? 0;
            photo.SortOrder = maxSortOrder + 1;
        }

        _context.AlbumPhotos.Add(photo);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePhotoAsync(AlbumPhoto photo)
    {
        _context.AlbumPhotos.Update(photo);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeletePhotoAsync(int photoId)
    {
        var photo = await GetPhotoByIdAsync(photoId);
        if (photo == null)
            return false;

        await DeletePhotoFileAsync(photo.ImageUrl);

        _context.AlbumPhotos.Remove(photo);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task ReorderPhotosAsync(int albumId, List<int> photoIds)
    {
        var photos = await _context.AlbumPhotos
            .Where(p => p.AlbumId == albumId && photoIds.Contains(p.Id))
            .ToListAsync();

        for (int i = 0; i < photoIds.Count; i++)
        {
            var photo = photos.FirstOrDefault(p => p.Id == photoIds[i]);
            if (photo != null)
            {
                photo.SortOrder = i + 1;
            }
        }

        await _context.SaveChangesAsync();
    }

    // Utility

    public async Task<List<int>> GetAvailableYearsAsync()
    {
        return await _context.Albums
            .Select(a => a.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToListAsync();
    }

    public async Task<string> SavePhotoFileAsync(Stream fileStream, string fileName)
    {
        try
        {
            // Create uploads directory if it doesn't exist
            var uploadsPath = Path.Combine(_webRootPath, PhotosFolder);
            Directory.CreateDirectory(uploadsPath);

            // Generate unique file name
            var extension = Path.GetExtension(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            // Save file
            using (var fileStreamOut = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(fileStreamOut);
            }

            // Return relative URL
            return $"/{PhotosFolder}/{uniqueFileName}".Replace("\\", "/");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving photo file: {FileName}", fileName);
            throw;
        }
    }

    public Task<bool> DeletePhotoFileAsync(string imageUrl)
    {
        try
        {
            if (string.IsNullOrEmpty(imageUrl))
                return Task.FromResult(false);

            // Extract file path from URL
            var relativePath = imageUrl.TrimStart('/');
            var filePath = Path.Combine(_webRootPath, relativePath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting photo file: {ImageUrl}", imageUrl);
            return Task.FromResult(false);
        }
    }
}
