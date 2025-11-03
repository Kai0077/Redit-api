// Controllers/PostsController.cs
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Redit_api.Models.DTO;
using Redit_api.Services.Interfaces;

namespace Redit_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly ISentryLogger _sentryLogger;
        private readonly IPostService _service;

        public PostsController(ISentryLogger sentryLogger, IPostService service)
        {
            _sentryLogger = sentryLogger;
            _service = service;
        }

        // ==========================================
        // CREATE POST (requires login)
        // ==========================================
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PostCreateDTO dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                _sentryLogger.Warn("Invalid model on post creation");
                return ValidationProblem(ModelState);
            }

            var email = User.FindFirstValue(ClaimTypes.Email)
                        ?? User.FindFirst("email")?.Value
                        ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                _sentryLogger.Warn("Missing email claim on post creation");
                return Unauthorized(new { message = "Missing email claim." });
            }
            
            _sentryLogger.Info("Starting post creation", $"User: {email}");

            var (ok, err, data) = await _service.CreateAsync(email, dto, ct);
            if (!ok)
            {
                _sentryLogger.Warn("Post creation failed", $"User: {email}, Reason: {err}");
                return BadRequest(new { message = err });
            }

            _sentryLogger.Success("Post created successfully", $"User: {email}, PostId: {((dynamic)data).Id}");
            return CreatedAtAction(nameof(GetById), new { id = ((dynamic)data).Id }, data);
        }

        // ==========================================
        // UPDATE POST (requires owner or superuser)
        // ==========================================
        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] PostUpdateDTO dto, CancellationToken ct)
        {
            var email = User.FindFirstValue(ClaimTypes.Email)
                        ?? User.FindFirst("email")?.Value
                        ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                _sentryLogger.Warn("Missing email claim on post update");
                return Unauthorized(new { message = "Missing email claim." });
            }
            
            _sentryLogger.Info("Starting post update", $"User: {email}, PostId: {id}");

            var (ok, err, data) = await _service.UpdateAsync(email, id, dto, ct);
            if (!ok)
            {
                _sentryLogger.Warn("Post update failed", $"User: {email}, PostId: {id}, Reason: {err}");
                return BadRequest(new { message = err });
            }

            _sentryLogger.Success("Post updated successfully", $"User: {email}, PostId: {id}");
            return Ok(data);
        }

        // ==========================================
        // DELETE POST (requires owner or superuser)
        // ==========================================
        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var email = User.FindFirstValue(ClaimTypes.Email)
                        ?? User.FindFirst("email")?.Value
                        ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                _sentryLogger.Warn("Missing email claim on post deletion");
                return Unauthorized(new { message = "Missing email claim." });
            }
            
            _sentryLogger.Info("Starting post deletion", $"User: {email}, PostId: {id}");

            var (ok, err) = await _service.DeleteAsync(email, id, ct);
            if (!ok)
            {
                _sentryLogger.Warn("Post deletion failed", $"User: {email}, PostId: {id}, Reason: {err}");
                return BadRequest(new { message = err });
            }

            _sentryLogger.Success("Post deleted successfully", $"User: {email}, PostId: {id}");
            return NoContent();
        }

        // ==========================================
        // GET ALL POSTS (public) only for superusers
        // ==========================================
        [Authorize(Roles = "super_user")]
        [HttpGet] // GET /api/posts
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var email = User.FindFirstValue(ClaimTypes.Email)
                        ?? User.FindFirst("email")?.Value
                        ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                _sentryLogger.Warn("Missing email claim on get all posts");
                return Unauthorized(new { message = "Missing email claim." });
            }
            
            _sentryLogger.Info("Super user fetching all posts", $"User: {email}");

            var (ok, err, data) = await _service.GetAllAsync(ct);
            if (!ok)
            {
                _sentryLogger.Warn("Failed to fetch all posts", $"User: {email}, Reason: {err}");
                return BadRequest(new { message = err });
            }

            _sentryLogger.Success("Fetched all posts", $"User: {email}, Count: {data?.Count()}");
            return Ok(data);
        }

        // ==========================================
        // GET POSTS BY LOGGED-IN USER
        // ==========================================
        [Authorize]
        [HttpGet("user")] // GET /api/posts/user
        public async Task<IActionResult> GetUserPosts(CancellationToken ct)
        {
            var email = User.FindFirstValue(ClaimTypes.Email)
                        ?? User.FindFirst("email")?.Value
                        ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                _sentryLogger.Warn("Missing email claim on get user posts");
                return Unauthorized(new { message = "Missing email claim." });
            }
            
            _sentryLogger.Info("Fetching user posts", $"User: {email}");

            var (ok, err, data) = await _service.GetByUserAsync(email, ct);
            if (!ok)
            {
                _sentryLogger.Warn("Failed to fetch user posts", $"User: {email}, Reason: {err}");
                return BadRequest(new { message = err });
            }
            
            _sentryLogger.Success("Fetched user posts", $"User: {email}, Count: {data?.Count()}");
            return Ok(data);
        }

        // ==========================================
        // GET POST BY ID
        // ==========================================
        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public IActionResult GetById([FromRoute] int id)
        {
            // (optional stub for CreatedAtAction)
            return Ok(new { id });
        }
    }
}