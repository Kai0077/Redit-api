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
        private readonly ISentryLogger _sentryLogger;
        private readonly ICommentService _service;

        public CommentsController(ISentryLogger sentryLogger, ICommentService service)
        {
            _sentryLogger = sentryLogger;
            _service = service;
        }

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
            if (!ModelState.IsValid)
            {
                _sentryLogger.Warn("Comment creation validation failed");
                return ValidationProblem(ModelState);
            }
            var email = Email();
            if (string.IsNullOrEmpty(email))
            {
                _sentryLogger.Warn("Unauthorized comment creation attempt");
                return Unauthorized();
            }
            
            _sentryLogger.Info("Creating comment", $"User: {email}, PostId: {dto.PostId}");

            var (ok, err, data) = await _service.CreateAsync(email, dto, ct);
            if (!ok)
            {
                _sentryLogger.Warn("Comment creation failed", $"User: {email}, Reason: {err}");
                return BadRequest(new { message = err });
            }

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
            if (string.IsNullOrEmpty(email))
            {
                _sentryLogger.Warn("Unauthorized comment update attempt");
                return Unauthorized();
            }
            
            _sentryLogger.Info("Updating comment", $"User: {email}, CommentId: {id}");

            var (ok, err, data) = await _service.UpdateAsync(email, id, dto, ct);
            if (!ok)
            {
                if (string.Equals(err, "Forbidden.", StringComparison.OrdinalIgnoreCase))
                {
                    _sentryLogger.Warn("Comment update forbidden", $"User: {email}, CommentId: {id}");
                    return Forbid();
                }

                if (string.Equals(err, "Not found.", StringComparison.OrdinalIgnoreCase))
                {
                    _sentryLogger.Warn("Comment update failed - not found", $"User: {email}, CommentId: {id}");
                    return NotFound(new { message = err });
                }
                
                _sentryLogger.Warn("Comment update failed", $"User: {email}, CommentId: {id}, Reason: {err}");
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
            if (string.IsNullOrEmpty(email))
            {
                _sentryLogger.Warn("Unauthorized comment deletion attempt");
                return Unauthorized();
            }
            
            _sentryLogger.Info("Deleting comment", $"User: {email}, CommentId: {id}");

            var (ok, err) = await _service.DeleteAsync(email, id, ct);
            if (!ok)
            {
                if (string.Equals(err, "Forbidden.", StringComparison.OrdinalIgnoreCase))
                {
                    _sentryLogger.Warn("Comment deletion forbidden", $"User: {email}, CommentId: {id}");
                    return Forbid();
                }

                if (string.Equals(err, "Not found.", StringComparison.OrdinalIgnoreCase))
                {
                    _sentryLogger.Warn("Comment deletion failed - not found", $"User: {email}, CommentId: {id}");
                    return NotFound(new { message = err });
                }
                
                _sentryLogger.Warn("Comment deletion failed", $"User: {email}, CommentId: {id}, Reason: {err}");
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
            {
                _sentryLogger.Warn("Unauthorized user comments fetch attempt");
                return Unauthorized(new { message = "Missing email claim." });
            }
            
            _sentryLogger.Info("Fetching user comments", $"User: {email}, Skip: {skip}, Take: {take}");

            var (ok, err, data) = await _service.GetByUserAsync(email, skip, take, ct);
            if (!ok)
            {
                _sentryLogger.Warn("Failed to fetch user comments", $"User: {email}, Reason: {err}");
                return BadRequest(new { message = err });
            }
            
            return Ok(data);
        }

        // ==========================================
        // GET COMMENT BY ID (stub for CreatedAtAction)
        // GET /api/comments/{id}
        // ==========================================
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            _sentryLogger.Info("Comment lookup by ID", $"CommentId: {id}");
            return Ok(new { id });
        }
    }
}