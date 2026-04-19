using System.ComponentModel.DataAnnotations;

namespace Pallas.API.Models.Inventory
{
    public class InventoryLevelUpdateRequest
    {
        [Range(0, int.MaxValue)]
        public int? Quantity { get; set; }

        [Range(0, int.MaxValue)]
        public int? ReorderPoint { get; set; }

        [Range(0, int.MaxValue)]
        public int? ReorderQty { get; set; }
    }
}