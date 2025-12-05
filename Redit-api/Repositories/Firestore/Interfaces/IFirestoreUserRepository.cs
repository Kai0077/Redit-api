using Redit_api.Models;
using Redit_api.Repositories.Interfaces;

namespace Redit_api.Repositories.Firestore.Interfaces
{
    public interface IFirestoreUserRepository : IUserRepository
    {
        new Task<bool> UsernameExistsAsync(string username, CancellationToken ct);
        new Task<bool> EmailExistsAsync(string email, CancellationToken ct);
        new Task<UserDTO?> GetByUsernameAsync(string username, CancellationToken ct);
        new Task<UserDTO?> GetByEmailAsync(string email, CancellationToken ct);
        new Task<UserDTO> CreateAsync(UserDTO user, CancellationToken ct);
        new Task UpdateAsync(UserDTO user, CancellationToken ct);
        new Task<List<UserDTO>> GetAllAsync(CancellationToken ct);
        new Task DeleteUserAsync(string username, CancellationToken ct);
        new Task<List<UserDTO>> GetFollowersAsync(string username, CancellationToken ct);
        new Task<List<UserDTO>> GetFollowingAsync(string username, CancellationToken ct);
        new Task<List<string>> GetFollowerUsernamesAsync(string username, CancellationToken ct);
        new Task<List<string>> GetFollowingUsernamesAsync(string username, CancellationToken ct);
    }
}