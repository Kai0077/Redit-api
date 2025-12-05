using Redit_api.Models.DTO;
using Redit_api.Repositories.Interfaces;

namespace Redit_api.Repositories.Firestore.Interfaces
{
    public interface IFirestoreCommentRepository : ICommentRepository 
    {
        new Task<CommentDTO> CreateAsync(CommentDTO comment, CancellationToken ct);
        new Task<CommentDTO?> GetByIdAsync(int id, CancellationToken ct);
        new Task UpdateAsync(CommentDTO comment, CancellationToken ct);
        new Task DeleteAsync(CommentDTO comment, CancellationToken ct);
        new Task<bool> PostExistsAsync(int postId, CancellationToken ct);
        new Task<bool> ParentBelongsToPostAsync(int parentId, int postId, CancellationToken ct);
        new Task<string?> GetUsernameByEmailAsync(string email, CancellationToken ct);
        new Task<List<CommentDTO>> GetByPostAsync(int postId, int skip, int take, CancellationToken ct);
        new Task<List<CommentDTO>> GetByUserAsync(string username, int skip, int take, CancellationToken ct);
    }
}