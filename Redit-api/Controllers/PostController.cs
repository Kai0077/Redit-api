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
        private readonly IPostService _service;

        public PostsController(IPostService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PostCreateDTO dto, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var email = User.FindFirstValue(ClaimTypes.Email)
                        ?? User.FindFirst("email")?.Value
                        ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(email))
                return Unauthorized(new { message = "Missing email claim." });

            var (ok, err, data) = await _service.CreateAsync(email, dto, ct);
            if (!ok) return BadRequest(new { message = err });

            return CreatedAtAction(nameof(GetById), new { id = ((dynamic)data).Id }, data);
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] PostUpdateDTO dto, CancellationToken ct)
        {
            var email = User.FindFirstValue(ClaimTypes.Email)
                        ?? User.FindFirst("email")?.Value
                        ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(email))
                return Unauthorized(new { message = "Missing email claim." });

            var (ok, err, data) = await _service.UpdateAsync(email, id, dto, ct);
            if (!ok) return BadRequest(new { message = err });

            return Ok(data);
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var email = User.FindFirstValue(ClaimTypes.Email)
                        ?? User.FindFirst("email")?.Value
                        ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(email))
                return Unauthorized(new { message = "Missing email claim." });

            var (ok, err) = await _service.DeleteAsync(email, id, ct);
            if (!ok) return BadRequest(new { message = err });

            return NoContent();
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById([FromRoute] int id)
        {
            // (optional stub for CreatedAtAction)
            return Ok(new { id });
        }
    }
}