using Microsoft.EntityFrameworkCore;
using Pallas.API.Models;
using Pallas.API.Models.Items;

namespace Pallas.API.Services.Items
{
    public class ItemService(
        ApplicationDatabaseContext context,
        ILogger<ItemService> logger) : IItemService
    {
        public async Task<Item?> GetItemByIdAsync(long id) =>
            await context.Items.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);

        public async Task<PagedResult<Item>> GetItemsAsync(ItemQuery query)
        {
            var queryable = context.Items.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
                queryable = queryable.Where(i => i.Sku.Contains(query.Search) || i.Name.Contains(query.Search));

            if (query.Status.HasValue)
                queryable = queryable.Where(i => i.Status == query.Status.Value);

            var count = await queryable.CountAsync();

            var data = await queryable.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync();

            return new PagedResult<Item>
            {
                Data = data,
                TotalCount = count,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<Item> CreateItemAsync(ItemCreateRequest request, long userId)
        {
            try
            {
                var item = new Item
                {
                    Sku = request.Sku,
                    Name = request.Name,
                    Description = request.Description,
                    Manufacturer = request.Manufacturer,
                    BarcodePrimary = request.BarcodePrimary,
                    Status = request.Status,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = userId,
                    UpdatedAt = DateTimeOffset.UtcNow
                };

                var created = await context.Items.AddAsync(item);
                await context.SaveChangesAsync();

                logger.LogInformation("Created new item {Sku}.", request.Sku);

                return item;
            }
            catch (DbUpdateException exception)
            {
                logger.LogError(exception, "Database update failed while creating item {Sku}.", request.Sku);
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unexpected error while creating item {Sku}.", request.Sku);
                throw;
            }
        }

        public async Task<Item?> UpdateItemAsync(long id, ItemUpdateRequest request, long userId)
        {
            try
            {
                var item = await context.Items.FirstOrDefaultAsync(i => i.Id == id);

                if (item == null)
                    return null;

                if (request.Sku != null) item.Sku = request.Sku;
                if (request.Name != null) item.Name = request.Name;
                if (request.Status.HasValue) item.Status = request.Status.Value;
                if (request.Description != null) item.Description = request.Description;
                if (request.Manufacturer != null) item.Manufacturer = request.Manufacturer;
                if (request.BarcodePrimary != null) item.BarcodePrimary = request.BarcodePrimary;

                item.UpdatedAt = DateTimeOffset.UtcNow;
                item.UpdatedBy = userId;

                await context.SaveChangesAsync();

                logger.LogInformation("Updated item {Sku}.", item.Sku);

                return item;
            }
            catch (DbUpdateException exception)
            {
                logger.LogError(exception, "Database update failed while updating item {Id}.", id);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while updating item {Id}.", id);
                throw;
            }
        }

        public async Task<bool> DeleteItemAsync(long id)
        {
            try
            {
                var item = await context.Items.FirstOrDefaultAsync(i => i.Id == id);

                if (item == null)
                    return false;

                context.Items.Remove(item);
                await context.SaveChangesAsync();

                logger.LogInformation("Deleted item {Sku}.", item.Sku);

                return true;
            }
            catch (DbUpdateException exception)
            {
                logger.LogError(exception, "Database update failed while deleting item {Id}.", id);
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unexpected error while deleting item {Id}.", id);
                throw;
            }
        }
    }
}