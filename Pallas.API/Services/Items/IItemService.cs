using Pallas.API.Models;
using Pallas.API.Models.Items;

namespace Pallas.API.Services.Items
{
    public interface IItemService
    {
        Task<Item?> GetItemByIdAsync(long id);

        Task<PagedResult<Item>> GetItemsAsync(ItemQuery query);

        Task<Item> CreateItemAsync(ItemCreateRequest request, long userId);

        Task<Item?> UpdateItemAsync(long id, ItemUpdateRequest request, long userId);

        Task<bool> DeleteItemAsync(long id);
    }
}