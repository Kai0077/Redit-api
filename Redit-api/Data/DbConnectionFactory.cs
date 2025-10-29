using Npgsql;

namespace Redit_api.Data
{
    public static class DbConnectionFactory
    {
        public static async Task<NpgsqlConnection> CreateOpenConnectionAsync(string connectionString)
        {
            var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}