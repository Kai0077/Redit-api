using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Redit_api.Models.DTO;
using Redit_api.Services.Interfaces;

namespace Redit_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _service;
        public CommentsController(ICommentService service) => _service = service;

        // ==========================================
        // HELPERS: GET EMAIL CLAIM FROM JWT
        // ==========================================
        private string? Email() =>
            User.FindFirstValue(ClaimTypes.Email) ??
            User.FindFirst("email")?.Value ??
            User.FindFirst("sub")?.Value;

        // ==========================================
        // CREATE COMMENT (requires login)
        // POST /api/comments
        // ==========================================
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CommentCreateDTO dto, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var email = Email(); 
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var (ok, err, data) = await _service.CreateAsync(email, dto, ct);
            if (!ok) return BadRequest(new { message = err });

            var c = (dynamic)data!;
            return CreatedAtAction(nameof(GetById), new { id = c.Id }, data);
        }

        // ==========================================
        // UPDATE COMMENT (owner or superuser)
        // PUT /api/comments/{id}
        // ==========================================
        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CommentUpdateDTO dto, CancellationToken ct)
        {
            var email = Email(); 
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var (ok, err, data) = await _service.UpdateAsync(email, id, dto, ct);
            if (!ok)
            {
                if (string.Equals(err, "Forbidden.", StringComparison.OrdinalIgnoreCase)) return Forbid();
                if (string.Equals(err, "Not found.", StringComparison.OrdinalIgnoreCase)) return NotFound(new { message = err });
                return BadRequest(new { message = err });
            }
            return Ok(data);
        }

        // ==========================================
        // DELETE COMMENT (owner or superuser)
        // DELETE /api/comments/{id}
        // ==========================================
        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var email = Email(); 
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var (ok, err) = await _service.DeleteAsync(email, id, ct);
            if (!ok)
            {
                if (string.Equals(err, "Forbidden.", StringComparison.OrdinalIgnoreCase)) return Forbid();
                if (string.Equals(err, "Not found.", StringComparison.OrdinalIgnoreCase)) return NotFound(new { message = err });
                return BadRequest(new { message = err });
            }
            return NoContent();
        }

        // ==========================================
// GET COMMENTS BY LOGGED-IN USER (paginated)
// GET /api/comments/user?skip=0&take=20
// ==========================================
        [Authorize]
        [HttpGet("user")]
        public async Task<IActionResult> GetUserComments(
            [FromQuery] int skip = 0, 
            [FromQuery] int take = 20, 
            CancellationToken ct = default)
        {
            var email = Email(); 
            if (string.IsNullOrEmpty(email)) 
                return Unauthorized(new { message = "Missing email claim." });

            var (ok, err, data) = await _service.GetByUserAsync(email, skip, take, ct);
            if (!ok) 
                return BadRequest(new { message = err });

            return Ok(data);
        }

        // ==========================================
        // GET COMMENT BY ID (stub for CreatedAtAction)
        // GET /api/comments/{id}
        // ==========================================
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id) => Ok(new { id });
    }
}