using Redit_api.Models;
using Redit_api.Models.DTO;
using Redit_api.Models.Status;
using Redit_api.Repositories.Postgresql.Interfaces;
using Redit_api.Services.Interfaces;

namespace Redit_api.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _posts;

        public PostService(IPostRepository posts)
        {
            _posts = posts;
        }

        public async Task<(bool Success, string? Error, object? Data)> CreateAsync(
            string requesterEmail, PostCreateDTO dto, CancellationToken ct)
        {
            var username = await _posts.GetUsernameByEmailAsync(requesterEmail, ct);
            if (string.IsNullOrEmpty(username))
                return (false, "User not found.", null);

            string? community = string.IsNullOrWhiteSpace(dto.Community) ? null : dto.Community.Trim();

            if (community != null)
            {
                var exists = await _posts.CommunityExistsAsync(community, ct);
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

            var created = await _posts.CreateAsync(post, ct);

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
            var user = await _posts.GetUserByEmailAsync(requesterEmail, ct);
            if (user == null) return (false, "User not found.", null);

            var post = await _posts.GetByIdAsync(id, ct);
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
                    var exists = await _posts.CommunityExistsAsync(community, ct);
                    if (!exists) return (false, "Community does not exist.", null);
                }
                post.Community = community;
            }

            await _posts.UpdateAsync(post, ct);

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
            var user = await _posts.GetUserByEmailAsync(requesterEmail, ct);
            if (user == null) return (false, "User not found.");

            var post = await _posts.GetByIdAsync(id, ct);
            if (post == null) return (false, "Post not found.");

            var isOwner = string.Equals(post.OriginalPoster, user.Username, StringComparison.OrdinalIgnoreCase);
            var isSuperUser = user.Role == UserRole.SuperUser;
            

            if (!isOwner && !isSuperUser)
                return (false, "Forbidden.");

            await _posts.DeleteAsync(post, ct);
            return (true, null);
        }

        public async Task<(bool Success, string? Error, IEnumerable<object?> Data)> GetAllPublicAsync(CancellationToken ct)
        {
            var posts = await _posts.GetAllPublicAsync(ct);
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
            var posts = await _posts.GetAllAsync(ct);
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
            var username = await _posts.GetUsernameByEmailAsync(requesterEmail, ct);
            if (string.IsNullOrEmpty(username))
                return (false, "User not found.", null);

            var posts = await _posts.GetByUserAsync(username, ct);
            var shaped = posts.Select(p => new {
                p.Id, p.Title, p.Description, p.Aura, p.OriginalPoster, p.Community, p.Embeds,
                Status = p.Status.ToString(), p.IsPublic, p.PublishAt
            });
            return (true, null, shaped);
        }
    }
}