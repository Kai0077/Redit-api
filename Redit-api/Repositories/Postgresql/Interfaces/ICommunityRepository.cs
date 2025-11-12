using Redit_api.Models;
using Redit_api.Models.DTO;
using Redit_api.Models.Status;

namespace Redit_api.Repositories.Postgresql.Interfaces
{
    public interface ICommunityRepository
    {
        Task<bool> ExistsAsync(string name, CancellationToken ct);
        Task<CommunityDTO?> GetAsync(string name, CancellationToken ct);
        Task<List<CommunityDTO>> ListAsync(int skip, int take, CancellationToken ct);
        Task<CommunityDTO> CreateAsync(CommunityDTO community, CancellationToken ct);
        Task<CommunityDTO> UpdateAsync(CommunityDTO community, CancellationToken ct);
        Task<bool> DeleteAsync(string name, CancellationToken ct);
        Task<bool> UserExistsAsync(string username, CancellationToken ct);
        Task<string?> GetUsernameByEmailAsync(string email, CancellationToken ct);
        Task<UserRole?> GetRoleByEmailAsync(string email, CancellationToken ct);
        Task<List<CommunityDTO>> GetAllAsync(CancellationToken ct);
        Task<List<CommunityDTO>> GetByUserAsync(string username, CancellationToken ct);
    }
}