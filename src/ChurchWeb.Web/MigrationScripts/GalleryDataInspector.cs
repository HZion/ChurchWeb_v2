using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace ChurchWeb.Web.MigrationScripts
{
    /// <summary>
    /// Inspects ALL gallery data in churchweb schema
    /// </summary>
    public class GalleryDataInspector
    {
        private readonly string _connectionString;

        public GalleryDataInspector(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException("ConnectionStrings__Default not found");
        }

        public async Task InspectDataAsync()
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            Console.WriteLine("=== Complete Gallery Data Report ===\n");

            // Get all albums
            Console.WriteLine("--- ALL Albums ---\n");
            var albumSql = @"SELECT * FROM churchweb.""Albums"" ORDER BY ""Id""";
            await using var albumCmd = new NpgsqlCommand(albumSql, conn);
            await using var albumReader = await albumCmd.ExecuteReaderAsync();

            var albumCount = 0;
            while (await albumReader.ReadAsync())
            {
                albumCount++;
                Console.WriteLine($"Album #{albumCount}:");
                Console.WriteLine($"  Id: {albumReader["Id"]}");
                Console.WriteLine($"  Title: {albumReader["Title"]}");
                Console.WriteLine($"  EventDate: {albumReader["EventDate"]}");
                Console.WriteLine($"  Category: {albumReader["Category"]}");
                Console.WriteLine($"  Description: {albumReader["Description"]}");
                Console.WriteLine($"  CoverImageUrl: {albumReader["CoverImageUrl"]}");
                Console.WriteLine($"  Year: {albumReader["Year"]}");
                Console.WriteLine($"  IsVisible: {albumReader["IsVisible"]}");
                Console.WriteLine($"  SortOrder: {albumReader["SortOrder"]}");
                Console.WriteLine();
            }
            await albumReader.CloseAsync();

            Console.WriteLine($"Total Albums: {albumCount}\n");

            // Get all photos
            Console.WriteLine("--- ALL Album Photos ---\n");
            var photoSql = @"SELECT * FROM churchweb.""AlbumPhotos"" ORDER BY ""AlbumId"", ""SortOrder""";
            await using var photoCmd = new NpgsqlCommand(photoSql, conn);
            await using var photoReader = await photoCmd.ExecuteReaderAsync();

            var photoCount = 0;
            var currentAlbumId = -1;
            while (await photoReader.ReadAsync())
            {
                photoCount++;
                var albumId = (int)photoReader["AlbumId"];

                if (albumId != currentAlbumId)
                {
                    currentAlbumId = albumId;
                    Console.WriteLine($"\n--- Photos for Album {albumId} ---");
                }

                Console.WriteLine($"  Photo Id: {photoReader["Id"]}, ImageUrl: {photoReader["ImageUrl"]}, Caption: {photoReader["Caption"]}, SortOrder: {photoReader["SortOrder"]}");
            }

            Console.WriteLine($"\n\nTotal Photos: {photoCount}");
            Console.WriteLine("\n=== End of Report ===");
        }
    }
}
