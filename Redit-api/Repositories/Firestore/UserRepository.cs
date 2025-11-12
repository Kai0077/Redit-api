using Redit_api.Models;
using Redit_api.Repositories.Firestore.Interfaces;

namespace Redit_api.Repositories.Firestore;

public class UserRepository : IUserRepository
{
    public Task<bool> UsernameExistsAsync(string username, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<UserDTO?> GetByUsernameAsync(string username, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<UserDTO?> GetByEmailAsync(string email, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<UserDTO> CreateAsync(UserDTO user, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(UserDTO user, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<List<UserDTO>> GetAllAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task DeleteUserAsync(string username, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<List<UserDTO>> GetFollowersAsync(string username, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<List<UserDTO>> GetFollowingAsync(string username, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<List<string>> GetFollowerUsernamesAsync(string username, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<List<string>> GetFollowingUsernamesAsync(string username, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}