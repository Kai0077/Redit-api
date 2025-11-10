using Microsoft.EntityFrameworkCore;
using Redit_api.Data;
using Redit_api.Models;
using Redit_api.Repositories.Interfaces;

namespace Redit_api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDBContext _db;
        public UserRepository(AppDBContext db) => _db = db;

        public Task<bool> UsernameExistsAsync(string username, CancellationToken ct) =>
            _db.Users.AsNoTracking().AnyAsync(u => u.Username == username, ct);

        public Task<bool> EmailExistsAsync(string email, CancellationToken ct) =>
            _db.Users.AsNoTracking().AnyAsync(u => u.Email == email, ct);

        public Task<UserDTO?> GetByUsernameAsync(string username, CancellationToken ct) =>
            _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username, ct);

        public Task<UserDTO?> GetByEmailAsync(string email, CancellationToken ct) =>
            _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, ct);

        public async Task<UserDTO> CreateAsync(UserDTO user, CancellationToken ct)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);
            return user;
        }

        public async Task UpdateAsync(UserDTO user, CancellationToken ct)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync(ct);
        }

        // NEW
        public Task<List<UserDTO>> GetAllAsync(CancellationToken ct) =>
            _db.Users.AsNoTracking()
                     .OrderBy(u => u.Username)
                     .ToListAsync(ct);

        public async Task DeleteUserAsync(string username, CancellationToken ct)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync(ct);
            try
            {
                var userExists = await _db.Users.AnyAsync(u => u.Username == username, ct);
                if (!userExists)
                    throw new Exception("User not found");
                
                await _db.Users
                    .Where(u => u.Username == username)
                    .ExecuteDeleteAsync(ct);

                await transaction.CommitAsync(ct);
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }

        public Task<List<UserDTO>> GetFollowersAsync(string username, CancellationToken ct) =>
            // Who follows {username}? f.following_username = username -> pick follower users
            _db.Users.FromSqlRaw(@"
                SELECT u.*
                FROM ""user"" u
                JOIN user_follows f ON u.username = f.follower_username
                WHERE f.following_username = {0}
            ", username)
            .AsNoTracking()
            .ToListAsync(ct);

        public Task<List<UserDTO>> GetFollowingAsync(string username, CancellationToken ct) =>
            // Who does {username} follow? f.follower_username = username -> pick following users
            _db.Users.FromSqlRaw(@"
                SELECT u.*
                FROM ""user"" u
                JOIN user_follows f ON u.username = f.following_username
                WHERE f.follower_username = {0}
            ", username)
            .AsNoTracking()
            .ToListAsync(ct);
        
        public async Task<List<string>> GetFollowerUsernamesAsync(string username, CancellationToken ct)
        {
            // usernames of people who follow {username}
            return await _db.VUserFollowers
                .Where(v => v.TargetUsername == username)
                .Select(v => v.FollowerUsername)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<List<string>> GetFollowingUsernamesAsync(string username, CancellationToken ct)
        {
            // usernames of people that {username} is following
            return await _db.VUserFollowing
                .Where(v => v.SourceUsername == username)
                .Select(v => v.FollowingUsername)
                .AsNoTracking()
                .ToListAsync(ct);
        }
    }
}