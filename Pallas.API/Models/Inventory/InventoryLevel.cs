using Pallas.API.Models.Items;
using Pallas.API.Models.Locations;
using Pallas.API.Models.Users;

namespace Pallas.API.Models.Inventory
{
    public class InventoryLevel
    {
        public long Id { get; set; }

        public long ItemId { get; set; }

        public long LocationId { get; set; }

        public int Quantity { get; set; }

        public int? ReorderPoint { get; set; }

        public int? ReorderQty { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public long? CreatedBy { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public long? UpdatedBy { get; set; }

        public Item? Item { get; set; }

        public Location? Location { get; set; }

        public User? CreatedByUser { get; set; }

        public User? UpdatedByUser { get; set; }
    }
}