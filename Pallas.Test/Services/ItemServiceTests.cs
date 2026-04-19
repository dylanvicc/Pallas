using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Pallas.API;
using Pallas.API.Models.Items;
using Pallas.API.Services.Items;

namespace Pallas.Test.Services
{
    [TestClass]
    public class ItemServiceTests
    {
        private ItemService _service = null!;
        private ApplicationDatabaseContext _context = null!;
        private Mock<ILogger<ItemService>> _mockLogger = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDatabaseContext>()
                .UseInMemoryDatabase(databaseName: $"test_db_{Guid.NewGuid()}")
                .Options;

            _context = new ApplicationDatabaseContext(options);
            _mockLogger = new Mock<ILogger<ItemService>>();
            _service = new ItemService(_context, _mockLogger.Object);
        }

        [TestCleanup]
        public void Cleanup() => _context?.Dispose();

        [TestMethod]
        public async Task GetItemByIdAsync_ExistingId_ReturnsItem()
        {
            var item = await SeedItemAsync();

            var result = await _service.GetItemByIdAsync(item.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(item.Id, result.Id);
            Assert.AreEqual(item.Sku, result.Sku);
        }

        [TestMethod]
        public async Task GetItemByIdAsync_NonExistentId_ReturnsNull()
        {
            var result = await _service.GetItemByIdAsync(999);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetItemsAsync_NoFilters_ReturnsAllItems()
        {
            await SeedItemAsync(sku: "SKU-001");
            await SeedItemAsync(sku: "SKU-002");
            await SeedItemAsync(sku: "SKU-003");

            var result = await _service.GetItemsAsync(new ItemQuery());

            Assert.AreEqual(3, result.TotalCount);
            Assert.AreEqual(3, result.Data.Count());
        }

        [TestMethod]
        public async Task GetItemsAsync_EmptyDatabase_ReturnsEmpty()
        {
            var result = await _service.GetItemsAsync(new ItemQuery());

            Assert.AreEqual(0, result.TotalCount);
            Assert.AreEqual(0, result.Data.Count());
        }

        [TestMethod]
        public async Task GetItemsAsync_SearchBySku_ReturnsMatchingItems()
        {
            await SeedItemAsync(sku: "WIDGET-001");
            await SeedItemAsync(sku: "WIDGET-002");
            await SeedItemAsync(sku: "GADGET-001");

            var result = await _service.GetItemsAsync(new ItemQuery { Search = "WIDGET" });

            Assert.AreEqual(2, result.TotalCount);
            Assert.IsTrue(result.Data.All(i => i.Sku.Contains("WIDGET")));
        }

        [TestMethod]
        public async Task GetItemsAsync_SearchByName_ReturnsMatchingItems()
        {
            await SeedItemAsync(sku: "SKU-001", name: "Blue Widget");
            await SeedItemAsync(sku: "SKU-002", name: "Red Widget");
            await SeedItemAsync(sku: "SKU-003", name: "Green Gadget");

            var result = await _service.GetItemsAsync(new ItemQuery { Search = "Widget" });

            Assert.AreEqual(2, result.TotalCount);
            Assert.IsTrue(result.Data.All(i => i.Name.Contains("Widget")));
        }

        [TestMethod]
        public async Task GetItemsAsync_FilterByStatus_ReturnsMatchingItems()
        {
            await SeedItemAsync(sku: "SKU-001", status: ItemStatus.Active);
            await SeedItemAsync(sku: "SKU-002", status: ItemStatus.Active);
            await SeedItemAsync(sku: "SKU-003", status: ItemStatus.Inactive);
            await SeedItemAsync(sku: "SKU-004", status: ItemStatus.Discontinued);

            var result = await _service.GetItemsAsync(new ItemQuery { Status = ItemStatus.Active });

            Assert.AreEqual(2, result.TotalCount);
            Assert.IsTrue(result.Data.All(i => i.Status == ItemStatus.Active));
        }

        [TestMethod]
        public async Task GetItemsAsync_FilterByStatus_NoMatches_ReturnsEmpty()
        {
            await SeedItemAsync(sku: "SKU-001", status: ItemStatus.Active);

            var result = await _service.GetItemsAsync(new ItemQuery { Status = ItemStatus.Hold });

            Assert.AreEqual(0, result.TotalCount);
        }

        [TestMethod]
        public async Task GetItemsAsync_Pagination_ReturnsCorrectPage()
        {
            for (var i = 1; i <= 10; i++)
                await SeedItemAsync(sku: $"SKU-{i:D3}");

            var result = await _service.GetItemsAsync(new ItemQuery { Page = 2, PageSize = 3 });

            Assert.AreEqual(10, result.TotalCount);
            Assert.AreEqual(3, result.Data.Count());
            Assert.AreEqual(2, result.Page);
            Assert.AreEqual(3, result.PageSize);
        }

        [TestMethod]
        public async Task GetItemsAsync_LastPage_ReturnsRemainingItems()
        {
            for (var i = 1; i <= 7; i++)
                await SeedItemAsync(sku: $"SKU-{i:D3}");

            var result = await _service.GetItemsAsync(new ItemQuery { Page = 2, PageSize = 5 });

            Assert.AreEqual(7, result.TotalCount);
            Assert.AreEqual(2, result.Data.Count());
        }

        [TestMethod]
        public async Task UpdateItemAsync_ExistingItem_ReturnsUpdatedItem()
        {
            var item = await SeedItemAsync(name: "Original Name");

            var result = await _service.UpdateItemAsync(item.Id, new ItemUpdateRequest
            {
                Name = "Updated Name"
            }, 1);

            Assert.IsNotNull(result);
            Assert.AreEqual("Updated Name", result.Name);
        }

        [TestMethod]
        public async Task UpdateItemAsync_NonExistentId_ReturnsNull()
        {
            var result = await _service.UpdateItemAsync(999, new ItemUpdateRequest
            {
                Name = "Updated Name"
            }, 1);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UpdateItemAsync_NullFields_DoesNotOverwriteExistingValues()
        {
            var item = await SeedItemAsync(
                name: "Original Name",
                description: "Original Description",
                manufacturer: "Original Manufacturer"
            );

            var result = await _service.UpdateItemAsync(item.Id, new ItemUpdateRequest
            {
                Status = ItemStatus.Inactive
            }, 1);

            Assert.IsNotNull(result);
            Assert.AreEqual("Original Name", result.Name);
            Assert.AreEqual("Original Description", result.Description);
            Assert.AreEqual("Original Manufacturer", result.Manufacturer);
            Assert.AreEqual(ItemStatus.Inactive, result.Status);
        }

        [TestMethod]
        public async Task UpdateItemAsync_StatusToActive_UpdatesCorrectly()
        {
            var item = await SeedItemAsync(status: ItemStatus.Inactive);

            var result = await _service.UpdateItemAsync(item.Id, new ItemUpdateRequest
            {
                Status = ItemStatus.Active
            }, 1);

            Assert.IsNotNull(result);
            Assert.AreEqual(ItemStatus.Active, result.Status);
        }

        [TestMethod]
        public async Task UpdateItemAsync_SetsUpdatedByAndUpdatedAt()
        {
            var item = await SeedItemAsync();
            var before = DateTimeOffset.UtcNow;

            var result = await _service.UpdateItemAsync(item.Id, new ItemUpdateRequest
            {
                Name = "Updated"
            }, 42);

            Assert.IsNotNull(result);
            Assert.AreEqual(42, result.UpdatedBy);
            Assert.IsTrue(result.UpdatedAt >= before);
        }

        [TestMethod]
        public async Task UpdateItemAsync_PersistsChangesToDatabase()
        {
            var item = await SeedItemAsync(name: "Before");

            await _service.UpdateItemAsync(item.Id, new ItemUpdateRequest
            {
                Name = "After"
            }, 1);

            var persisted = await _context.Items.FindAsync(item.Id);
            Assert.AreEqual("After", persisted!.Name);
        }

        [TestMethod]
        public async Task DeleteItemAsync_ExistingItem_ReturnsTrue()
        {
            var item = await SeedItemAsync();

            var result = await _service.DeleteItemAsync(item.Id);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task DeleteItemAsync_NonExistentId_ReturnsFalse()
        {
            var result = await _service.DeleteItemAsync(999);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task DeleteItemAsync_ExistingItem_RemovesFromDatabase()
        {
            var item = await SeedItemAsync();

            await _service.DeleteItemAsync(item.Id);

            var persisted = await _context.Items.FindAsync(item.Id);
            Assert.IsNull(persisted);
        }

        [TestMethod]
        public async Task DeleteItemAsync_OnlyDeletesTargetItem()
        {
            var item1 = await SeedItemAsync(sku: "SKU-001");
            var item2 = await SeedItemAsync(sku: "SKU-002");

            await _service.DeleteItemAsync(item1.Id);

            var remaining = await _context.Items.FindAsync(item2.Id);
            Assert.IsNotNull(remaining);
        }

        private async Task<Item> SeedItemAsync(
            string sku = "SKU-001",
            string name = "Test Item",
            string? description = null,
            string? manufacturer = null,
            string? barcode = null,
            ItemStatus status = ItemStatus.Active)
        {
            var item = BuildItem(sku, name, description, manufacturer, barcode, status);

            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            return item;
        }

        private static Item BuildItem(

            string sku = "SKU-001",

            string name = "Test Item",

            string? description = null,

            string? manufacturer = null,

            string? barcode = null,

            ItemStatus status = ItemStatus.Active) => new()
            {
                Sku = sku,
                Name = name,
                Description = description,
                Manufacturer = manufacturer,
                BarcodePrimary = barcode,
                Status = status,
                CreatedBy = 1,
                UpdatedBy = 1
            };
    }
}