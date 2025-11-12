using Redit_api.Models.DTO;
using Redit_api.Models.Status;
using Redit_api.Repositories.Firestore.Interfaces;

namespace Redit_api.Repositories.Firestore;

public class CommunityRepository : ICommunityRepository
{
    public Task<bool> ExistsAsync(string name, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<CommunityDTO?> GetAsync(string name, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<List<CommunityDTO>> ListAsync(int skip, int take, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<CommunityDTO> CreateAsync(CommunityDTO community, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<CommunityDTO> UpdateAsync(CommunityDTO community, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(string name, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UserExistsAsync(string username, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetUsernameByEmailAsync(string email, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<UserRole?> GetRoleByEmailAsync(string email, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<List<CommunityDTO>> GetAllAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<List<CommunityDTO>> GetByUserAsync(string username, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}