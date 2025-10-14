// Services/Interfaces/IPostService.cs
using Redit_api.Models.DTO;

namespace Redit_api.Services.Interfaces
{
    public interface IPostService
    {
        Task<(bool Success, string? Error, object? Data)> CreateAsync(string requesterEmail, PostCreateDTO dto, CancellationToken ct);
        Task<(bool Success, string? Error, object? Data)> UpdateAsync(string requesterEmail, int id, PostUpdateDTO dto, CancellationToken ct);
        Task<(bool Success, string? Error)> DeleteAsync(string requesterEmail, int id, CancellationToken ct);
    }
}