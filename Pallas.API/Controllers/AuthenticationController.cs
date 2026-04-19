using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pallas.API.Models.Authentication;
using Pallas.API.Services.Authentication;
using Pallas.API.Services.Hash;
using Pallas.API.Services.Users;

namespace Pallas.API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/authenticate")]
    public class AuthenticationController(
        IUserService users,
        IJWTAuthenticationService authenticator,
        IPasswordHashService hasher) : ControllerBase
    {
        [HttpPost("token")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await users.GetUserByUsernameAsync(request.Username);

            if (user == null)
                return NotFound();

            if (hasher.Verify(request.Password, user.Password))
            {
                var token = authenticator.GenerateToken(user);

                if (string.IsNullOrEmpty(token))
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to generate token.");

                return Ok(new
                {
                    token,
                    username = user.Username,
                    email = user.Email
                });
            }
            else
            {
                return Unauthorized("Invalid username or password.");
            }
        }
    }
}
