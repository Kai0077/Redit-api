using Redit_api.Models;
using Redit_api.Models.DTO;
using Redit_api.Models.Status;
using Redit_api.Repositories.Firestore.Interfaces;
using Redit_api.Repositories.Postgresql.Interfaces;
using Redit_api.Services.Interfaces;
using Redit_api.Repositories.Interfaces;

namespace Redit_api.Services
{
    public class CommunityService : ICommunityService
    {
        private readonly ICommunityRepository _repo;

        public CommunityService(IPostgresCommunityRepository postgresRepo, IFirestoreCommunityRepository firestoreRepo)
        {
            var db = Environment.GetEnvironmentVariable("DB_TYPE")?.ToLower();
            _repo = db switch
            {
                "postgres" => postgresRepo,
                "firestore" => firestoreRepo,
                _ => throw new Exception("DB_TYPE must not be empty")
            };
        }

        public async Task<(bool Ok, string? Err, object? Data)> CreateAsync(string requesterEmail, CommunityCreateDTO dto, CancellationToken ct)
        {
            var name = dto.Name.Trim();
            if (await _repo.ExistsAsync(name, ct))
                return (false, "Community name already exists.", null);

            // Owner: prefer explicit OwnerUsername, else from token email
            var owner = string.IsNullOrWhiteSpace(dto.OwnerUsername)
                ? await _repo.GetUsernameByEmailAsync(requesterEmail, ct)
                : dto.OwnerUsername.Trim();

            if (!string.IsNullOrEmpty(owner) && !await _repo.UserExistsAsync(owner, ct))
                return (false, "Owner user does not exist.", null);

            var entity = new CommunityDTO
            {
                Name = name,
                Description = dto.Description,
                ProfilePicture = dto.ProfilePicture,
                OwnerUsername = owner,
                PinnedPostIds = System.Array.Empty<int>()
            };

            var created = await _repo.CreateAsync(entity, ct);
            return (true, null, created);
        }

        public async Task<(bool Ok, string? Err, object? Data)> GetAsync(string name, CancellationToken ct)
        {
            var c = await _repo.GetAsync(name, ct);
            return c is null ? (false, "Not found.", null) : (true, null, c);
        }

        public async Task<(bool Ok, string? Err, object? Data)> ListAsync(int skip, int take, CancellationToken ct)
        {
            var list = await _repo.ListAsync(skip, take <= 0 ? 20 : take, ct);
            return (true, null, list);
        }

        public async Task<(bool Ok, string? Err, object? Data)> UpdateAsync(string requesterEmail, string name, CommunityUpdateDTO dto, CancellationToken ct)
        {
            var existing = await _repo.GetAsync(name, ct);
            if (existing is null) return (false, "Not found.", null);

            // auth: must be owner or super_user
            var requester = await _repo.GetUsernameByEmailAsync(requesterEmail, ct);
            var role = await _repo.GetRoleByEmailAsync(requesterEmail, ct);
            var isOwner = !string.IsNullOrEmpty(requester) && requester == existing.OwnerUsername;
            var isSuper = role == UserRole.SuperUser;
            if (!isOwner && !isSuper) return (false, "Forbidden.", null);

            if (dto.Description != null) existing.Description = dto.Description;
            if (dto.ProfilePicture != null) existing.ProfilePicture = dto.ProfilePicture;

            if (dto.OwnerUsername != null)
            {
                if (!await _repo.UserExistsAsync(dto.OwnerUsername, ct))
                    return (false, "New owner does not exist.", null);
                existing.OwnerUsername = dto.OwnerUsername;
            }

            if (dto.PinnedPostIds != null)
                existing.PinnedPostIds = dto.PinnedPostIds;

            var updated = await _repo.UpdateAsync(existing, ct);
            return (true, null, updated);
        }

        public async Task<(bool Ok, string? Err)> DeleteAsync(string requesterEmail, string name, CancellationToken ct)
        {
            var existing = await _repo.GetAsync(name, ct);
            if (existing is null) return (false, "Not found.");

            var requester = await _repo.GetUsernameByEmailAsync(requesterEmail, ct);
            var role = await _repo.GetRoleByEmailAsync(requesterEmail, ct);
            var isOwner = !string.IsNullOrEmpty(requester) && requester == existing.OwnerUsername;
            var isSuper = role == UserRole.SuperUser;
            if (!isOwner && !isSuper) return (false, "Forbidden.");

            var ok = await _repo.DeleteAsync(name, ct);
            return ok ? (true, null) : (false, "Delete failed.");
        }
        
        public async Task<(bool Success, string? Error, IEnumerable<object>? Data)> GetAllAsync(CancellationToken ct)
        {
            var list = await _repo.GetAllAsync(ct);
            var shaped = list.Select(c => new
            {
                c.Name,
                c.Description,
                c.ProfilePicture,
                c.OwnerUsername,
                c.PinnedPostIds
            });
            return (true, null, shaped);
        }

        public async Task<(bool Success, string? Error, IEnumerable<object>? Data)> GetByUserAsync(string requesterEmail, CancellationToken ct)
        {
            var username = await _repo.GetUsernameByEmailAsync(requesterEmail, ct);
            if (string.IsNullOrEmpty(username))
                return (false, "User not found.", null);

            var list = await _repo.GetByUserAsync(username, ct);
            var shaped = list.Select(c => new
            {
                c.Name,
                c.Description,
                c.ProfilePicture,
                c.OwnerUsername,
                c.PinnedPostIds
            });
            return (true, null, shaped);
        }
    }
}