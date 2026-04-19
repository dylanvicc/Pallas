using Microsoft.EntityFrameworkCore;
using Pallas.API.Models;
using Pallas.API.Models.Inventory;

namespace Pallas.API.Services.Inventory
{
    public class InventoryLevelService(
        ApplicationDatabaseContext context,
        ILogger<InventoryLevelService> logger) : IInventoryLevelService
    {
        public async Task<InventoryLevel?> GetInventoryLevelByIdAsync(long id) =>
            await context.InventoryLevels.AsNoTracking().FirstOrDefaultAsync(il => il.Id == id);

        public async Task<PagedResult<InventoryLevel>> GetInventoryLevelsAsync(InventoryLevelQuery query)
        {
            var queryable = context.InventoryLevels.AsNoTracking().AsQueryable();

            if (query.ItemId.HasValue)
                queryable = queryable.Where(il => il.ItemId == query.ItemId.Value);

            if (query.LocationId.HasValue)
                queryable = queryable.Where(il => il.LocationId == query.LocationId.Value);

            if (query.BelowReorderPoint == true)
                queryable = queryable.Where(il => il.ReorderPoint.HasValue && il.Quantity < il.ReorderPoint.Value);

            var count = await queryable.CountAsync();

            var data = await queryable.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync();

            return new PagedResult<InventoryLevel>
            {
                Data = data,
                TotalCount = count,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<InventoryLevel> CreateInventoryLevelAsync(InventoryLevelCreateRequest request, long userId)
        {
            try
            {
                var exists = await context.InventoryLevels
                    .AnyAsync(il => il.ItemId == request.ItemId && il.LocationId == request.LocationId);

                if (exists)
                    throw new InvalidOperationException($"An inventory level for item {request.ItemId} at location {request.LocationId} already exists.");

                var level = new InventoryLevel
                {
                    ItemId = request.ItemId,
                    LocationId = request.LocationId,
                    Quantity = request.Quantity,
                    ReorderPoint = request.ReorderPoint,
                    ReorderQty = request.ReorderQty,
                    CreatedBy = userId,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedBy = userId,
                    UpdatedAt = DateTimeOffset.UtcNow
                };

                await context.InventoryLevels.AddAsync(level);
                await context.SaveChangesAsync();

                logger.LogInformation("Created inventory level for item {ItemId} at location {LocationId}.", level.ItemId, level.LocationId);

                return level;
            }
            catch (DbUpdateException exception)
            {
                logger.LogError(exception, "Database update failed while creating inventory level for item {ItemId}.", request.ItemId);
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unexpected error while creating inventory level for item {ItemId}.", request.ItemId);
                throw;
            }
        }

        public async Task<InventoryLevel?> UpdateInventoryLevelAsync(long id, InventoryLevelUpdateRequest request, long userId)
        {
            try
            {
                var level = await context.InventoryLevels.FirstOrDefaultAsync(il => il.Id == id);

                if (level == null)
                    return null;

                if (request.Quantity.HasValue) level.Quantity = request.Quantity.Value;
                if (request.ReorderPoint.HasValue) level.ReorderPoint = request.ReorderPoint.Value;
                if (request.ReorderQty.HasValue) level.ReorderQty = request.ReorderQty.Value;

                level.UpdatedAt = DateTimeOffset.UtcNow;
                level.UpdatedBy = userId;

                await context.SaveChangesAsync();

                logger.LogInformation("Updated inventory level {Id}.", id);

                return level;
            }
            catch (DbUpdateException exception)
            {
                logger.LogError(exception, "Database update failed while updating inventory level {Id}.", id);
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unexpected error while updating inventory level {Id}.", id);
                throw;
            }
        }

        public async Task<bool> DeleteInventoryLevelAsync(long id)
        {
            try
            {
                var level = await context.InventoryLevels.FirstOrDefaultAsync(il => il.Id == id);

                if (level == null)
                    return false;

                context.InventoryLevels.Remove(level);
                await context.SaveChangesAsync();

                logger.LogInformation("Deleted inventory level {Id}.", id);

                return true;
            }
            catch (DbUpdateException exception)
            {
                logger.LogError(exception, "Database update failed while deleting inventory level {Id}.", id);
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unexpected error while deleting inventory level {Id}.", id);
                throw;
            }
        }
    }
}