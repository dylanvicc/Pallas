using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pallas.API.Models.Locations;
using Pallas.API.Services.Locations;
using System.Security.Claims;

namespace Pallas.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/locations")]
    public class LocationController(ILocationService locations) : ControllerBase
    {
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetLocation(long id)
        {
            var location = await locations.GetLocationByIdAsync(id);

            if (location == null)
                return NotFound();

            return Ok(MapToResponse(location));
        }

        [HttpGet]
        public async Task<IActionResult> GetLocations([FromQuery] LocationQuery query)
        {
            return Ok(await locations.GetLocationsAsync(query));
        }

        [HttpPost]
        public async Task<IActionResult> CreateLocation([FromBody] LocationCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!long.TryParse(User.FindFirst(ClaimTypes.PrimarySid)?.Value, out var userId))
                return Unauthorized("Invalid or missing user identifier in token.");

            var created = await locations.CreateLocationAsync(request, userId);

            return CreatedAtAction(nameof(GetLocation), new { id = created.Id }, MapToResponse(created));
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateLocation(long id, [FromBody] LocationUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!long.TryParse(User.FindFirst(ClaimTypes.PrimarySid)?.Value, out var userId))
                return Unauthorized("Invalid or missing user identifier in token.");

            var updated = await locations.UpdateLocationAsync(id, request, userId);

            if (updated == null)
                return NotFound();

            return Ok(MapToResponse(updated));
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteLocation(long id)
        {
            var deleted = await locations.DeleteLocationAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }

        private static LocationResponse MapToResponse(Location location) => new()
        {
            Id = location.Id,
            Name = location.Name,
            Description = location.Description,
            Active = location.Active,
            CreatedAt = location.CreatedAt,
            CreatedBy = location.CreatedBy,
            UpdatedAt = location.UpdatedAt,
            UpdatedBy = location.UpdatedBy
        };
    }
}