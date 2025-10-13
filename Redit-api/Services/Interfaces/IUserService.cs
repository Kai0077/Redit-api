using Redit_api.Models;
using Redit_api.Models.DTO;

namespace Redit_api.Services.Interfaces
{
    public interface IUserService
    {
        Task<(bool Success, string? Error, object? UserData)> SignupAsync(UserSignupDTO dto, CancellationToken ct);
        
        Task<(bool Success, string? Error, string? Token, object? UserData)>
            LoginAsync(UserLoginDTO dto, CancellationToken ct);
        
        Task<(bool Success, string? Error)> SetStatusAsync(string username, UserStatus status, CancellationToken ct);

    }
}

