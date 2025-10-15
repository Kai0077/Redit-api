using Microsoft.EntityFrameworkCore;
using Redit_api.Data;
using Redit_api.Models;
using Redit_api.Models.DTO;
using Redit_api.Repositories.Interfaces;

namespace Redit_api.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDBContext _db;
        public CommentRepository(AppDBContext db) => _db = db;

        public async Task<CommentDTO> CreateAsync(CommentDTO comment, CancellationToken ct)
        {
            _db.Comments.Add(comment);
            await _db.SaveChangesAsync(ct);
            return comment;
        }

        public Task<CommentDTO?> GetByIdAsync(int id, CancellationToken ct) =>
            _db.Comments.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);

        public async Task UpdateAsync(CommentDTO comment, CancellationToken ct)
        {
            _db.Comments.Update(comment);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(CommentDTO comment, CancellationToken ct)
        {
            _db.Comments.Remove(comment);
            await _db.SaveChangesAsync(ct);
        }

        public Task<bool> PostExistsAsync(int postId, CancellationToken ct) =>
            _db.Posts.AsNoTracking().AnyAsync(p => p.Id == postId, ct);

        public Task<bool> ParentBelongsToPostAsync(int parentId, int postId, CancellationToken ct) =>
            _db.Comments.AsNoTracking().AnyAsync(c => c.Id == parentId && c.PostId == postId, ct);

        public Task<string?> GetUsernameByEmailAsync(string email, CancellationToken ct) =>
            _db.Users.AsNoTracking()
                .Where(u => u.Email == email)
                .Select(u => u.Username)
                .FirstOrDefaultAsync(ct);

        public Task<List<CommentDTO>> GetByPostAsync(int postId, int skip, int take, CancellationToken ct) =>
            _db.Comments.AsNoTracking()
                .Where(c => c.PostId == postId)
                .OrderBy(c => c.Id)
                .Skip(skip)
                .Take(take)
                .ToListAsync(ct);

        public Task<List<CommentDTO>> GetByUserAsync(string username, int skip, int take, CancellationToken ct) =>
            _db.Comments.AsNoTracking()
                .Where(c => c.Commenter == username)
                .OrderByDescending(c => c.Id)
                .Skip(skip)
                .Take(take)
                .ToListAsync(ct);
    }
}