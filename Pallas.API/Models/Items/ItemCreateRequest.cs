using System.ComponentModel.DataAnnotations;

namespace Pallas.API.Models.Items
{
    public class ItemCreateRequest
    {
        [Required]
        public string Sku { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public ItemStatus Status { get; set; } = ItemStatus.Active;

        public string? Manufacturer { get; set; }

        public string? BarcodePrimary { get; set; }
    }
}
