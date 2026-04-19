using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Pallas.API.Models.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Pallas.API.Services.Authentication
{
    public class JWTAuthenticationService(IConfiguration configuration) : IJWTAuthenticationService
    {
        public ClaimsIdentity GenerateClaims(User user)
        {
            var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            identity.AddClaim(new Claim(ClaimTypes.PrimarySid, user.Id.ToString()));
            return identity;
        }

        public string GenerateRefreshToken()
        {
            var token = new byte[32];

            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(token);

            return Convert.ToBase64String(token);
        }

        public string GenerateToken(User user)
        {
            var handler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(configuration.GetValue<string>("Authentication:Key") ?? string.Empty);

            if (key.Length == 0)
                return string.Empty;

            var descriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = configuration.GetValue<string>("Authentication:Issuer") ?? string.Empty,
                Subject = GenerateClaims(user)
            };

            return handler.WriteToken(handler.CreateToken(descriptor));
        }
    }
}
