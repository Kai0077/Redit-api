using Redit_api.Models.DTO;

namespace Redit_api.Services.Interfaces
{
    public interface ICommentService
    {
        Task<(bool Success, string? Error, object? Data)> CreateAsync(string requesterEmail, CommentCreateDTO dto, CancellationToken ct);
        Task<(bool Success, string? Error, object? Data)> UpdateAsync(string requesterEmail, int id, CommentUpdateDTO dto, CancellationToken ct);
        Task<(bool Success, string? Error)> DeleteAsync(string requesterEmail, int id, CancellationToken ct);

        Task<(bool Success, string? Error, IEnumerable<object>? Data)> GetByPostAsync(int postId, int skip, int take, CancellationToken ct);
        Task<(bool Success, string? Error, IEnumerable<object>? Data)> GetByUserAsync(string requesterEmail, int skip, int take, CancellationToken ct);
    }
}