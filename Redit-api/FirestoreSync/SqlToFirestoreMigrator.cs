using Google.Cloud.Firestore;
using Npgsql;
using Redit_api.Data;

namespace Redit_api.FirestoreSync
{
    public class SqlToFirestoreMigrator
    {
        private readonly FirestoreDb _firestore;
        private readonly string _connectionString;

        public SqlToFirestoreMigrator(FirestoreDb firestore, string connectionString)
        {
            _firestore = firestore;
            _connectionString = connectionString;
        }

        public async Task RunMigrationAsync()
        {
            var tables = new[]
            {
                "user",
                "community",
                "post",
                "comments",
                "user_followers",
                "post_audit_log"
            };

            foreach (var table in tables)
            {
                await MigrateTableAsync(table);
            }
        }

        private async Task MigrateTableAsync(string table)
        {
            Console.WriteLine($"Clearing old Firestore data for '{table}'...");
            var collection = _firestore.Collection(table);

            var oldDocs = await collection.ListDocumentsAsync().ToListAsync();
            var deleteTasks = oldDocs.Select(doc => doc.DeleteAsync());
            await Task.WhenAll(deleteTasks);

            Console.WriteLine($"Deleted {oldDocs.Count} old documents from '{table}'.");

            await using var connection = await DbConnectionFactory.CreateOpenConnectionAsync(_connectionString);
            var cmd = new NpgsqlCommand($"SELECT * FROM \"{table}\";", connection);
            var reader = await cmd.ExecuteReaderAsync();

            var count = 0;

            while (await reader.ReadAsync())
            {
                var values = Enumerable.Range(0, reader.FieldCount)
                    .ToDictionary(reader.GetName, reader.GetValue);

                string documentId;
                if (values.ContainsKey("id"))
                {
                    documentId = values["id"]?.ToString() ?? Guid.NewGuid().ToString();
                }
                else if (values.ContainsKey("username"))
                {
                    documentId = values["username"]?.ToString() ?? Guid.NewGuid().ToString();
                }
                else
                {
                    documentId = Guid.NewGuid().ToString();
                }

                var cleanData = values.ToDictionary(
                    keyValuePair => keyValuePair.Key,
                    keyValuePair =>
                    {
                        if (keyValuePair.Value is DBNull) return null;
                        if (keyValuePair.Value is DateTime dateTime)
                            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                        return keyValuePair.Value;
                    });

                await collection.Document(documentId).SetAsync(cleanData);
                count++;
            }

            Console.WriteLine($"-> Migrated {count} rows from {table}");
        }

    }
}