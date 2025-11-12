using Redit_api.Models;

namespace Redit_api.Repositories.Firestore.Interfaces
{
    public interface IPostRepository
    {
        Task<PostDTO> CreateAsync(PostDTO post, CancellationToken ct);
        Task<PostDTO?> GetByIdAsync(int id, CancellationToken ct);
        Task UpdateAsync(PostDTO post, CancellationToken ct);
        Task DeleteAsync(PostDTO post, CancellationToken ct);

        Task<bool> CommunityExistsAsync(string communityName, CancellationToken ct);
        Task<string?> GetUsernameByEmailAsync(string email, CancellationToken ct);
        Task<UserDTO?> GetUserByEmailAsync(string email, CancellationToken ct);
        Task<IEnumerable<PostDTO>> GetAllPublicAsync(CancellationToken ct);
        Task<IEnumerable<PostDTO>> GetAllAsync(CancellationToken ct);        
        Task<IEnumerable<PostDTO>> GetByUserAsync(string username, CancellationToken ct); 
    }
}