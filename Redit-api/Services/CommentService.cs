using Redit_api.Models.DTO;
using Redit_api.Models.Status;
using Redit_api.Repositories.Firestore.Interfaces;
using Redit_api.Repositories.Postgresql.Interfaces;
using Redit_api.Services.Interfaces;
using Redit_api.Repositories.Interfaces;

namespace Redit_api.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _repo;
        private readonly IPostgresUserRepository _postgresUsers;

        public CommentService(IPostgresCommentRepository postgresRepo, IFirestoreCommentRepository firestoreRepo, IPostgresUserRepository postgresUsers)
        {
            _postgresUsers = postgresUsers;
            
            var db = Environment.GetEnvironmentVariable("DB_TYPE")?.ToLower();

            _repo = db switch
            {
                "postgres" => postgresRepo,
                "firestore" => firestoreRepo,
                _ => throw new Exception("DB_TYPE can not be empty")
            };
        }

        public async Task<(bool Success, string? Error, object? Data)> CreateAsync(
            string requesterEmail, CommentCreateDTO dto, CancellationToken ct)
        {
            // resolve commenter username
            var username = await _repo.GetUsernameByEmailAsync(requesterEmail, ct);
            if (string.IsNullOrEmpty(username))
                return (false, "User not found.", null);

            // post must exist
            if (!await _repo.PostExistsAsync(dto.PostId, ct))
                return (false, "Post not found.", null);

            // if replying, parent must be in the same post
            if (dto.ParentId.HasValue)
            {
                var ok = await _repo.ParentBelongsToPostAsync(dto.ParentId.Value, dto.PostId, ct);
                if (!ok) return (false, "Invalid parent comment for this post.", null);
            }

            var c = new CommentDTO
            {
                PostId = dto.PostId,
                ParentId = dto.ParentId,
                Text = dto.Text.Trim(),
                Embeds = dto.Embeds ?? Array.Empty<string>(),
                Commenter = username,
                Aura = 0
            };

            var created = await _repo.CreateAsync(c, ct);

            var result = new
            {
                created.Id,
                created.PostId,
                created.ParentId,
                created.Text,
                created.Embeds,
                created.Commenter,
                created.Aura
            };
            return (true, null, result);
        }

        public async Task<(bool Success, string? Error, object? Data)> UpdateAsync(
            string requesterEmail, int id, CommentUpdateDTO dto, CancellationToken ct)
        {
            var existing = await _repo.GetByIdAsync(id, ct);
            if (existing == null) return (false, "Not found.", null);

            var requester = await _postgresUsers.GetByUsernameAsync(existing.Commenter, ct); // load owner userDTO
            var emailOwner = requester?.Email?.ToLowerInvariant();

            // who is making the request?
            var requesterName = await _repo.GetUsernameByEmailAsync(requesterEmail, ct);
            if (string.IsNullOrEmpty(requesterName)) return (false, "User not found.", null);
            var requesterUser = await _postgresUsers.GetByUsernameAsync(requesterName, ct);
            var isOwner = string.Equals(emailOwner, requesterEmail, StringComparison.OrdinalIgnoreCase);
            var isSuper = requesterUser?.Role == UserRole.SuperUser;

            if (!isOwner && !isSuper) return (false, "Forbidden.", null);

            if (!string.IsNullOrWhiteSpace(dto.Text)) existing.Text = dto.Text.Trim();
            if (dto.Embeds != null) existing.Embeds = dto.Embeds;

            await _repo.UpdateAsync(existing, ct);

            var result = new
            {
                existing.Id,
                existing.PostId,
                existing.ParentId,
                existing.Text,
                existing.Embeds,
                existing.Commenter,
                existing.Aura
            };
            return (true, null, result);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(
            string requesterEmail, int id, CancellationToken ct)
        {
            var existing = await _repo.GetByIdAsync(id, ct);
            if (existing == null) return (false, "Not found.");

            var owner = await _postgresUsers.GetByUsernameAsync(existing.Commenter, ct);
            var ownerEmail = owner?.Email?.ToLowerInvariant();

            var requesterName = await _repo.GetUsernameByEmailAsync(requesterEmail, ct);
            if (string.IsNullOrEmpty(requesterName)) return (false, "User not found.");
            var requesterUser = await _postgresUsers.GetByUsernameAsync(requesterName, ct);

            var isOwner = string.Equals(ownerEmail, requesterEmail, StringComparison.OrdinalIgnoreCase);
            var isSuper = requesterUser?.Role == UserRole.SuperUser;

            if (!isOwner && !isSuper) return (false, "Forbidden.");

            await _repo.DeleteAsync(existing, ct);
            return (true, null);
        }

        public async Task<(bool Success, string? Error, IEnumerable<object>? Data)> GetByPostAsync(
            int postId, int skip, int take, CancellationToken ct)
        {
            var list = await _repo.GetByPostAsync(postId, skip, take, ct);
            var shaped = list.Select(c => new
            {
                c.Id,
                c.PostId,
                c.ParentId,
                c.Text,
                c.Embeds,
                c.Commenter,
                c.Aura
            });
            return (true, null, shaped);
        }

        public async Task<(bool Success, string? Error, IEnumerable<object>? Data)> GetByUserAsync(
            string requesterEmail, int skip, int take, CancellationToken ct)
        {
            var username = await _repo.GetUsernameByEmailAsync(requesterEmail, ct);
            if (string.IsNullOrEmpty(username))
                return (false, "User not found.", null);

            var list = await _repo.GetByUserAsync(username, skip, take, ct);
            var shaped = list.Select(c => new
            {
                c.Id,
                c.PostId,
                c.ParentId,
                c.Text,
                c.Embeds,
                c.Commenter,
                c.Aura
            });
            return (true, null, shaped);
        }
    }
}