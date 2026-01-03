using Neo4j.Driver;
using Redit_api.Models;
using Redit_api.Models.Status;
using Redit_api.Repositories.Neo4j.Interfaces;

namespace Redit_api.Repositories.Neo4j
{
    public class Neo4jUserReadRepository : INeo4jUserReadRepository
    {
        private readonly IDriver _driver;

        public Neo4jUserReadRepository(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<List<UserDTO>> GetAllAsync(CancellationToken ct)
        {
            const string cypher = @"
                MATCH (u:User)
                RETURN
                    u.username        AS username,
                    u.name            AS name,
                    u.email           AS email,
                    u.age             AS age,
                    u.aura            AS aura,
                    u.bio             AS bio,
                    u.profile_picture AS profilePicture,
                    u.account_status  AS accountStatus,
                    u.role            AS role
                ORDER BY username
            ";

            await using var session = _driver.AsyncSession();

            // IMPORTANT: this driver version doesn't support cancellationToken named arg here
            var cursor = await session.RunAsync(cypher);

            var users = new List<UserDTO>();

            // IMPORTANT: manual iteration avoids ToListAsync(ct) overload differences
            while (await cursor.FetchAsync())
            {
                ct.ThrowIfCancellationRequested();

                var r = cursor.Current;

                var statusRaw = r["accountStatus"]?.As<string>() ?? "Active";
                Enum.TryParse<UserStatus>(statusRaw, true, out var status);

                var roleRaw = r["role"]?.As<string>() ?? "User";
                Enum.TryParse<UserRole>(roleRaw, true, out var role);

                users.Add(new UserDTO
                {
                    Username = r["username"].As<string>(),
                    Name = r["name"].As<string>(),
                    Email = r["email"].As<string>(),
                    Age = r["age"].As<int>(),
                    Aura = r["aura"].As<int>(),
                    Bio = r["bio"]?.As<string>(),
                    ProfilePicture = r["profilePicture"]?.As<string>(),
                    AccountStatus = status,
                    Role = role,
                    PasswordHash = ""
                });
            }

            return users;
        }
    }
}