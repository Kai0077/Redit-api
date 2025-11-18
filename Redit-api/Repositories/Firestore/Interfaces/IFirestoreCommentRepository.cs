using Redit_api.Models.DTO;

namespace Redit_api.Repositories.Firestore.Interfaces
{
    public interface IFirestoreCommentRepository
    {
        Task<CommentDTO> CreateAsync(CommentDTO comment, CancellationToken ct);
        Task<CommentDTO?> GetByIdAsync(int id, CancellationToken ct);
        Task UpdateAsync(CommentDTO comment, CancellationToken ct);
        Task DeleteAsync(CommentDTO comment, CancellationToken ct);

        Task<bool> PostExistsAsync(int postId, CancellationToken ct);
        Task<bool> ParentBelongsToPostAsync(int parentId, int postId, CancellationToken ct);
        Task<string?> GetUsernameByEmailAsync(string email, CancellationToken ct);

        Task<List<CommentDTO>> GetByPostAsync(int postId, int skip, int take, CancellationToken ct);
        Task<List<CommentDTO>> GetByUserAsync(string username, int skip, int take, CancellationToken ct);
    }
}