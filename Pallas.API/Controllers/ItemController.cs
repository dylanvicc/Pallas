using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pallas.API.Models.Items;
using Pallas.API.Services.Items;
using System.Security.Claims;

namespace Pallas.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/items")]
    public class ItemController(IItemService items) : ControllerBase
    {
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetItem(long id)
        {
            var item = await items.GetItemByIdAsync(id);

            if (item == null)
                return NotFound();

            return Ok(MapToResponse(item));
        }

        [HttpGet]
        public async Task<IActionResult> GetItems([FromQuery] ItemQuery query)
        {
            return Ok(await items.GetItemsAsync(query));
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody] ItemCreateRequest request)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            if (!long.TryParse(User.FindFirst(ClaimTypes.PrimarySid)?.Value, out var userId))
                return Unauthorized("Invalid or missing user identifier in token.");

            var created = await items.CreateItemAsync(request, userId);

            return CreatedAtAction(nameof(GetItem), new { id = created.Id }, MapToResponse(created));
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateItem(long id, [FromBody] ItemUpdateRequest request)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            if (!long.TryParse(User.FindFirst(ClaimTypes.PrimarySid)?.Value, out var userId))
                return Unauthorized("Invalid or missing user identifier in token.");

            var updated = await items.UpdateItemAsync(id, request, userId);

            if (updated == null)
                return NotFound();

            return Ok(MapToResponse(updated));
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteItem(long id)
        {
            var deleted = await items.DeleteItemAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }

        private static ItemResponse MapToResponse(Item item) => new()
        {
            Id = item.Id,
            Sku = item.Sku,
            Name = item.Name,
            Description = item.Description,
            Status = item.Status,
            Manufacturer = item.Manufacturer,
            BarcodePrimary = item.BarcodePrimary,
            CreatedAt = item.CreatedAt,
            CreatedBy = item.CreatedBy,
            UpdatedAt = item.UpdatedAt,
            UpdatedBy = item.UpdatedBy
        };
    }
}
