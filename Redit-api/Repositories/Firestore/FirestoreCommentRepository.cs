using Redit_api.Models.DTO;
using Redit_api.Repositories.Firestore.Interfaces;

namespace Redit_api.Repositories.Firestore;

public class FirestoreCommentRepository : IFirestoreCommentRepository
{
    public Task<CommentDTO> CreateAsync(CommentDTO comment, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<CommentDTO?> GetByIdAsync(int id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(CommentDTO comment, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(CommentDTO comment, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<bool> PostExistsAsync(int postId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ParentBelongsToPostAsync(int parentId, int postId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetUsernameByEmailAsync(string email, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<List<CommentDTO>> GetByPostAsync(int postId, int skip, int take, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<List<CommentDTO>> GetByUserAsync(string username, int skip, int take, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}