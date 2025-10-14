// Controllers/CommunitiesController.cs
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
        private readonly ICommunityService _service;
        public CommunitiesController(ICommunityService service) => _service = service;

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
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var email = GetEmail();
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var (ok, err, data) = await _service.CreateAsync(email, dto, ct);
            if (!ok) return BadRequest(new { message = err });

            var c = (dynamic)data!;
            return CreatedAtAction(nameof(Get), new { name = c.Name }, data);
        }
        
        // ==========================================
        // GET /api/communities/{name}
        // ==========================================
        [AllowAnonymous]
        [HttpGet("{name}")]
        public async Task<IActionResult> Get([FromRoute] string name, CancellationToken ct)
        {
            var (ok, err, data) = await _service.GetAsync(name, ct);
            return ok ? Ok(data) : NotFound(new { message = err });
        }
        
        // ==========================================
        // GET /api/communities  (public, paginated list)
        // ==========================================
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 20, CancellationToken ct = default)
        {
            var (ok, err, data) = await _service.ListAsync(skip, take, ct);
            if (!ok) return BadRequest(new { message = err });
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
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var (ok, err, data) = await _service.UpdateAsync(email, name, dto, ct);
            if (!ok) return ForbidOrBadRequest(err);

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
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var (ok, err) = await _service.DeleteAsync(email, name, ct);
            if (!ok) return ForbidOrBadRequest(err);

            return NoContent();
        }

        // ==========================================
        // GET /api/communities/all  (superuser-only full list)
        // ==========================================
        [Authorize(Roles = "super_user")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var (ok, err, data) = await _service.GetAllAsync(ct);
            if (!ok) return BadRequest(new { message = err });
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
                return Unauthorized(new { message = "Missing email claim." });

            var (ok, err, data) = await _service.GetByUserAsync(email, ct);
            if (!ok) return BadRequest(new { message = err });
            return Ok(data);
        }

        private IActionResult ForbidOrBadRequest(string? err)
        {
            if (string.Equals(err, "Forbidden.", StringComparison.OrdinalIgnoreCase)) return Forbid();
            if (string.Equals(err, "Not found.", StringComparison.OrdinalIgnoreCase)) return NotFound(new { message = err });
            return BadRequest(new { message = err });
        }
    }
}