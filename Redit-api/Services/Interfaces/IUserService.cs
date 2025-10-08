using Redit_api.Models.DTO;

namespace Redit_api.Services.Interfaces
{
    public interface IUserService
    {
        Task<(bool Success, string? Error, object? UserData)> SignupAsync(UserSignupDTO dto, CancellationToken ct);
    }
}

