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
        private readonly IUserService _service;
        public UsersController(IUserService service) => _service = service;

        private string? Email() =>
            User.FindFirstValue(ClaimTypes.Email) ??
            User.FindFirst("email")?.Value ??
            User.FindFirst("sub")?.Value;

        // GET /api/users  (superuser only)
        [Authorize(Roles = "super_user")]
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var (ok, err, users) = await _service.GetAllUsersAsync(ct);
            if (!ok) return BadRequest(new { message = err });
            return Ok(users);
        }

        // DELETE /api/users/{username} (superuser or self)
        [Authorize]
        [HttpDelete("{username}")]
        public async Task<IActionResult> Delete([FromRoute] string username, CancellationToken ct)
        {
            var requester = Email();
            if (string.IsNullOrEmpty(requester)) return Unauthorized();

            var (ok, err) = await _service.DeleteUserAsync(requester, username, ct);
            if (!ok)
            {
                if (string.Equals(err, "Forbidden.", StringComparison.OrdinalIgnoreCase)) return Forbid();
                if (string.Equals(err, "Target user not found.", StringComparison.OrdinalIgnoreCase)) return NotFound(new { message = err });
                return BadRequest(new { message = err });
            }
            return NoContent();
        }

        // GET /api/users/{username}/followers (public)
        [AllowAnonymous]
        [HttpGet("{username}/followers")]
        public async Task<IActionResult> Followers([FromRoute] string username, CancellationToken ct)
        {
            var (ok, err, users) = await _service.GetFollowersAsync(username, ct);
            if (!ok)
            {
                if (string.Equals(err, "User not found.", StringComparison.OrdinalIgnoreCase)) return NotFound(new { message = err });
                return BadRequest(new { message = err });
            }
            return Ok(users);
        }

        // GET /api/users/{username}/following (public)
        [AllowAnonymous]
        [HttpGet("{username}/following")]
        public async Task<IActionResult> Following([FromRoute] string username, CancellationToken ct)
        {
            var (ok, err, users) = await _service.GetFollowingAsync(username, ct);
            if (!ok)
            {
                if (string.Equals(err, "User not found.", StringComparison.OrdinalIgnoreCase)) return NotFound(new { message = err });
                return BadRequest(new { message = err });
            }
            return Ok(users);
        }
    }
}