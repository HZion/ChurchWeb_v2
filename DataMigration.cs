using Npgsql;
using System;
using System.Threading.Tasks;

class DataMigration
{
    static async Task Main(string[] args)
    {
        var sourceConnString = "Host=dpg-d0iti6adbo4c738s5b8g-a.singapore-postgres.render.com;Database=chuch_sc;Username=chuch_sc_user;Password=bBxfCj2vkQSA4mdCZ5oZOsXY94UM5kbc;SSL Mode=Require;Trust Server Certificate=true";

        Console.WriteLine("Connecting to source database...");

        await using var sourceConn = new NpgsqlConnection(sourceConnString);
        await sourceConn.OpenAsync();

        Console.WriteLine("Connected successfully!");
        Console.WriteLine("\nListing all tables in the database:");
        Console.WriteLine("=" + new string('=', 60));

        // Get all tables
        var tablesQuery = @"
            SELECT table_schema, table_name
            FROM information_schema.tables
            WHERE table_type = 'BASE TABLE'
            AND table_schema NOT IN ('pg_catalog', 'information_schema')
            ORDER BY table_schema, table_name";

        await using var cmd = new NpgsqlCommand(tablesQuery, sourceConn);
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var schema = reader.GetString(0);
            var table = reader.GetString(1);
            Console.WriteLine($"{schema}.{table}");
        }

        await reader.CloseAsync();

        // Get row counts for each table
        Console.WriteLine("\n\nRow counts:");
        Console.WriteLine("=" + new string('=', 60));

        var countCmd = new NpgsqlCommand(tablesQuery, sourceConn);
        await using var tableReader = await countCmd.ExecuteReaderAsync();

        var tables = new System.Collections.Generic.List<(string schema, string table)>();
        while (await tableReader.ReadAsync())
        {
            tables.Add((tableReader.GetString(0), tableReader.GetString(1)));
        }
        await tableReader.CloseAsync();

        foreach (var (schema, table) in tables)
        {
            var countQuery = $"SELECT COUNT(*) FROM \"{schema}\".\"{table}\"";
            await using var countReader = new NpgsqlCommand(countQuery, sourceConn);
            var count = await countReader.ExecuteScalarAsync();
            Console.WriteLine($"{schema}.{table}: {count} rows");
        }
    }
}
