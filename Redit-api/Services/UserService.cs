using Microsoft.AspNetCore.Identity;
using Redit_api.Models;
using Redit_api.Models.DTO;
using Redit_api.Models.Status;
using Redit_api.Repositories.Interfaces;
using Redit_api.Services.Interfaces;

namespace Redit_api.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IPasswordHasher<UserDTO> _hasher;

        public UserService(IUserRepository repo, IPasswordHasher<UserDTO> hasher)
        {
            _repo = repo;
            _hasher = hasher;
        }

        public async Task<(bool Success, string? Error, object? UserData)> SignupAsync(UserSignupDTO dto, CancellationToken ct)
        {
            var username = dto.Username.Trim().ToLowerInvariant();
            var email = dto.Email.Trim().ToLowerInvariant();

            if (await _repo.UsernameExistsAsync(username, ct))
                return (false, "Username already exists.", null);

            if (await _repo.EmailExistsAsync(email, ct))
                return (false, "Email already in use.", null);

            var user = new UserDTO
            {
                Username = username,
                Name = dto.Name.Trim(),
                Email = email,
                Age = dto.Age,
                Aura = 0,
                Bio = null,
                ProfilePicture = null,

            };

            user.PasswordHash = _hasher.HashPassword(user, dto.Password);

            try
            {
                var created = await _repo.CreateAsync(user, ct);
                var result = new
                {
                    created.Username,
                    created.Name,
                    created.Email,
                    created.Age,
                    created.Aura,
                    created.Bio,
                    created.ProfilePicture,
                    //AccountStatus = created.AccountStatus
                };
                return (true, null, result);
            }
            catch (Exception ex)
            {
                return (false, $"Database error: {ex.Message}", null);
            }
        }
    }
}