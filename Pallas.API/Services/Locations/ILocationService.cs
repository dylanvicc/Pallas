using Pallas.API.Models;
using Pallas.API.Models.Locations;

namespace Pallas.API.Services.Locations
{
    public interface ILocationService
    {
        Task<Location?> GetLocationByIdAsync(long id);

        Task<PagedResult<Location>> GetLocationsAsync(LocationQuery query);

        Task<Location> CreateLocationAsync(LocationCreateRequest request, long userId);

        Task<Location?> UpdateLocationAsync(long id, LocationUpdateRequest request, long userId);

        Task<bool> DeleteLocationAsync(long id);
    }
}