namespace Pallas.API.Models.Items
{
    public class ItemQuery
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 25;

        public string? Search { get; set; }

        public ItemStatus? Status { get; set; }
    }
}