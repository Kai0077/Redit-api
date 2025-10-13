using Microsoft.AspNetCore.Mvc;
using Redit_api.Models;
using Redit_api.Models.DTO;
using Redit_api.Services.Interfaces;

namespace Redit_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] UserSignupDTO dto, CancellationToken ct)
        {
            if (!ModelState.IsValid) 
                return ValidationProblem(ModelState);

            var (success, error, userData) = await _service.SignupAsync(dto, ct);

            if (!success)
                return Conflict(new { message = error });

            return Ok(userData);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO dto, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var (success, error, token, user) = await _service.LoginAsync(dto, ct);
            return success ? Ok(new { token, user }) : Unauthorized(new { message = error });
        }
        
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutDTO dto, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var (ok, err) = await _service.SetStatusAsync(dto.Username, UserStatus.Offline, ct);
            if (!ok) return NotFound(new { message = err });

            // Client must delete its stored JWT; server just updates status.
            return Ok(new { message = "Logged out. Status set to offline." });
        }
    }
}