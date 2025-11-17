using Redit_api.Models;

namespace Redit_api.Repositories.Postgresql.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> UsernameExistsAsync(string username, CancellationToken ct);
        Task<bool> EmailExistsAsync(string email, CancellationToken ct);
        Task<UserDTO?> GetByUsernameAsync(string username, CancellationToken ct);
        Task<UserDTO?> GetByEmailAsync(string email, CancellationToken ct);
        Task<UserDTO> CreateAsync(UserDTO user, CancellationToken ct);
        Task UpdateAsync(UserDTO user, CancellationToken ct);
        Task<List<UserDTO>> GetAllAsync(CancellationToken ct);
        Task DeleteUserAsync(string username, CancellationToken ct);
        Task<List<UserDTO>> GetFollowersAsync(string username, CancellationToken ct);
        Task<List<UserDTO>> GetFollowingAsync(string username, CancellationToken ct);
        Task<List<string>> GetFollowerUsernamesAsync(string username, CancellationToken ct);
        Task<List<string>> GetFollowingUsernamesAsync(string username, CancellationToken ct);
    }
}