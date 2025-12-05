using Redit_api.Models;
using Redit_api.Repositories.Interfaces;

namespace Redit_api.Repositories.Postgresql.Interfaces
{
    public interface IPostgresPostRepository : IPostRepository
    {
        new Task<PostDTO> CreateAsync(PostDTO post, CancellationToken ct);
        new Task<PostDTO?> GetByIdAsync(int id, CancellationToken ct);
        new Task UpdateAsync(PostDTO post, CancellationToken ct);
        new Task DeleteAsync(PostDTO post, CancellationToken ct);
        new Task<bool> CommunityExistsAsync(string communityName, CancellationToken ct);
        new Task<string?> GetUsernameByEmailAsync(string email, CancellationToken ct);
        new Task<UserDTO?> GetUserByEmailAsync(string email, CancellationToken ct);
        new Task<IEnumerable<PostDTO>> GetAllPublicAsync(CancellationToken ct);
        new Task<IEnumerable<PostDTO>> GetAllAsync(CancellationToken ct);
        new Task<IEnumerable<PostDTO>> GetByUserAsync(string username, CancellationToken ct); 
    }
}