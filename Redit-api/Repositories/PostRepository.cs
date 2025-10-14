// Repositories/PostRepository.cs
using Microsoft.EntityFrameworkCore;
using Redit_api.Data;
using Redit_api.Models;
using Redit_api.Repositories.Interfaces;

namespace Redit_api.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly AppDBContext _db;
        public PostRepository(AppDBContext db) => _db = db;

        public async Task<PostDTO> CreateAsync(PostDTO post, CancellationToken ct)
        {
            _db.Set<PostDTO>().Add(post);
            await _db.SaveChangesAsync(ct);
            return post;
        }

        public Task<PostDTO?> GetByIdAsync(int id, CancellationToken ct) =>
            _db.Set<PostDTO>().FirstOrDefaultAsync(p => p.Id == id, ct);

        public async Task UpdateAsync(PostDTO post, CancellationToken ct)
        {
            _db.Set<PostDTO>().Update(post);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(PostDTO post, CancellationToken ct)
        {
            _db.Set<PostDTO>().Remove(post);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<bool> CommunityExistsAsync(string communityName, CancellationToken ct)
        {
            var count = await _db.Database
                .SqlQueryRaw<int>(@"SELECT COUNT(*)::int FROM community WHERE name = {0}", communityName)
                .SingleAsync(ct);

            return count > 0;
        }

        public Task<string?> GetUsernameByEmailAsync(string email, CancellationToken ct) =>
            _db.Users.Where(u => u.Email == email)
                .Select(u => u.Username)
                .FirstOrDefaultAsync(ct);

        public Task<UserDTO?> GetUserByEmailAsync(string email, CancellationToken ct) =>
            _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
    }
}