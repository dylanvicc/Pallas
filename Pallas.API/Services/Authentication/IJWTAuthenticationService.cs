using Pallas.API.Models.Users;
using System.Security.Claims;

namespace Pallas.API.Services.Authentication
{
    public interface IJWTAuthenticationService
    {
        string GenerateToken(User user);

        string GenerateRefreshToken();

        ClaimsIdentity GenerateClaims(User user);
    }
}
