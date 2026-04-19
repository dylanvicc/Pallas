using Pallas.API.Models.Users;

namespace Pallas.API.Models.Locations
{
    public class Location
    {
        public long Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public bool Active { get; set; } = true;

        public DateTimeOffset CreatedAt { get; set; }

        public long? CreatedBy { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public long? UpdatedBy { get; set; }

        public User? CreatedByUser { get; set; }

        public User? UpdatedByUser { get; set; }
    }
}