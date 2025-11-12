using Redit_api.Models;
using Redit_api.Repositories.Firestore.Interfaces;

namespace Redit_api.Repositories.Firestore;

public class PostRepository : IPostRepository
{
    public Task<PostDTO> CreateAsync(PostDTO post, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<PostDTO?> GetByIdAsync(int id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(PostDTO post, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(PostDTO post, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CommunityExistsAsync(string communityName, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetUsernameByEmailAsync(string email, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<UserDTO?> GetUserByEmailAsync(string email, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<PostDTO>> GetAllPublicAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<PostDTO>> GetAllAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<PostDTO>> GetByUserAsync(string username, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}