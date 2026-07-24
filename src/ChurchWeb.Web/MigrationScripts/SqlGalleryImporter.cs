using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace ChurchWeb.Web.MigrationScripts
{
    /// <summary>
    /// Imports gallery data from SQL file to churchweb schema
    /// SAFETY: READ SQL file, WRITE only to churchweb schema
    /// </summary>
    public class SqlGalleryImporter
    {
        private readonly string _connectionString;
        private readonly string _sqlFilePath = @"C:\Users\28400\AppData\Roaming\Claude\local-agent-mode-sessions\1909083d-64f4-4a5d-aa2a-c739920b190b\fa77552d-5b9a-42d2-a99d-53bcb3a695d1\local_ae97c210-7695-43cc-bd5c-7f04abb62575\outputs\chuch_sc_jv0j.sql";

        public SqlGalleryImporter(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException("ConnectionStrings__Default not found");
        }

        public async Task ImportAsync()
        {
            Console.WriteLine("=== Gallery Data Import from SQL ===\n");

            if (!File.Exists(_sqlFilePath))
            {
                Console.WriteLine($"ERROR: SQL file not found: {_sqlFilePath}");
                return;
            }

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            Console.WriteLine("Step 1: Clearing existing sample data in churchweb schema...\n");
            await ClearExistingDataAsync(conn);

            Console.WriteLine("Step 2: Parsing SQL file and extracting gallery data...\n");
            var (albums, photos) = ParseSqlFile();

            Console.WriteLine($"Found {albums.Count} albums and {photos.Count} photos\n");

            Console.WriteLine("Step 3: Importing albums to churchweb.Albums...\n");
            await ImportAlbumsAsync(conn, albums);

            Console.WriteLine("Step 4: Importing photos to churchweb.AlbumPhotos...\n");
            await ImportPhotosAsync(conn, photos);

            Console.WriteLine("\n=== Import Complete ===");
            Console.WriteLine($"Total Albums: {albums.Count}");
            Console.WriteLine($"Total Photos: {photos.Count}");
        }

        private async Task ClearExistingDataAsync(NpgsqlConnection conn)
        {
            // DELETE existing sample data (only from churchweb schema - SAFE)
            await using var cmd1 = new NpgsqlCommand(@"DELETE FROM churchweb.""AlbumPhotos""", conn);
            var deletedPhotos = await cmd1.ExecuteNonQueryAsync();
            Console.WriteLine($"  Deleted {deletedPhotos} existing photos");

            await using var cmd2 = new NpgsqlCommand(@"DELETE FROM churchweb.""Albums""", conn);
            var deletedAlbums = await cmd2.ExecuteNonQueryAsync();
            Console.WriteLine($"  Deleted {deletedAlbums} existing albums");
        }

        private (List<AlbumData> albums, List<PhotoData> photos) ParseSqlFile()
        {
            var albums = new List<AlbumData>();
            var photos = new List<PhotoData>();

            var lines = File.ReadAllLines(_sqlFilePath);
            bool inAlbumsSection = false;
            bool inPhotosSection = false;

            foreach (var line in lines)
            {
                var trimmed = line.Trim();

                // Detect sections
                if (trimmed.StartsWith("COPY \"public\".\"photo_albums\""))
                {
                    inAlbumsSection = true;
                    inPhotosSection = false;
                    continue;
                }
                else if (trimmed.StartsWith("COPY \"public\".\"photos\""))
                {
                    inPhotosSection = true;
                    inAlbumsSection = false;
                    continue;
                }
                else if (trimmed == "\\.")
                {
                    // End of COPY section
                    inAlbumsSection = false;
                    inPhotosSection = false;
                    continue;
                }

                // Parse data
                if (inAlbumsSection && !string.IsNullOrWhiteSpace(trimmed))
                {
                    var album = ParseAlbumLine(trimmed);
                    if (album != null) albums.Add(album);
                }
                else if (inPhotosSection && !string.IsNullOrWhiteSpace(trimmed))
                {
                    var photo = ParsePhotoLine(trimmed);
                    if (photo != null) photos.Add(photo);
                }
            }

            return (albums, photos);
        }

        private AlbumData? ParseAlbumLine(string line)
        {
            try
            {
                var parts = line.Split('\t');
                if (parts.Length < 9) return null;

                return new AlbumData
                {
                    SourceId = int.Parse(parts[0]),
                    Title = parts[1],
                    Description = parts[2] == "\\N" ? "" : parts[2],
                    EventDate = DateTime.Parse(parts[3]),
                    Category = parts[4],
                    CoverImageUrl = parts[5] == "\\N" ? "" : parts[5]
                };
            }
            catch
            {
                return null;
            }
        }

        private PhotoData? ParsePhotoLine(string line)
        {
            try
            {
                var parts = line.Split('\t');
                if (parts.Length < 8) return null;

                return new PhotoData
                {
                    SourceId = int.Parse(parts[0]),
                    AlbumSourceId = int.Parse(parts[1]),
                    ImageUrl = parts[2],
                    Caption = parts[4] == "\\N" ? "" : parts[4],
                    SortOrder = parts[5] == "\\N" ? 0 : int.Parse(parts[5])
                };
            }
            catch
            {
                return null;
            }
        }

        private async Task ImportAlbumsAsync(NpgsqlConnection conn, List<AlbumData> albums)
        {
            var imported = 0;
            foreach (var album in albums)
            {
                var year = album.EventDate.Year;

                var sql = @"
                    INSERT INTO churchweb.""Albums""
                    (""Title"", ""EventDate"", ""Category"", ""Description"", ""CoverImageUrl"", ""Year"", ""IsVisible"", ""SortOrder"", ""CreatedAt"", ""UpdatedAt"")
                    VALUES
                    (@Title, @EventDate, @Category, @Description, @CoverImageUrl, @Year, true, @SourceId, @CreatedAt, @UpdatedAt)
                    RETURNING ""Id""";

                await using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("Title", album.Title);
                cmd.Parameters.AddWithValue("EventDate", album.EventDate);
                cmd.Parameters.AddWithValue("Category", album.Category);
                cmd.Parameters.AddWithValue("Description", album.Description);
                cmd.Parameters.AddWithValue("CoverImageUrl", album.CoverImageUrl);
                cmd.Parameters.AddWithValue("Year", year);
                cmd.Parameters.AddWithValue("SourceId", album.SourceId);
                cmd.Parameters.AddWithValue("CreatedAt", DateTime.UtcNow);
                cmd.Parameters.AddWithValue("UpdatedAt", DateTime.UtcNow);

                var newId = (int)(await cmd.ExecuteScalarAsync() ?? 0);
                album.NewId = newId;
                imported++;

                if (imported % 10 == 0)
                {
                    Console.WriteLine($"  Imported {imported} albums...");
                }
            }

            Console.WriteLine($"  Total imported: {imported} albums");
        }

        private async Task ImportPhotosAsync(NpgsqlConnection conn, List<PhotoData> photos)
        {
            // Group photos by album
            var photosByAlbum = new Dictionary<int, List<PhotoData>>();
            foreach (var photo in photos)
            {
                if (!photosByAlbum.ContainsKey(photo.AlbumSourceId))
                {
                    photosByAlbum[photo.AlbumSourceId] = new List<PhotoData>();
                }
                photosByAlbum[photo.AlbumSourceId].Add(photo);
            }

            var imported = 0;
            foreach (var (albumSourceId, albumPhotos) in photosByAlbum)
            {
                // Find the new album ID
                var getAlbumIdSql = @"SELECT ""Id"" FROM churchweb.""Albums"" WHERE ""SortOrder"" = @SourceId LIMIT 1";
                await using var getCmd = new NpgsqlCommand(getAlbumIdSql, conn);
                getCmd.Parameters.AddWithValue("SourceId", albumSourceId);
                var albumId = (int?)(await getCmd.ExecuteScalarAsync());

                if (!albumId.HasValue)
                {
                    Console.WriteLine($"  WARNING: Album with source ID {albumSourceId} not found, skipping photos");
                    continue;
                }

                foreach (var photo in albumPhotos)
                {
                    var sql = @"
                        INSERT INTO churchweb.""AlbumPhotos""
                        (""AlbumId"", ""ImageUrl"", ""Caption"", ""SortOrder"")
                        VALUES
                        (@AlbumId, @ImageUrl, @Caption, @SortOrder)";

                    await using var cmd = new NpgsqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("AlbumId", albumId.Value);
                    cmd.Parameters.AddWithValue("ImageUrl", photo.ImageUrl);
                    cmd.Parameters.AddWithValue("Caption", photo.Caption);
                    cmd.Parameters.AddWithValue("SortOrder", photo.SortOrder);

                    await cmd.ExecuteNonQueryAsync();
                    imported++;
                }

                if (imported % 50 == 0)
                {
                    Console.WriteLine($"  Imported {imported} photos...");
                }
            }

            Console.WriteLine($"  Total imported: {imported} photos");
        }

        private class AlbumData
        {
            public int SourceId { get; set; }
            public int NewId { get; set; }
            public string Title { get; set; } = "";
            public string Description { get; set; } = "";
            public DateTime EventDate { get; set; }
            public string Category { get; set; } = "";
            public string CoverImageUrl { get; set; } = "";
        }

        private class PhotoData
        {
            public int SourceId { get; set; }
            public int AlbumSourceId { get; set; }
            public string ImageUrl { get; set; } = "";
            public string Caption { get; set; } = "";
            public int SortOrder { get; set; }
        }
    }
}
