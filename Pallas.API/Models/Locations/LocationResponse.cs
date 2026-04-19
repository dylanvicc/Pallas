namespace Pallas.API.Models.Locations
{
    public class LocationResponse
    {
        public long Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public bool Active { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public long? CreatedBy { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public long? UpdatedBy { get; set; }
    }
}