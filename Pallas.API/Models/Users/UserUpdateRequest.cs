using System.ComponentModel.DataAnnotations;

namespace Pallas.API.Models.Users
{
    public class UserUpdateRequest
    {
        [StringLength(50, MinimumLength = 3)]
        public string? Username { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(128, MinimumLength = 8)]
        public string? Password { get; set; }
    }
}