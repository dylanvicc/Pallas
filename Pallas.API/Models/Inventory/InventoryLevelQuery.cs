namespace Pallas.API.Models.Inventory
{
    public class InventoryLevelQuery
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 25;

        public long? ItemId { get; set; }

        public long? LocationId { get; set; }

        public bool? BelowReorderPoint { get; set; }
    }
}