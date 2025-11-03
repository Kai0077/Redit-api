using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Redit_api.Services.Interfaces;

namespace Redit_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ISentryLogger _sentryLogger;
        private readonly IUserService _service;
        public UsersController(ISentryLogger sentryLogger, IUserService service)
        {
            _sentryLogger = sentryLogger;
            _service = service;
        }

        private string? Email() =>
            User.FindFirstValue(ClaimTypes.Email) ??
            User.FindFirst("email")?.Value ??
            User.FindFirst("sub")?.Value;

        // GET /api/users  (superuser only)
        [Authorize(Roles = "super_user")]
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var email = Email();
            _sentryLogger.Info("Super user fetching all users", $"Requester: {email ?? "unknown"}");
            
            var (ok, err, users) = await _service.GetAllUsersAsync(ct);

            if (!ok)
            {
                _sentryLogger.Warn("Failed to fetch users", $"Requester: {email ?? "unknown"}, Reason: {err}");
                return BadRequest(new { message = err });
            }
            
            _sentryLogger.Success("Fetched all users successfully", $"Count: {users?.Count()}");
            return Ok(users);
        }

        // DELETE /api/users/{username} (superuser or self)
        [Authorize]
        [HttpDelete("{username}")]
        public async Task<IActionResult> Delete([FromRoute] string username, CancellationToken ct)
        {
            var requester = Email();
            if (string.IsNullOrEmpty(requester))
            {
                _sentryLogger.Warn("Unauthorized delete attempt", $"Target: {username}");
                return Unauthorized();
            }
            
            _sentryLogger.Info("Attempting user deletion", $"Requester: {requester}, Target: {username}");

            var (ok, err) = await _service.DeleteUserAsync(requester, username, ct);
            if (!ok)
            {
                if (string.Equals(err, "Forbidden.", StringComparison.OrdinalIgnoreCase))
                {
                    _sentryLogger.Warn("Forbidden user deletion attempt", $"Requester: {requester}, Target: {username}");
                    return Forbid();
                }

                if (string.Equals(err, "Target user not found.", StringComparison.OrdinalIgnoreCase))
                {
                    _sentryLogger.Warn("User deletion failed - not found", $"Requester: {requester}, Target: {username}");
                    return NotFound(new { message = err });
                }
                
                _sentryLogger.Warn("User deletion failed", $"Requester: {requester}, Target: {username}, Reason: {err}");
                return BadRequest(new { message = err });
            }
            
            _sentryLogger.Success("User deleted successfully", $"Requester: {requester}, Target: {username}");
            return NoContent();
        }

        // GET /api/users/{username}/followers (public)
        [AllowAnonymous]
        [HttpGet("{username}/followers")]
        public async Task<IActionResult> Followers([FromRoute] string username, CancellationToken ct)
        {
            _sentryLogger.Info("Fetching followers", $"Target user: {username}");
            
            var (ok, err, users) = await _service.GetFollowersAsync(username, ct);
            if (!ok)
            {
                if (string.Equals(err, "User not found.", StringComparison.OrdinalIgnoreCase))
                {
                    _sentryLogger.Warn("Followers fetch failed - user not found", $"Target: {username}");
                    return NotFound(new { message = err });
                }
                
                _sentryLogger.Warn("Followers fetch failed", $"Target: {username}, Reason: {err}");
                return BadRequest(new { message = err });
            }
            
            _sentryLogger.Success("Fetched followers successfully", $"Target: {username}, Count: {users?.Count()}");
            return Ok(users);
        }

        // GET /api/users/{username}/following (public)
        [AllowAnonymous]
        [HttpGet("{username}/following")]
        public async Task<IActionResult> Following([FromRoute] string username, CancellationToken ct)
        {
            _sentryLogger.Info("Fetching following list", $"Target user: {username}");
            
            var (ok, err, users) = await _service.GetFollowingAsync(username, ct);
            if (!ok)
            {
                if (string.Equals(err, "User not found.", StringComparison.OrdinalIgnoreCase))
                {
                    _sentryLogger.Warn("Following fetch failed - user not found", $"Target: {username}");
                    return NotFound(new { message = err });
                }
                
                _sentryLogger.Warn("Following fetch failed", $"Target: {username}, Reason: {err}");
                return BadRequest(new { message = err });
            }
            
            _sentryLogger.Success("Fetched following successfully", $"Target: {username}, Count: {users?.Count()}");
            return Ok(users);
        }
    }
}