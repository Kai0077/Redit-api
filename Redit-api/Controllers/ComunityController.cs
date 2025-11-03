using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Redit_api.Models.DTO;
using Redit_api.Services.Interfaces;

namespace Redit_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommunitiesController : ControllerBase
    {
        private readonly ISentryLogger _sentryLogger;
        private readonly ICommunityService _service;

        public CommunitiesController(ISentryLogger sentryLogger, ICommunityService service)
        {
            _sentryLogger = sentryLogger;
            _service = service;
        }

        private string? GetEmail() =>
            User.FindFirstValue(ClaimTypes.Email) ??
            User.FindFirst("email")?.Value ??
            User.FindFirst("sub")?.Value;
        
        // ==========================================
        // POST /api/communities
        // ==========================================
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CommunityCreateDTO dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                _sentryLogger.Warn("Community creation validation failed");
                return ValidationProblem(ModelState);
            }
            
            var email = GetEmail();
            if (string.IsNullOrEmpty(email))
            {
                _sentryLogger.Warn("Unauthorized community creation attempt");
                return Unauthorized();
            }

            _sentryLogger.Info("Creating community", $"User: {email}, Name: {dto.Name}");
            
            var (ok, err, data) = await _service.CreateAsync(email, dto, ct);
            if (!ok)
            {
                _sentryLogger.Warn("Community creation failed", $"User: {email}, Name: {dto.Name}, Reason: {err}");
                return BadRequest(new { message = err });
            }

            var c = (dynamic)data!;
            _sentryLogger.Success("Community created successfully", $"User: {email}, Name: {c.Name}");
            return CreatedAtAction(nameof(Get), new { name = c.Name }, data);
        }
        
        // ==========================================
        // GET /api/communities/{name}
        // ==========================================
        [AllowAnonymous]
        [HttpGet("{name}")]
        public async Task<IActionResult> Get([FromRoute] string name, CancellationToken ct)
        {
            _sentryLogger.Info("Fetching community by name", $"Name: {name}");
            
            var (ok, err, data) = await _service.GetAsync(name, ct);
            if (!ok)
            {
                _sentryLogger.Warn("Community not found", $"Name: {name}");
                return NotFound(new { message = err });
            }

            _sentryLogger.Success("Fetched community successfully", $"Name: {name}");
            return Ok(data);
        }
        
        // ==========================================
        // GET /api/communities  (public, paginated list)
        // ==========================================
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 20, CancellationToken ct = default)
        {
            _sentryLogger.Info("Fetching community list", $"Skip: {skip}, Take: {take}");
            var (ok, err, data) = await _service.ListAsync(skip, take, ct);
            if (!ok)
            {
                _sentryLogger.Warn("Failed to fetch community list", $"Reason: {err}");
                return BadRequest(new { message = err });
            }
            
            _sentryLogger.Success("Fetched community list successfully");
            return Ok(data);
        }

        // ==========================================
        // PUT /api/communities/{name}
        // ==========================================
        [Authorize]
        [HttpPut("{name}")]
        public async Task<IActionResult> Update([FromRoute] string name, [FromBody] CommunityUpdateDTO dto, CancellationToken ct)
        {
            var email = GetEmail();
            if (string.IsNullOrEmpty(email))
            {
                _sentryLogger.Warn("Unauthorized community update attempt", $"Target: {name}");
                return Unauthorized();
            }
            
            _sentryLogger.Info("Updating community", $"User: {email}, Name: {name}");

            var (ok, err, data) = await _service.UpdateAsync(email, name, dto, ct);
            if (!ok)
            {
                _sentryLogger.Warn("Community update failed", $"User: {email}, Name: {name}, Reason: {err}");
                return ForbidOrBadRequest(err);
            }

            _sentryLogger.Success("Community updated successfully", $"User: {email}, Name: {name}");
            return Ok(data);
        }

        // ==========================================
        // DELETE /api/communities/{name}
        // ==========================================
        [Authorize]
        [HttpDelete("{name}")]
        public async Task<IActionResult> Delete([FromRoute] string name, CancellationToken ct)
        {
            var email = GetEmail();
            if (string.IsNullOrEmpty(email))
            {
                _sentryLogger.Warn("Unauthorized community deletion attempt", $"Target: {name}");
                return Unauthorized();
            }
            
            _sentryLogger.Info("Deleting community", $"User: {email}, Name: {name}");

            var (ok, err) = await _service.DeleteAsync(email, name, ct);
            if (!ok)
            {
                _sentryLogger.Warn("Community deletion failed", $"User: {email}, Name: {name}, Reason: {err}");
                return ForbidOrBadRequest(err);
            }

            _sentryLogger.Success("Community deleted successfully", $"User: {email}, Name: {name}");
            return NoContent();
        }

        // ==========================================
        // GET /api/communities/all  (superuser-only full list)
        // ==========================================
        [Authorize(Roles = "super_user")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            _sentryLogger.Info("Super user fetching all communities");
            
            var (ok, err, data) = await _service.GetAllAsync(ct);
            if (!ok)
            {
                _sentryLogger.Warn("Failed to fetch all communities", $"Reason: {err}");
                return BadRequest(new { message = err });
            }
            
            _sentryLogger.Success("Fetched all communities successfully");
            return Ok(data);
        }

        // ==========================================
        // GET /api/communities/user  (communities for logged-in user)
        // ==========================================
        [Authorize]
        [HttpGet("user")]
        public async Task<IActionResult> GetUserCommunities(CancellationToken ct)
        {
            var email = GetEmail();
            if (string.IsNullOrEmpty(email))
            {
                _sentryLogger.Warn("Unauthorized user community fetch attempt");
                return Unauthorized(new { message = "Missing email claim." });
            }
            
            _sentryLogger.Info("Fetching user communities", $"User: {email}");

            var (ok, err, data) = await _service.GetByUserAsync(email, ct);
            if (!ok)
            {
                _sentryLogger.Warn("Failed to fetch user communities", $"User: {email}, Reason: {err}");
                return BadRequest(new { message = err });
            }
            
            _sentryLogger.Success("Fetched user communities successfully", $"User: {email}");
            return Ok(data);
        }

        private IActionResult ForbidOrBadRequest(string? err)
        {
            if (string.Equals(err, "Forbidden.", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            if (string.Equals(err, "Not found.", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(new { message = err });
            }
            
            return BadRequest(new { message = err });
        }
    }
}