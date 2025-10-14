// Services/Interfaces/ICommunityService.cs
using Redit_api.Models.DTO;

namespace Redit_api.Services.Interfaces
{
    public interface ICommunityService
    {
        Task<(bool Ok, string? Err, object? Data)> CreateAsync(string requesterEmail, CommunityCreateDTO dto, CancellationToken ct);
        Task<(bool Ok, string? Err, object? Data)> GetAsync(string name, CancellationToken ct);
        Task<(bool Ok, string? Err, object? Data)> ListAsync(int skip, int take, CancellationToken ct);
        Task<(bool Ok, string? Err, object? Data)> UpdateAsync(string requesterEmail, string name, CommunityUpdateDTO dto, CancellationToken ct);
        Task<(bool Ok, string? Err)> DeleteAsync(string requesterEmail, string name, CancellationToken ct);
        Task<(bool Success, string? Error, IEnumerable<object>? Data)> GetAllAsync(CancellationToken ct);
        Task<(bool Success, string? Error, IEnumerable<object>? Data)> GetByUserAsync(string requesterEmail, CancellationToken ct);
    }
}
