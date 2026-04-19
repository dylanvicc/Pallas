namespace Pallas.API.Models.Items
{
    public class ItemUpdateRequest
    {
        public string? Sku { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public ItemStatus? Status { get; set; }

        public string? Manufacturer { get; set; }

        public string? BarcodePrimary { get; set; }
    }
}