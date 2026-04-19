namespace Pallas.API.Models.Locations
{
    public class LocationQuery
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 25;

        public string? Search { get; set; }

        public bool? Active { get; set; }
    }
}