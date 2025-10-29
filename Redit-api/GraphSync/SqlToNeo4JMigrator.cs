using Neo4j.Driver;
using Npgsql;
using Redit_api.Data;

namespace Redit_api.GraphSync
{
    public class SqlToNeo4JMigrator
    {
        private readonly IDriver _neo4J;
        private readonly string _connectionString;

        public SqlToNeo4JMigrator(IDriver neo4J, string connectionString)
        {
            _neo4J = neo4J;
            _connectionString = connectionString;
        }

        public async Task RunMigrationAsync()
        {
            Console.WriteLine("Starting PostgresSQL -> Neo4j migration...");

            await using var connection = await DbConnectionFactory.CreateOpenConnectionAsync(_connectionString);
            await using var session = _neo4J.AsyncSession();

            try
            {
                Console.WriteLine("Clearing old Neo4j data...");
                await session.RunAsync("MATCH (n) DETACH DELETE n");

                // === USERS ===
                Console.WriteLine("Migrating users...");
                var userCmd = new NpgsqlCommand(
                    "SELECT username, name, email, age, password_hash, aura, bio, profile_picture, account_status, role FROM \"user\";",
                    connection);
                var userReader = await userCmd.ExecuteReaderAsync();

                while (await userReader.ReadAsync())
                {
                    var username = userReader.GetString(0);
                    var name = userReader.GetString(1);
                    var email = userReader.GetString(2);
                    var age = userReader.GetInt32(3);
                    var passwordHash = userReader.GetString(4);
                    var aura = userReader.GetInt32(5);
                    var bio = userReader.IsDBNull(6) ? "" : userReader.GetString(6);
                    var profilePicture = userReader.IsDBNull(7) ? "" : userReader.GetString(7);
                    var accountStatus = userReader.GetString(8);
                    var role = userReader.GetString(9);

                    await session.RunAsync(
                        "MERGE (u:User {username: $username}) " +
                        "SET u.name = $name, u.email = $email, u.age = $age, " +
                        "u.password_hash = $passwordHash, u.aura = $aura, u.bio = $bio, " +
                        "u.profile_picture = $profilePicture, u.account_status = $accountStatus, u.role = $role",
                        new
                        {
                            username, name, email, age, passwordHash, aura, bio, profilePicture, accountStatus, role
                        });
                }

                userReader.Close();
                userReader.Dispose();

                // ===== COMMUNITIES =====
                Console.WriteLine("Migrating communities...");
                var communityCmd = new NpgsqlCommand(
                    "SELECT name, description, profile_picture, owner_username FROM community;",
                    connection);
                var communityReader = await communityCmd.ExecuteReaderAsync();

                while (await communityReader.ReadAsync())
                {
                    var name = communityReader.GetString(0);
                    var description = communityReader.IsDBNull(1) ? "" : communityReader.GetString(1);
                    var profilePicture = communityReader.IsDBNull(2) ? "" : communityReader.GetString(2);
                    var owner = communityReader.IsDBNull(3) ? null : communityReader.GetString(3);

                    await session.RunAsync(
                        "MERGE (c:Community {name: $name}) " +
                        "SET c.description = $description, c.profile_picture = $profilePicture",
                        new { name, description, profilePicture });

                    if (!string.IsNullOrEmpty(owner))
                    {
                        await session.RunAsync(
                            "MATCH (u:User {username: $owner}), (c:Community {name: $name}) " +
                            "MERGE (u)-[:OWNS]->(c)",
                            new { owner, name });
                    }
                }

                communityReader.Close();
                communityCmd.Dispose();

                // ===== POSTS =====
                Console.WriteLine("Migrating posts...");
                var postCmd = new NpgsqlCommand(
                    "SELECT id, title, description, aura, original_poster, community, status FROM post;",
                    connection);
                var postReader = await postCmd.ExecuteReaderAsync();

                while (await postReader.ReadAsync())
                {
                    var id = postReader.GetInt32(0);
                    var title = postReader.GetString(1);
                    var description = postReader.IsDBNull(2) ? "" : postReader.GetString(2);
                    var aura = postReader.GetInt32(3);
                    var poster = postReader.GetString(4);
                    var community = postReader.GetString(5);
                    var status = postReader.GetString(6);

                    await session.RunAsync(
                        "MERGE (p:Post {id: $id}) " +
                        "SET p.title = $title, p.description = $description, p.aura = $aura, p.status = $status " +
                        "WITH p " +
                        "MATCH (u:User {username: $poster}), (c:Community {name: $community}) " +
                        "MERGE (u)-[:POSTED]->(p) " +
                        "MERGE (p)-[:IN_COMMUNITY]->(c)",
                        new { id, title, description, aura, poster, community, status });
                }

                postReader.Close();
                postCmd.Dispose();

                // ===== COMMENTS =====
                Console.WriteLine("Migrating comments...");
                var commentCmd = new NpgsqlCommand(
                    "SELECT id, text, aura, commenter, post_id, parent_id FROM comments;",
                    connection);
                var commentReader = await commentCmd.ExecuteReaderAsync();

                while (await commentReader.ReadAsync())
                {
                    var id = commentReader.GetInt32(0);
                    var text = commentReader.GetString(1);
                    var aura = commentReader.GetInt32(2);
                    var commenter = commentReader.GetString(3);
                    var postId = commentReader.GetInt32(4);
                    int? parentId = commentReader.IsDBNull(5) ? null : commentReader.GetInt32(5);

                    await session.RunAsync(
                        "MERGE (c:Comment {id: $id}) " +
                        "SET c.text = $text, c.aura = $aura " +
                        "WITH c " +
                        "MATCH (u:User {username: $commenter}), (p:Post {id: $postId}) " +
                        "MERGE (u)-[:COMMENTED]->(c) " +
                        "MERGE (c)-[:ON_POST]->(p)",
                        new { id, text, aura, commenter, postId });

                    if (parentId.HasValue)
                    {
                        await session.RunAsync(
                            "MATCH (child:Comment {id: $id}), (parent:Comment {id: $parentId}) " +
                            "MERGE (child)-[:REPLY_TO]->(parent)",
                            new { id, parentId });
                    }
                }

                commentReader.Close();
                communityCmd.Dispose();

                // ===== COMMUNITY MEMBERS =====
                Console.WriteLine("Migrating community members...");
                var membersCmd =
                    new NpgsqlCommand("SELECT community_name, username FROM community_members;", connection);
                var membersReader = await membersCmd.ExecuteReaderAsync();

                while (await membersReader.ReadAsync())
                {
                    var community = membersReader.GetString(0);
                    var username = membersReader.GetString(1);

                    await session.RunAsync("MATCH (u:User {username: $username}), (c:Community {name: $community}) " +
                                           "MERGE (u)-[:MEMBER_OF]->(c)",
                        new { community, username });
                }

                membersReader.Close();
                membersCmd.Dispose();

                // ===== USER FOLLOWS =====
                Console.WriteLine("Migrating user follows...");
                var followsCmd = new NpgsqlCommand("SELECT follower_username, following_username FROM user_follows;",
                    connection);
                var followsReader = await followsCmd.ExecuteReaderAsync();

                while (await followsReader.ReadAsync())
                {
                    var follower = followsReader.GetString(0);
                    var following = followsReader.GetString(1);

                    await session.RunAsync(
                        "MATCH (a:User {username: $follower}), (b:User {username: $following}) " +
                        "MERGE (a)-[:FOLLOWS]->(b)",
                        new { follower, following });
                }

                followsReader.Close();
                followsCmd.Dispose();

                Console.WriteLine("Neo4j migration complete!");
            }
            finally
            {
                await session.CloseAsync();
                await connection.CloseAsync();
            }
        }
    }
}