using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace ChurchWeb.Web.MigrationScripts
{
    /// <summary>
    /// Inspects the existing public schema to identify gallery-related tables
    /// READ-ONLY operations on public schema
    /// </summary>
    public class GallerySchemaInspector
    {
        private readonly string _connectionString;

        public GallerySchemaInspector(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException("ConnectionStrings__Default not found");
        }

        public async Task InspectSchemaAsync()
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            Console.WriteLine("=== Gallery Schema Inspection Report ===\n");

            // First, list all schemas
            Console.WriteLine("1. Listing all schemas in database...\n");
            var schemas = await FindAllSchemasAsync(conn);
            Console.WriteLine($"Found {schemas.Count} schema(s):");
            foreach (var schema in schemas)
            {
                Console.WriteLine($"  - {schema}");
            }
            Console.WriteLine();

            // Step 2: Find gallery-related tables in public schema
            Console.WriteLine("2. Searching for gallery-related tables in public schema...\n");
            var galleryTables = await FindGalleryTablesAsync(conn);

            if (galleryTables.Count > 0)
            {
                Console.WriteLine($"Found {galleryTables.Count} potential gallery table(s):");
                foreach (var table in galleryTables)
                {
                    Console.WriteLine($"  - {table}");
                }
                Console.WriteLine();

                // Get detailed structure for each table
                foreach (var table in galleryTables)
                {
                    await InspectTableStructureAsync(conn, table);
                }
            }
            else
            {
                Console.WriteLine("No tables with gallery/photo/album/image in name found.");
                Console.WriteLine("\n3. Listing ALL tables in public schema:\n");
                var allTables = await FindAllPublicTablesAsync(conn);
                if (allTables.Count > 0)
                {
                    foreach (var table in allTables)
                    {
                        Console.WriteLine($"  - {table}");
                    }

                    // Inspect all tables to find gallery data
                    Console.WriteLine("\n4. Inspecting all tables for potential gallery data:\n");
                    foreach (var table in allTables)
                    {
                        await InspectTableStructureAsync(conn, table);
                    }
                }
                else
                {
                    Console.WriteLine("  No tables found in public schema.");
                    Console.WriteLine("\n4. Searching in ALL schemas:\n");
                    await SearchAllSchemasAsync(conn);
                }
            }

            // Inspect churchweb schema Albums and AlbumPhotos
            Console.WriteLine("\n5. Inspecting churchweb.Albums and churchweb.AlbumPhotos:\n");
            await InspectTableStructureAsync(conn, "Albums", "churchweb");
            await InspectTableStructureAsync(conn, "AlbumPhotos", "churchweb");

            Console.WriteLine("\n=== End of Inspection Report ===");
        }

        private async Task<List<string>> FindAllSchemasAsync(NpgsqlConnection conn)
        {
            var schemas = new List<string>();
            var sql = @"
                SELECT schema_name
                FROM information_schema.schemata
                WHERE schema_name NOT IN ('information_schema', 'pg_catalog', 'pg_toast')
                ORDER BY schema_name";

            await using var cmd = new NpgsqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                schemas.Add(reader.GetString(0));
            }

            return schemas;
        }

        private async Task SearchAllSchemasAsync(NpgsqlConnection conn)
        {
            var sql = @"
                SELECT table_schema, table_name
                FROM information_schema.tables
                WHERE table_schema NOT IN ('information_schema', 'pg_catalog', 'pg_toast')
                ORDER BY table_schema, table_name";

            await using var cmd = new NpgsqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            var currentSchema = "";
            while (await reader.ReadAsync())
            {
                var schema = reader.GetString(0);
                var table = reader.GetString(1);

                if (schema != currentSchema)
                {
                    Console.WriteLine($"\nSchema: {schema}");
                    currentSchema = schema;
                }
                Console.WriteLine($"  - {table}");
            }
        }

        private async Task<List<string>> FindGalleryTablesAsync(NpgsqlConnection conn)
        {
            var tables = new List<string>();
            var sql = @"
                SELECT table_name
                FROM information_schema.tables
                WHERE table_schema = 'public'
                  AND (
                    table_name ILIKE '%gallery%'
                    OR table_name ILIKE '%photo%'
                    OR table_name ILIKE '%album%'
                    OR table_name ILIKE '%image%'
                  )
                ORDER BY table_name";

            await using var cmd = new NpgsqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                tables.Add(reader.GetString(0));
            }

            return tables;
        }

        private async Task<List<string>> FindAllPublicTablesAsync(NpgsqlConnection conn)
        {
            var tables = new List<string>();
            var sql = @"
                SELECT table_name
                FROM information_schema.tables
                WHERE table_schema = 'public'
                ORDER BY table_name";

            await using var cmd = new NpgsqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                tables.Add(reader.GetString(0));
            }

            return tables;
        }

        private async Task InspectTableStructureAsync(NpgsqlConnection conn, string tableName, string schemaName = "public")
        {
            Console.WriteLine($"--- Table: {schemaName}.{tableName} ---");

            // Get columns
            var columnSql = @"
                SELECT
                    column_name,
                    data_type,
                    character_maximum_length,
                    is_nullable,
                    column_default
                FROM information_schema.columns
                WHERE table_schema = @schemaName
                  AND table_name = @tableName
                ORDER BY ordinal_position";

            await using var cmd = new NpgsqlCommand(columnSql, conn);
            cmd.Parameters.AddWithValue("schemaName", schemaName);
            cmd.Parameters.AddWithValue("tableName", tableName);
            await using var reader = await cmd.ExecuteReaderAsync();

            Console.WriteLine("Columns:");
            while (await reader.ReadAsync())
            {
                var name = reader.GetString(0);
                var type = reader.GetString(1);
                var nullable = reader.GetString(3);
                var maxLength = reader.IsDBNull(2) ? "" : $"({reader.GetInt32(2)})";
                var defaultVal = reader.IsDBNull(4) ? "" : $" DEFAULT {reader.GetString(4)}";

                Console.WriteLine($"  {name}: {type}{maxLength} {nullable}{defaultVal}");
            }

            await reader.CloseAsync();

            // Get row count
            var countSql = $"SELECT COUNT(*) FROM {schemaName}.\"{tableName}\"";
            await using var countCmd = new NpgsqlCommand(countSql, conn);
            var count = (long)(await countCmd.ExecuteScalarAsync() ?? 0L);
            Console.WriteLine($"\nRow count: {count}");

            // Get sample data (first 3 rows)
            if (count > 0)
            {
                Console.WriteLine("\nSample data (first 3 rows):");
                var sampleSql = $"SELECT * FROM {schemaName}.\"{tableName}\" LIMIT 3";
                await using var sampleCmd = new NpgsqlCommand(sampleSql, conn);
                await using var sampleReader = await sampleCmd.ExecuteReaderAsync();

                var fieldCount = sampleReader.FieldCount;
                var rowNum = 1;

                while (await sampleReader.ReadAsync())
                {
                    Console.WriteLine($"\n  Row {rowNum}:");
                    for (int i = 0; i < fieldCount; i++)
                    {
                        var fieldName = sampleReader.GetName(i);
                        var value = sampleReader.IsDBNull(i) ? "NULL" : sampleReader.GetValue(i).ToString();

                        // Truncate long values
                        if (value != null && value.Length > 100)
                        {
                            value = value.Substring(0, 100) + "... (truncated)";
                        }

                        Console.WriteLine($"    {fieldName}: {value}");
                    }
                    rowNum++;
                }
            }

            Console.WriteLine("\n");
        }
    }
}
