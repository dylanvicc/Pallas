using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pallas.API.Models.Inventory;
using Pallas.API.Services.Inventory;
using System.Security.Claims;

namespace Pallas.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/inventory")]
    public class InventoryLevelController(IInventoryLevelService inventory) : ControllerBase
    {
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetInventoryLevel(long id)
        {
            var level = await inventory.GetInventoryLevelByIdAsync(id);

            if (level == null)
                return NotFound();

            return Ok(MapToResponse(level));
        }

        [HttpGet]
        public async Task<IActionResult> GetInventoryLevels([FromQuery] InventoryLevelQuery query)
        {
            return Ok(await inventory.GetInventoryLevelsAsync(query));
        }

        [HttpPost]
        public async Task<IActionResult> CreateInventoryLevel([FromBody] InventoryLevelCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!long.TryParse(User.FindFirst(ClaimTypes.PrimarySid)?.Value, out var userId))
                return Unauthorized("Invalid or missing user identifier in token.");

            try
            {
                var created = await inventory.CreateInventoryLevelAsync(request, userId);

                return CreatedAtAction(nameof(GetInventoryLevel), new { id = created.Id }, MapToResponse(created));
            }
            catch (InvalidOperationException exception)
            {
                return Conflict(exception.Message);
            }
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateInventoryLevel(long id, [FromBody] InventoryLevelUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!long.TryParse(User.FindFirst(ClaimTypes.PrimarySid)?.Value, out var userId))
                return Unauthorized("Invalid or missing user identifier in token.");

            var updated = await inventory.UpdateInventoryLevelAsync(id, request, userId);

            if (updated == null)
                return NotFound();

            return Ok(MapToResponse(updated));
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteInventoryLevel(long id)
        {
            var deleted = await inventory.DeleteInventoryLevelAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }

        private static InventoryLevelResponse MapToResponse(InventoryLevel level) => new()
        {
            Id = level.Id,
            ItemId = level.ItemId,
            LocationId = level.LocationId,
            Quantity = level.Quantity,
            ReorderPoint = level.ReorderPoint,
            ReorderQty = level.ReorderQty,
            CreatedAt = level.CreatedAt,
            CreatedBy = level.CreatedBy,
            UpdatedAt = level.UpdatedAt,
            UpdatedBy = level.UpdatedBy
        };
    }
}