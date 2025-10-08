using Microsoft.AspNetCore.Mvc;
using Redit_api.Models.DTO;
using Redit_api.Services;
using Redit_api.Services.Interfaces;

namespace Redit_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
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
    }
}