using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Pallas.API;
using Pallas.API.Models.Inventory;
using Pallas.API.Models.Items;
using Pallas.API.Models.Locations;
using Pallas.API.Services.Inventory;

namespace Pallas.Test.Services
{
    [TestClass]
    public class InventoryLevelServiceTests
    {
        private InventoryLevelService _service = null!;
        private ApplicationDatabaseContext _context = null!;
        private Mock<ILogger<InventoryLevelService>> _mockLogger = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDatabaseContext>()
                .UseInMemoryDatabase(databaseName: $"test_db_{Guid.NewGuid()}")
                .Options;

            _context = new ApplicationDatabaseContext(options);
            _mockLogger = new Mock<ILogger<InventoryLevelService>>();
            _service = new InventoryLevelService(_context, _mockLogger.Object);
        }

        [TestCleanup]
        public void Cleanup() => _context?.Dispose();

        [TestMethod]
        public async Task GetInventoryLevelByIdAsync_ExistingId_ReturnsInventoryLevel()
        {
            var (_, _, level) = await SeedInventoryLevelAsync();

            var result = await _service.GetInventoryLevelByIdAsync(level.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(level.Id, result.Id);
            Assert.AreEqual(level.Quantity, result.Quantity);
        }

        [TestMethod]
        public async Task GetInventoryLevelByIdAsync_NonExistentId_ReturnsNull()
        {
            var result = await _service.GetInventoryLevelByIdAsync(999);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetInventoryLevelsAsync_NoFilters_ReturnsAllLevels()
        {
            await SeedInventoryLevelAsync();
            await SeedInventoryLevelAsync();
            await SeedInventoryLevelAsync();

            var result = await _service.GetInventoryLevelsAsync(new InventoryLevelQuery());

            Assert.AreEqual(3, result.TotalCount);
            Assert.AreEqual(3, result.Data.Count());
        }

        [TestMethod]
        public async Task GetInventoryLevelsAsync_EmptyDatabase_ReturnsEmpty()
        {
            var result = await _service.GetInventoryLevelsAsync(new InventoryLevelQuery());

            Assert.AreEqual(0, result.TotalCount);
            Assert.AreEqual(0, result.Data.Count());
        }

        [TestMethod]
        public async Task GetInventoryLevelsAsync_FilterByItemId_ReturnsMatchingLevels()
        {
            var (item1, _, _) = await SeedInventoryLevelAsync();
            var (item2, _, _) = await SeedInventoryLevelAsync();

            var result = await _service.GetInventoryLevelsAsync(new InventoryLevelQuery { ItemId = item1.Id });

            Assert.AreEqual(1, result.TotalCount);
            Assert.IsTrue(result.Data.All(il => il.ItemId == item1.Id));
        }

        [TestMethod]
        public async Task GetInventoryLevelsAsync_FilterByLocationId_ReturnsMatchingLevels()
        {
            var (_, location1, _) = await SeedInventoryLevelAsync();
            var (_, location2, _) = await SeedInventoryLevelAsync();

            var result = await _service.GetInventoryLevelsAsync(new InventoryLevelQuery { LocationId = location1.Id });

            Assert.AreEqual(1, result.TotalCount);
            Assert.IsTrue(result.Data.All(il => il.LocationId == location1.Id));
        }

        [TestMethod]
        public async Task GetInventoryLevelsAsync_FilterBelowReorderPoint_ReturnsMatchingLevels()
        {
            var (item1, location1, _) = await SeedInventoryLevelAsync(quantity: 5, reorderPoint: 10);
            var (item2, location2, _) = await SeedInventoryLevelAsync(quantity: 20, reorderPoint: 10);
            var (item3, location3, _) = await SeedInventoryLevelAsync(quantity: 3, reorderPoint: 15);

            var result = await _service.GetInventoryLevelsAsync(new InventoryLevelQuery { BelowReorderPoint = true });

            Assert.AreEqual(2, result.TotalCount);
            Assert.IsTrue(result.Data.All(il => il.Quantity < il.ReorderPoint));
        }

        [TestMethod]
        public async Task GetInventoryLevelsAsync_BelowReorderPoint_IgnoresLevelsWithNoReorderPoint()
        {
            await SeedInventoryLevelAsync(quantity: 5, reorderPoint: null);

            var result = await _service.GetInventoryLevelsAsync(new InventoryLevelQuery { BelowReorderPoint = true });

            Assert.AreEqual(0, result.TotalCount);
        }

        [TestMethod]
        public async Task GetInventoryLevelsAsync_Pagination_ReturnsCorrectPage()
        {
            for (var i = 0; i < 10; i++)
                await SeedInventoryLevelAsync();

            var result = await _service.GetInventoryLevelsAsync(new InventoryLevelQuery { Page = 2, PageSize = 3 });

            Assert.AreEqual(10, result.TotalCount);
            Assert.AreEqual(3, result.Data.Count());
            Assert.AreEqual(2, result.Page);
            Assert.AreEqual(3, result.PageSize);
        }

        [TestMethod]
        public async Task CreateInventoryLevelAsync_ValidRequest_ReturnsCreatedLevel()
        {
            var item = await SeedItemAsync();
            var location = await SeedLocationAsync();

            var result = await _service.CreateInventoryLevelAsync(BuildCreateRequest(item.Id, location.Id), 1);

            Assert.IsNotNull(result);
            Assert.AreNotEqual(0, result.Id);
            Assert.AreEqual(item.Id, result.ItemId);
            Assert.AreEqual(location.Id, result.LocationId);
        }

        [TestMethod]
        public async Task CreateInventoryLevelAsync_ValidRequest_PersistsToDatabase()
        {
            var item = await SeedItemAsync();
            var location = await SeedLocationAsync();

            var result = await _service.CreateInventoryLevelAsync(BuildCreateRequest(item.Id, location.Id), 1);

            var persisted = await _context.InventoryLevels.FindAsync(result.Id);
            Assert.IsNotNull(persisted);
            Assert.AreEqual(item.Id, persisted.ItemId);
        }

        [TestMethod]
        public async Task CreateInventoryLevelAsync_SetsCreatedByAndUpdatedBy()
        {
            var item = await SeedItemAsync();
            var location = await SeedLocationAsync();

            var result = await _service.CreateInventoryLevelAsync(BuildCreateRequest(item.Id, location.Id), 42);

            Assert.AreEqual(42, result.CreatedBy);
            Assert.AreEqual(42, result.UpdatedBy);
        }

        [TestMethod]
        public async Task CreateInventoryLevelAsync_SetsAllFields()
        {
            var item = await SeedItemAsync();
            var location = await SeedLocationAsync();

            var result = await _service.CreateInventoryLevelAsync(BuildCreateRequest(
                item.Id, location.Id, quantity: 50, reorderPoint: 10, reorderQty: 25), 1);

            Assert.AreEqual(50, result.Quantity);
            Assert.AreEqual(10, result.ReorderPoint);
            Assert.AreEqual(25, result.ReorderQty);
        }

        [TestMethod]
        public async Task CreateInventoryLevelAsync_DuplicateItemAndLocation_ThrowsInvalidOperationException()
        {
            var (item, location, _) = await SeedInventoryLevelAsync();

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.CreateInventoryLevelAsync(BuildCreateRequest(item.Id, location.Id), 1));
        }

        [TestMethod]
        public async Task CreateInventoryLevelAsync_SameItemDifferentLocation_Succeeds()
        {
            var (item, _, _) = await SeedInventoryLevelAsync();
            var newLocation = await SeedLocationAsync(name: "New Location");

            var result = await _service.CreateInventoryLevelAsync(BuildCreateRequest(item.Id, newLocation.Id), 1);

            Assert.IsNotNull(result);
            Assert.AreEqual(item.Id, result.ItemId);
            Assert.AreEqual(newLocation.Id, result.LocationId);
        }

        [TestMethod]
        public async Task UpdateInventoryLevelAsync_ExistingLevel_ReturnsUpdatedLevel()
        {
            var (_, _, level) = await SeedInventoryLevelAsync(quantity: 10);

            var result = await _service.UpdateInventoryLevelAsync(level.Id, new InventoryLevelUpdateRequest
            {
                Quantity = 50
            }, 1);

            Assert.IsNotNull(result);
            Assert.AreEqual(50, result.Quantity);
        }

        [TestMethod]
        public async Task UpdateInventoryLevelAsync_NonExistentId_ReturnsNull()
        {
            var result = await _service.UpdateInventoryLevelAsync(999, new InventoryLevelUpdateRequest
            {
                Quantity = 50
            }, 1);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UpdateInventoryLevelAsync_NullFields_DoesNotOverwriteExistingValues()
        {
            var (_, _, level) = await SeedInventoryLevelAsync(quantity: 10, reorderPoint: 5, reorderQty: 20);

            var result = await _service.UpdateInventoryLevelAsync(level.Id, new InventoryLevelUpdateRequest
            {
                Quantity = 99
            }, 1);

            Assert.IsNotNull(result);
            Assert.AreEqual(99, result.Quantity);
            Assert.AreEqual(5, result.ReorderPoint);
            Assert.AreEqual(20, result.ReorderQty);
        }

        [TestMethod]
        public async Task UpdateInventoryLevelAsync_SetsUpdatedByAndUpdatedAt()
        {
            var (_, _, level) = await SeedInventoryLevelAsync();
            var before = DateTimeOffset.UtcNow;

            var result = await _service.UpdateInventoryLevelAsync(level.Id, new InventoryLevelUpdateRequest
            {
                Quantity = 5
            }, 42);

            Assert.IsNotNull(result);
            Assert.AreEqual(42, result.UpdatedBy);
            Assert.IsTrue(result.UpdatedAt >= before);
        }

        [TestMethod]
        public async Task UpdateInventoryLevelAsync_PersistsChangesToDatabase()
        {
            var (_, _, level) = await SeedInventoryLevelAsync(quantity: 10);

            await _service.UpdateInventoryLevelAsync(level.Id, new InventoryLevelUpdateRequest
            {
                Quantity = 99
            }, 1);

            var persisted = await _context.InventoryLevels.FindAsync(level.Id);
            Assert.AreEqual(99, persisted!.Quantity);
        }

        [TestMethod]
        public async Task DeleteInventoryLevelAsync_ExistingLevel_ReturnsTrue()
        {
            var (_, _, level) = await SeedInventoryLevelAsync();

            var result = await _service.DeleteInventoryLevelAsync(level.Id);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task DeleteInventoryLevelAsync_NonExistentId_ReturnsFalse()
        {
            var result = await _service.DeleteInventoryLevelAsync(999);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task DeleteInventoryLevelAsync_ExistingLevel_RemovesFromDatabase()
        {
            var (_, _, level) = await SeedInventoryLevelAsync();

            await _service.DeleteInventoryLevelAsync(level.Id);

            var persisted = await _context.InventoryLevels.FindAsync(level.Id);
            Assert.IsNull(persisted);
        }

        [TestMethod]
        public async Task DeleteInventoryLevelAsync_OnlyDeletesTargetLevel()
        {
            var (_, _, level1) = await SeedInventoryLevelAsync();
            var (_, _, level2) = await SeedInventoryLevelAsync();

            await _service.DeleteInventoryLevelAsync(level1.Id);

            var remaining = await _context.InventoryLevels.FindAsync(level2.Id);
            Assert.IsNotNull(remaining);
        }

        private async Task<(Item, Location, InventoryLevel)> SeedInventoryLevelAsync(
            int quantity = 0,
            int? reorderPoint = null,
            int? reorderQty = null)
        {
            var item = await SeedItemAsync();
            var location = await SeedLocationAsync();

            var level = new InventoryLevel
            {
                ItemId = item.Id,
                LocationId = location.Id,
                Quantity = quantity,
                ReorderPoint = reorderPoint,
                ReorderQty = reorderQty,
                CreatedBy = 1,
                UpdatedBy = 1
            };

            _context.InventoryLevels.Add(level);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            return (item, location, level);
        }

        private async Task<Item> SeedItemAsync()
        {
            var item = new Item
            {
                Sku = $"SKU-{Guid.NewGuid()}",
                Name = "Test Item",
                Status = ItemStatus.Active,
                CreatedBy = 1,
                UpdatedBy = 1
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            return item;
        }

        private async Task<Location> SeedLocationAsync(string name = "Test Location")
        {
            var location = new Location
            {
                Name = $"{name} {Guid.NewGuid()}",
                Active = true,
                CreatedBy = 1,
                UpdatedBy = 1
            };

            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            return location;
        }

        private static InventoryLevelCreateRequest BuildCreateRequest(
            long itemId = 1,
            long locationId = 1,
            int quantity = 0,
            int? reorderPoint = null,
            int? reorderQty = null) => new()
            {
                ItemId = itemId,
                LocationId = locationId,
                Quantity = quantity,
                ReorderPoint = reorderPoint,
                ReorderQty = reorderQty
            };
    }
}