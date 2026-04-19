namespace Pallas.API.Models.Items
{
    public class ItemResponse
    {
        public long Id { get; set; }

        public string Sku { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public ItemStatus Status { get; set; }

        public string? Manufacturer { get; set; }

        public string? BarcodePrimary { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public long? CreatedBy { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public long? UpdatedBy { get; set; }
    }
}
