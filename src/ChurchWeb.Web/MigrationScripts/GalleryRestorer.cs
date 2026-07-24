using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ChurchWeb.Web.MigrationScripts
{
    /// <summary>
    /// Restores gallery data from PostgreSQL backup to churchweb schema
    /// SAFETY: READ-ONLY on public, WRITE-ONLY on churchweb
    /// </summary>
    public class GalleryRestorer
    {
        private readonly IConfiguration _configuration;
        private readonly string _backupPath = @"C:\Users\28400\Desktop\ChurchWeb\chuch_sc_jv0j";

        public GalleryRestorer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task RestoreAsync()
        {
            Console.WriteLine("=== Gallery Data Restoration ===\n");

            // Check if backup file exists
            if (!Directory.Exists(_backupPath))
            {
                Console.WriteLine($"ERROR: Backup file not found at {_backupPath}");
                return;
            }

            var connectionString = _configuration.GetConnectionString("Default");
            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("ERROR: Connection string not found");
                return;
            }

            // Parse connection string to get host, database, user
            var connParams = ParseConnectionString(connectionString);

            Console.WriteLine("Backup path: " + _backupPath);
            Console.WriteLine($"Target: {connParams.Database} (churchweb schema)");
            Console.WriteLine();

            Console.WriteLine("IMPORTANT:");
            Console.WriteLine("Since pg_restore is not available, please use one of these methods:");
            Console.WriteLine();
            Console.WriteLine("Option 1: Use Render Web Dashboard");
            Console.WriteLine("  1. Go to your Render PostgreSQL database dashboard");
            Console.WriteLine("  2. Use the 'Restore from backup' feature");
            Console.WriteLine($"  3. Upload the backup directory: {_backupPath}");
            Console.WriteLine();
            Console.WriteLine("Option 2: Install PostgreSQL client tools");
            Console.WriteLine("  1. Install PostgreSQL from https://www.postgresql.org/download/windows/");
            Console.WriteLine("  2. Add pg_restore to PATH");
            Console.WriteLine("  3. Run:");
            Console.WriteLine($"     pg_restore -h {connParams.Host} -U {connParams.User} -d {connParams.Database} \\");
            Console.WriteLine($"       --data-only -t Albums -t AlbumPhotos \"{_backupPath}\"");
            Console.WriteLine();
            Console.WriteLine("Option 3: Convert backup to SQL manually");
            Console.WriteLine("  1. If you have access to the source database");
            Console.WriteLine("  2. Export Albums and AlbumPhotos as plain SQL");
            Console.WriteLine("  3. Then run the import-gallery-sql command");
            Console.WriteLine();

            await Task.CompletedTask;
        }

        private (string Host, string Database, string User) ParseConnectionString(string connString)
        {
            var parts = connString.Split(';');
            string host = "", database = "", user = "";

            foreach (var part in parts)
            {
                var kv = part.Split('=');
                if (kv.Length == 2)
                {
                    var key = kv[0].Trim().ToLower();
                    var value = kv[1].Trim();

                    if (key == "host" || key == "server") host = value;
                    else if (key == "database") database = value;
                    else if (key == "user id" || key == "username") user = value;
                }
            }

            return (host, database, user);
        }
    }
}
