using Pallas.API.Models;
using Pallas.API.Models.Inventory;

namespace Pallas.API.Services.Inventory
{
    public interface IInventoryLevelService
    {
        Task<InventoryLevel?> GetInventoryLevelByIdAsync(long id);

        Task<PagedResult<InventoryLevel>> GetInventoryLevelsAsync(InventoryLevelQuery query);

        Task<InventoryLevel> CreateInventoryLevelAsync(InventoryLevelCreateRequest request, long userId);

        Task<InventoryLevel?> UpdateInventoryLevelAsync(long id, InventoryLevelUpdateRequest request, long userId);

        Task<bool> DeleteInventoryLevelAsync(long id);
    }
}