// Repositories/PostRepository.cs
using Microsoft.EntityFrameworkCore;
using Redit_api.Data;
using Redit_api.Models;
using Redit_api.Repositories.Postgresql.Interfaces;

namespace Redit_api.Repositories.Postgresql
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

        public Task<bool> CommunityExistsAsync(string communityName, CancellationToken ct) =>
            _db.Communities.AnyAsync(c => c.Name == communityName, ct);

        public Task<string?> GetUsernameByEmailAsync(string email, CancellationToken ct) =>
            _db.Users.Where(u => u.Email == email)
                .Select(u => u.Username)
                .FirstOrDefaultAsync(ct);

        public Task<UserDTO?> GetUserByEmailAsync(string email, CancellationToken ct) =>
            _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

        public async Task<IEnumerable<PostDTO>> GetAllPublicAsync(CancellationToken ct) =>
            await _db.Posts
                .OrderBy(p => Guid.NewGuid())
                .Take(100)
                .ToListAsync(ct);
        
        public async Task<IEnumerable<PostDTO>> GetAllAsync(CancellationToken ct) =>
            await _db.Posts
                .OrderByDescending(p => p.Id)
                .ToListAsync(ct);

        public async Task<IEnumerable<PostDTO>> GetByUserAsync(string username, CancellationToken ct) =>
            await _db.Posts
                .Where(p => p.OriginalPoster == username)
                .OrderByDescending(p => p.Id)
                .ToListAsync(ct);
    }
}