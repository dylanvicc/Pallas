using System.ComponentModel.DataAnnotations;

namespace Pallas.API.Models.Locations
{
    public class LocationCreateRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public bool Active { get; set; } = true;
    }
}