using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pallas.API.Models.Users;
using Pallas.API.Services.Users;
using System.Security.Claims;

namespace Pallas.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UserController(IUserService users) : ControllerBase
    {
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetUser(long id)
        {
            var user = await users.GetUserAsync(id);

            if (user == null)
                return NotFound();

            return Ok(MapToResponse(user));
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await users.GetUserByEmailAsync(email);

            if (user == null)
                return NotFound();

            return Ok(MapToResponse(user));
        }

        [HttpGet("username/{username}")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var user = await users.GetUserByUsernameAsync(username);

            if (user == null)
                return NotFound();

            return Ok(MapToResponse(user));
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await users.GetUserByEmailAsync(request.Email) != null)
                return Conflict("This email already in use.");

            if (await users.GetUserByUsernameAsync(request.Username) != null)
                return Conflict("This username already in use.");

            var created = await users.CreateUserAsync(request);

            if (created == null)
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create user.");

            return CreatedAtAction(nameof(GetUser), new { id = created.Id }, MapToResponse(created));
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateUser(long id, [FromBody] UserUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!long.TryParse(User.FindFirst(ClaimTypes.PrimarySid)?.Value, out var sid))
                return Unauthorized("Invalid or missing user identifier in token.");

            if (sid != id)
                return StatusCode(StatusCodes.Status403Forbidden, "You can only update your own account.");

            var updated = await users.UpdateUserAsync(id, request);

            if (updated == null)
                return NotFound();

            return Ok(MapToResponse(updated));
        }

        private static UserResponse MapToResponse(User user) => new()
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            LastLogin = user.LastLogin
        };
    }
}