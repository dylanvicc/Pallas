using Microsoft.EntityFrameworkCore;
using Pallas.API.Models;
using Pallas.API.Models.Locations;

namespace Pallas.API.Services.Locations
{
    public class LocationService(
        ApplicationDatabaseContext context,
        ILogger<LocationService> logger) : ILocationService
    {
        public async Task<Location?> GetLocationByIdAsync(long id) =>
            await context.Locations.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);

        public async Task<PagedResult<Location>> GetLocationsAsync(LocationQuery query)
        {
            var queryable = context.Locations.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
                queryable = queryable.Where(l => l.Name.Contains(query.Search));

            if (query.Active.HasValue)
                queryable = queryable.Where(l => l.Active == query.Active.Value);

            var count = await queryable.CountAsync();

            var data = await queryable.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync();

            return new PagedResult<Location>
            {
                Data = data,
                TotalCount = count,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<Location> CreateLocationAsync(LocationCreateRequest request, long userId)
        {
            try
            {
                var location = new Location
                {
                    Name = request.Name,
                    Description = request.Description,
                    Active = request.Active,
                    CreatedBy = userId,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedBy = userId,
                    UpdatedAt = DateTimeOffset.UtcNow
                };

                await context.Locations.AddAsync(location);
                await context.SaveChangesAsync();

                logger.LogInformation("Created new location {Name}.", location.Name);

                return location;
            }
            catch (DbUpdateException exception)
            {
                logger.LogError(exception, "Database update failed while creating location {Name}.", request.Name);
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unexpected error while creating location {Name}.", request.Name);
                throw;
            }
        }

        public async Task<Location?> UpdateLocationAsync(long id, LocationUpdateRequest request, long userId)
        {
            try
            {
                var location = await context.Locations.FirstOrDefaultAsync(l => l.Id == id);

                if (location == null)
                    return null;

                if (request.Name != null) location.Name = request.Name;
                if (request.Description != null) location.Description = request.Description;
                if (request.Active.HasValue) location.Active = request.Active.Value;

                location.UpdatedAt = DateTimeOffset.UtcNow;
                location.UpdatedBy = userId;

                await context.SaveChangesAsync();

                logger.LogInformation("Updated location {Id}.", id);

                return location;
            }
            catch (DbUpdateException exception)
            {
                logger.LogError(exception, "Database update failed while updating location {Id}.", id);
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unexpected error while updating location {Id}.", id);
                throw;
            }
        }

        public async Task<bool> DeleteLocationAsync(long id)
        {
            try
            {
                var location = await context.Locations.FirstOrDefaultAsync(l => l.Id == id);

                if (location == null)
                    return false;

                context.Locations.Remove(location);
                await context.SaveChangesAsync();

                logger.LogInformation("Deleted location {Id}.", id);

                return true;
            }
            catch (DbUpdateException exception)
            {
                logger.LogError(exception, "Database update failed while deleting location {Id}.", id);
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unexpected error while deleting location {Id}.", id);
                throw;
            }
        }
    }
}