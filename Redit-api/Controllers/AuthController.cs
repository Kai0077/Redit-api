using Microsoft.AspNetCore.Mvc;
using Redit_api.Models;
using Redit_api.Models.DTO;
using Redit_api.Services.Interfaces;

namespace Redit_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ISentryLogger _sentryLogger;
        private readonly IUserService _service;

        public AuthController(ISentryLogger sentryLogger, IUserService service)
        {
            _sentryLogger = sentryLogger;
            _service = service;
        }

        // ==========================================
        // POST /api/auth/signup
        // ==========================================
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] UserSignupDTO dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                _sentryLogger.Warn("Signup validation failed", $"Email: {dto.Email}");
                return ValidationProblem(ModelState);
            }
            
            _sentryLogger.Info("Signup attempt", $"Email: {dto.Email}");

            var (success, error, userData) = await _service.SignupAsync(dto, ct);

            if (!success)
            {
                _sentryLogger.Warn("Signup failed", $"Email: {dto.Email}, Reason: {error}");
                return Conflict(new { message = error });
            }

            _sentryLogger.Success("User signed up successfully", $"Email: {dto.Email}, Username: {dto.Username}");
            return Ok(userData);
        }

        // ==========================================
        // POST /api/auth/login
        // ==========================================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                _sentryLogger.Warn("Login validation failed", $"Email: {dto.Email}");
                return ValidationProblem(ModelState);
            }
            
            _sentryLogger.Info("Login attempt", $"Email: {dto.Email}");
            
            var (success, error, token, user) = await _service.LoginAsync(dto, ct);
            if (!success)
            {
                _sentryLogger.Warn("Login failed", $"Email: {dto.Email}, Reason: {error}");
                return Unauthorized(new { message = error });
            }
            
            _sentryLogger.Success("User logged in successfully", $"Email: {dto.Email}");
            return Ok(new { token, user });
        }
        
        // ==========================================
        // POST /api/auth/logout
        // ==========================================
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutDTO dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                _sentryLogger.Warn("Logout validation failed", $"Username: {dto.Username}");
                return ValidationProblem(ModelState);
            }
            
            _sentryLogger.Info("Logout attempt", $"Username: {dto.Username}");

            var (ok, err) = await _service.SetStatusAsync(dto.Username, UserStatus.Offline, ct);
            if (!ok)
            {
                _sentryLogger.Warn("Logout failed", $"Username: {dto.Username}, Reason: {err}");
                return NotFound(new { message = err });
            }

            _sentryLogger.Success("User logged out successfully", $"Username: {dto.Username}");
            return Ok(new { message = "Logged out. Status set to offline." });
        }
    }
}