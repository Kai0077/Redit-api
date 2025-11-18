using Redit_api.Models;
using Redit_api.Models.DTO;
using Redit_api.Models.Status;
using Redit_api.Repositories.Interfaces;

namespace Redit_api.Repositories.Postgresql.Interfaces
{
    public interface IPostgresCommunityRepository : ICommunityRepository
    {
        new Task<bool> ExistsAsync(string name, CancellationToken ct);
        new Task<CommunityDTO?> GetAsync(string name, CancellationToken ct);
        new Task<List<CommunityDTO>> ListAsync(int skip, int take, CancellationToken ct);
        new Task<CommunityDTO> CreateAsync(CommunityDTO community, CancellationToken ct);
        new Task<CommunityDTO> UpdateAsync(CommunityDTO community, CancellationToken ct);
        new Task<bool> DeleteAsync(string name, CancellationToken ct);
        new Task<bool> UserExistsAsync(string username, CancellationToken ct);
        new Task<string?> GetUsernameByEmailAsync(string email, CancellationToken ct);
        new Task<UserRole?> GetRoleByEmailAsync(string email, CancellationToken ct);
        new Task<List<CommunityDTO>> GetAllAsync(CancellationToken ct);
        new Task<List<CommunityDTO>> GetByUserAsync(string username, CancellationToken ct);
    }
}