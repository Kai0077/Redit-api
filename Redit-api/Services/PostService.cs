using Redit_api.Data;
using Redit_api.Models;
using Redit_api.Models.DTO;
using Redit_api.Models.Status;
using Redit_api.Repositories.Postgresql;
using Redit_api.Repositories.Firestore;
using Redit_api.Repositories.Firestore.Interfaces;
using Redit_api.Repositories.Postgresql.Interfaces;
using Redit_api.Services.Interfaces;
using Redit_api.Repositories.Interfaces;

namespace Redit_api.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _repository;
        private readonly AppDBContext _appDbContext;

        public PostService(IPostgresPostRepository postgresRepository, IFirestorePostRepository firestoreRepository, AppDBContext appDbContext)
        {
            _appDbContext = appDbContext;

            var db = Environment.GetEnvironmentVariable("DB_TYPE")?.ToLower();

            _repository = db switch
            {
                "postgres" => postgresRepository,
                "firestore" => firestoreRepository,
                _ => throw new Exception("DB_TYPE must not be empty")
            };
        }

        public async Task<(bool Success, string? Error, object? Data)> CreateAsync(
            string requesterEmail, PostCreateDTO dto, CancellationToken ct)
        {
            var username = await _repository.GetUsernameByEmailAsync(requesterEmail, ct);
            if (string.IsNullOrEmpty(username))
                return (false, "User not found.", null);

            string? community = string.IsNullOrWhiteSpace(dto.Community) ? null : dto.Community.Trim();

            if (community != null)
            {
                var exists = await _repository.CommunityExistsAsync(community, ct);
                if (!exists) return (false, "Community does not exist.", null);
            }
            var title = (dto.Title ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(title))
                return (false, "Title is required.", null);

            var plain = HtmlUtils.HtmlToPlainText(dto.DescriptionHtml);
            if (string.IsNullOrWhiteSpace(plain))
                return (false, "Description is required.", null);

            DateTime publishAt;
            bool isPublish;

            if (dto.PublishAt.HasValue)
            {
                var publishAtUtc = dto.PublishAt.Value.ToUniversalTime();
                var nowUtc = DateTime.UtcNow;

                if (publishAtUtc < nowUtc)
                    return (false, "Publish time cannot be in the past.", null);

                publishAt = publishAtUtc;
                isPublish = false;
            }
            else
            {
                publishAt = DateTime.UtcNow;
                isPublish = true;
            }
            
            var post = new PostDTO
            {
                Title = dto.Title.Trim(),
                Description = plain,
                OriginalPoster = username,
                Community = community,
                Embeds = dto.Embeds ?? Array.Empty<string>(),
                Status = dto.Status ?? PostStatus.Active,
                Aura = 0,
                PublishAt = publishAt,
                IsPublic = isPublish
            };

            var created = await _repository.CreateAsync(post, ct);

            var result = new
            {
                created.Id,
                created.Title,
                created.Description,
                created.Aura,
                created.OriginalPoster,
                created.Community,
                created.Embeds,
                Status = created.Status.ToString(),
                created.IsPublic,
                created.PublishAt
            };

            return (true, null, result);
        }

        public async Task<(bool Success, string? Error, object? Data)> UpdateAsync(
            string requesterEmail, int id, PostUpdateDTO dto, CancellationToken ct)
        {
            var user = await _repository.GetUserByEmailAsync(requesterEmail, ct);
            if (user == null) return (false, "User not found.", null);

            var post = await _repository.GetByIdAsync(id, ct);
            if (post == null) return (false, "Post not found.", null);

            var isOwner = string.Equals(post.OriginalPoster, user.Username, StringComparison.OrdinalIgnoreCase);
            var isSuperUser = user.Role == UserRole.SuperUser;

            if (!isOwner && !isSuperUser)
                return (false, "Forbidden.", null);
            
            if (!string.IsNullOrWhiteSpace(dto.Title)) post.Title = dto.Title.Trim();
            if (dto.Description != null)
            {
                var plain = HtmlUtils.HtmlToPlainText(dto.Description);

                post.Description = plain;
                
            }
            
            if (dto.Embeds != null) post.Embeds = dto.Embeds;
            if (dto.Status.HasValue) post.Status = dto.Status.Value;

            if (dto.Community != null)
            {
                var community = string.IsNullOrWhiteSpace(dto.Community) ? null : dto.Community.Trim();
                if (community != null)
                {
                    var exists = await _repository.CommunityExistsAsync(community, ct);
                    if (!exists) return (false, "Community does not exist.", null);
                }
                post.Community = community;
            }

            await using (var transaction = await _appDbContext.Database.BeginTransactionAsync(ct))
            {
                await _appDbContext.SetAppUsernameAsync(user.Username, ct);
                await _repository.UpdateAsync(post, ct);
                await transaction.CommitAsync(ct);
            }

            var result = new
            {
                post.Id,
                post.Title,
                post.Description,
                post.Aura,
                post.OriginalPoster,
                post.Community,
                post.Embeds,
                Status = post.Status.ToString()
            };

            return (true, null, result);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(
            string requesterEmail, int id, CancellationToken ct)
        {
            var user = await _repository.GetUserByEmailAsync(requesterEmail, ct);
            if (user == null) return (false, "User not found.");

            var post = await _repository.GetByIdAsync(id, ct);
            if (post == null) return (false, "Post not found.");

            var isOwner = string.Equals(post.OriginalPoster, user.Username, StringComparison.OrdinalIgnoreCase);
            var isSuperUser = user.Role == UserRole.SuperUser;
            

            if (!isOwner && !isSuperUser)
                return (false, "Forbidden.");

            await using (var transaction = await _appDbContext.Database.BeginTransactionAsync(ct))
            {
                await _appDbContext.SetAppUsernameAsync(user.Username, ct);
                await _repository.DeleteAsync(post, ct);
                await transaction.CommitAsync(ct);
            }

            return (true, null);
        }

        public async Task<(bool Success, string? Error, IEnumerable<object?> Data)> GetAllPublicAsync(CancellationToken ct)
        {
            var posts = await _repository.GetAllPublicAsync(ct);
            var shaped = posts.Select(p => new
            {
                p.Id, 
                p.Title, 
                p.Description, 
                p.Aura, 
                p.OriginalPoster, 
                p.Community, 
                p.Embeds,
                Status = p.Status.ToString()
            });
            return (true, null, shaped);
        }
        
        public async Task<(bool Success, string? Error, IEnumerable<object>? Data)> GetAllAsync(CancellationToken ct)
        {
            var posts = await _repository.GetAllAsync(ct);
            var shaped = posts.Select(p => new 
            {
                p.Id, 
                p.Title, 
                p.Description, 
                p.Aura, 
                p.OriginalPoster, 
                p.Community, 
                p.Embeds,
                Status = p.Status.ToString()
            });
            return (true, null, shaped);
        }

        public async Task<(bool Success, string? Error, IEnumerable<object>? Data)> GetByUserAsync(
            string requesterEmail, CancellationToken ct)
        {
            var username = await _repository.GetUsernameByEmailAsync(requesterEmail, ct);
            if (string.IsNullOrEmpty(username))
                return (false, "User not found.", null);

            var posts = await _repository.GetByUserAsync(username, ct);
            var shaped = posts.Select(p => new {
                p.Id, p.Title, p.Description, p.Aura, p.OriginalPoster, p.Community, p.Embeds,
                Status = p.Status.ToString(), p.IsPublic, p.PublishAt
            });
            return (true, null, shaped);
        }
    }
}