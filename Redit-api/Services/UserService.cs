using Microsoft.AspNetCore.Identity;
using Redit_api.Models;
using Redit_api.Models.DTO;
using Redit_api.Models.Status;
using Redit_api.Repositories.Firestore.Interfaces;
using Redit_api.Repositories.Interfaces;
using Redit_api.Repositories.Neo4j.Interfaces;
using Redit_api.Repositories.Postgresql.Interfaces;
using Redit_api.Services.Interfaces;

namespace Redit_api.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IPasswordHasher<UserDTO> _hasher;
        private readonly ITokenService _tokens;
        private readonly INeo4jUserReadRepository _neoUsers;

        public UserService(
            IPostgresUserRepository postgresRepository,
            IFirestoreUserRepository firestoreRepository,
            INeo4jUserReadRepository neoUsers,
            IPasswordHasher<UserDTO> hasher,
            ITokenService tokens)
        {
            _hasher = hasher;
            _tokens = tokens;
            _neoUsers = neoUsers;

            var db = Environment.GetEnvironmentVariable("DB_TYPE")?.ToLower();

            _repository = db switch
            {
                "postgres" => postgresRepository,
                "firestore" => firestoreRepository,
                _ => throw new Exception("DB_TYPE must not be empty")
            };
        }

        public async Task<(bool Success, string? Error, object? UserData)> SignupAsync(UserSignupDTO dto, CancellationToken ct)
        {
            var username = dto.Username.Trim().ToLowerInvariant();
            var email = dto.Email.Trim().ToLowerInvariant();

            if (await _repository.UsernameExistsAsync(username, ct))
                return (false, "Username already exists.", null);

            if (await _repository.EmailExistsAsync(email, ct))
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
                Role = UserRole.User,
            };

            user.PasswordHash = _hasher.HashPassword(user, dto.Password);

            try
            {
                var created = await _repository.CreateAsync(user, ct);
                var result = new
                {
                    created.Username,
                    created.Name,
                    created.Email,
                    created.Age,
                    created.Aura,
                    created.Bio,
                    created.ProfilePicture,
                    AccountStatus = created.AccountStatus,
                    created.Role,
                };
                return (true, null, result);
            }
            catch (Exception ex)
            {
                return (false, $"Database error: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string? Error, string? Token, object? UserData)> LoginAsync(UserLoginDTO dto, CancellationToken ct)
        {
            var email = dto.Email.Trim().ToLowerInvariant();
            var user = await _repository.GetByEmailAsync(email, ct);
            if (user == null)
                return (false, "Invalid email or password.", null, null);

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
                return (false, "Invalid email or password.", null, null);

            var token = _tokens.CreateToken(user);

            var safe = new
            {
                user.Username,
                user.Name,
                user.Email,
                user.Age,
                user.Aura,
                user.Bio,
                user.ProfilePicture,
                AccountStatus = user.AccountStatus.ToString(),
                user.Role
            };

            return (true, null, token, safe);
        }

        public async Task<(bool Success, string? Error)> SetStatusAsync(string username, UserStatus status, CancellationToken ct)
        {
            var user = await _repository.GetByUsernameAsync(username.Trim().ToLowerInvariant(), ct);
            if (user == null) return (false, "User not found.");

            user.AccountStatus = status;
            await _repository.UpdateAsync(user, ct);
            return (true, null);
        }

        // ===== Admin & Relations =====

        public async Task<(bool Success, string? Error, IEnumerable<object>? Users)> GetAllUsersAsync(CancellationToken ct)
        {
            // Toggle: comment/uncomment ONE of these lines:
            // var users = await _repository.GetAllAsync(ct);   // Postgres/Firestore (based on DB_TYPE)
           var users = await _neoUsers.GetAllAsync(ct);        // Neo4j

            var shaped = users.Select(u => new
            {
                u.Username,
                u.Name,
                u.Email,
                u.Age,
                u.Aura,
                u.Bio,
                u.ProfilePicture,
                AccountStatus = u.AccountStatus.ToString(),
                Role = u.Role.ToString().ToLower()
            });

            return (true, null, shaped);
        }

        public async Task<(bool Success, string? Error)> DeleteUserAsync(string requesterEmail, string targetUsername, CancellationToken ct)
        {
            var requester = await _repository.GetByEmailAsync(requesterEmail.Trim().ToLowerInvariant(), ct);
            if (requester == null) return (false, "Requester not found.");

            var target = await _repository.GetByUsernameAsync(targetUsername.Trim().ToLowerInvariant(), ct);
            if (target == null) return (false, "Target user not found.");

            var isSelf = string.Equals(requester.Username, target.Username, StringComparison.OrdinalIgnoreCase);
            var isSuper = requester.Role == UserRole.SuperUser;
            var targetIsSuper = target.Role == UserRole.SuperUser;

            if (isSelf && isSuper)
                return (false, "Forbidden: super users cannot delete themselves.");

            if (!isSelf && isSuper && targetIsSuper)
                return (false, "Forbidden: cannot delete another super user.");

            if (!isSelf && !isSuper)
                return (false, "Forbidden.");

            try
            {
                await _repository.DeleteUserAsync(target.Username, ct);
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, $"Delete failed: {ex.Message}");
            }
        }

        public async Task<(bool Success, string? Error, IEnumerable<string>? Usernames)> GetFollowersAsync(string username, CancellationToken ct)
        {
            var exists = await _repository.GetByUsernameAsync(username.Trim().ToLowerInvariant(), ct);
            if (exists == null) return (false, "User not found.", null);

            var usernames = await _repository.GetFollowerUsernamesAsync(username, ct);
            return (true, null, usernames);
        }

        public async Task<(bool Success, string? Error, IEnumerable<string>? Usernames)> GetFollowingAsync(string username, CancellationToken ct)
        {
            var exists = await _repository.GetByUsernameAsync(username.Trim().ToLowerInvariant(), ct);
            if (exists == null) return (false, "User not found.", null);

            var usernames = await _repository.GetFollowingUsernamesAsync(username, ct);
            return (true, null, usernames);
        }
    }
}