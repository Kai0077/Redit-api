using Npgsql;

namespace Redit_api.Data
{
    public static class SeedExecutor
    {
        public static async Task RunSeedAsync(string connectionString, string scriptPath)
        {
            var sql = await File.ReadAllTextAsync(scriptPath);
            await using var connection = await DbConnectionFactory.CreateOpenConnectionAsync(connectionString);
            await using var cmd = new NpgsqlCommand(sql, connection);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}