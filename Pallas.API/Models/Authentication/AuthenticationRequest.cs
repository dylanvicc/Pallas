using System.ComponentModel.DataAnnotations;

namespace Pallas.API.Models.Authentication
{
    public class AuthenticationRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
