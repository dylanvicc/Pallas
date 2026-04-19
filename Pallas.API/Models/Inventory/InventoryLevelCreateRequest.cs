using System.ComponentModel.DataAnnotations;

namespace Pallas.API.Models.Inventory
{
    public class InventoryLevelCreateRequest
    {
        [Required]
        public long ItemId { get; set; }

        [Required]
        public long LocationId { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; } = 0;

        [Range(0, int.MaxValue)]
        public int? ReorderPoint { get; set; }

        [Range(0, int.MaxValue)]
        public int? ReorderQty { get; set; }
    }
}