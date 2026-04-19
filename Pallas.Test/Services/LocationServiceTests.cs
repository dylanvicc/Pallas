using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Pallas.API;
using Pallas.API.Models.Locations;
using Pallas.API.Services.Locations;

namespace Pallas.Test.Services
{
    [TestClass]
    public class LocationServiceTests
    {
        private LocationService _service = null!;
        private ApplicationDatabaseContext _context = null!;
        private Mock<ILogger<LocationService>> _mockLogger = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDatabaseContext>()
                .UseInMemoryDatabase(databaseName: $"test_db_{Guid.NewGuid()}")
                .Options;

            _context = new ApplicationDatabaseContext(options);
            _mockLogger = new Mock<ILogger<LocationService>>();
            _service = new LocationService(_context, _mockLogger.Object);
        }

        [TestCleanup]
        public void Cleanup() => _context?.Dispose();

        [TestMethod]
        public async Task GetLocationByIdAsync_ExistingId_ReturnsLocation()
        {
            var location = await SeedLocationAsync();

            var result = await _service.GetLocationByIdAsync(location.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(location.Id, result.Id);
            Assert.AreEqual(location.Name, result.Name);
        }

        [TestMethod]
        public async Task GetLocationByIdAsync_NonExistentId_ReturnsNull()
        {
            var result = await _service.GetLocationByIdAsync(999);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetLocationsAsync_NoFilters_ReturnsAllLocations()
        {
            await SeedLocationAsync(name: "Warehouse A");
            await SeedLocationAsync(name: "Warehouse B");
            await SeedLocationAsync(name: "Warehouse C");

            var result = await _service.GetLocationsAsync(new LocationQuery());

            Assert.AreEqual(3, result.TotalCount);
            Assert.AreEqual(3, result.Data.Count());
        }

        [TestMethod]
        public async Task GetLocationsAsync_EmptyDatabase_ReturnsEmpty()
        {
            var result = await _service.GetLocationsAsync(new LocationQuery());

            Assert.AreEqual(0, result.TotalCount);
            Assert.AreEqual(0, result.Data.Count());
        }

        [TestMethod]
        public async Task GetLocationsAsync_SearchByName_ReturnsMatchingLocations()
        {
            await SeedLocationAsync(name: "Warehouse A");
            await SeedLocationAsync(name: "Warehouse B");
            await SeedLocationAsync(name: "Cold Storage");

            var result = await _service.GetLocationsAsync(new LocationQuery { Search = "Warehouse" });

            Assert.AreEqual(2, result.TotalCount);
            Assert.IsTrue(result.Data.All(l => l.Name.Contains("Warehouse")));
        }

        [TestMethod]
        public async Task GetLocationsAsync_FilterByActive_ReturnsMatchingLocations()
        {
            await SeedLocationAsync(name: "Warehouse A", active: true);
            await SeedLocationAsync(name: "Warehouse B", active: true);
            await SeedLocationAsync(name: "Warehouse C", active: false);

            var result = await _service.GetLocationsAsync(new LocationQuery { Active = true });

            Assert.AreEqual(2, result.TotalCount);
            Assert.IsTrue(result.Data.All(l => l.Active));
        }

        [TestMethod]
        public async Task GetLocationsAsync_FilterByInactive_ReturnsMatchingLocations()
        {
            await SeedLocationAsync(name: "Warehouse A", active: true);
            await SeedLocationAsync(name: "Warehouse B", active: false);

            var result = await _service.GetLocationsAsync(new LocationQuery { Active = false });

            Assert.AreEqual(1, result.TotalCount);
            Assert.IsFalse(result.Data.First().Active);
        }

        [TestMethod]
        public async Task GetLocationsAsync_Pagination_ReturnsCorrectPage()
        {
            for (var i = 1; i <= 10; i++)
                await SeedLocationAsync(name: $"Warehouse {i}");

            var result = await _service.GetLocationsAsync(new LocationQuery { Page = 2, PageSize = 3 });

            Assert.AreEqual(10, result.TotalCount);
            Assert.AreEqual(3, result.Data.Count());
            Assert.AreEqual(2, result.Page);
            Assert.AreEqual(3, result.PageSize);
        }

        [TestMethod]
        public async Task GetLocationsAsync_LastPage_ReturnsRemainingLocations()
        {
            for (var i = 1; i <= 7; i++)
                await SeedLocationAsync(name: $"Warehouse {i}");

            var result = await _service.GetLocationsAsync(new LocationQuery { Page = 2, PageSize = 5 });

            Assert.AreEqual(7, result.TotalCount);
            Assert.AreEqual(2, result.Data.Count());
        }

        [TestMethod]
        public async Task CreateLocationAsync_ValidRequest_ReturnsCreatedLocation()
        {
            var request = BuildCreateRequest();

            var result = await _service.CreateLocationAsync(request, 1);

            Assert.IsNotNull(result);
            Assert.AreNotEqual(0, result.Id);
            Assert.AreEqual(request.Name, result.Name);
        }

        [TestMethod]
        public async Task CreateLocationAsync_ValidRequest_PersistsToDatabase()
        {
            var request = BuildCreateRequest(name: "Persisted Location");

            var result = await _service.CreateLocationAsync(request, 1);

            var persisted = await _context.Locations.FindAsync(result.Id);
            Assert.IsNotNull(persisted);
            Assert.AreEqual("Persisted Location", persisted.Name);
        }

        [TestMethod]
        public async Task CreateLocationAsync_SetsCreatedByAndUpdatedBy()
        {
            var request = BuildCreateRequest();

            var result = await _service.CreateLocationAsync(request, 42);

            Assert.AreEqual(42, result.CreatedBy);
            Assert.AreEqual(42, result.UpdatedBy);
        }

        [TestMethod]
        public async Task CreateLocationAsync_SetsAllFields()
        {
            var request = BuildCreateRequest(
                name: "Full Location",
                description: "A description",
                active: false
            );

            var result = await _service.CreateLocationAsync(request, 1);

            Assert.AreEqual("Full Location", result.Name);
            Assert.AreEqual("A description", result.Description);
            Assert.IsFalse(result.Active);
        }

        [TestMethod]
        public async Task UpdateLocationAsync_ExistingLocation_ReturnsUpdatedLocation()
        {
            var location = await SeedLocationAsync(name: "Original");

            var result = await _service.UpdateLocationAsync(location.Id, new LocationUpdateRequest
            {
                Name = "Updated"
            }, 1);

            Assert.IsNotNull(result);
            Assert.AreEqual("Updated", result.Name);
        }

        [TestMethod]
        public async Task UpdateLocationAsync_NonExistentId_ReturnsNull()
        {
            var result = await _service.UpdateLocationAsync(999, new LocationUpdateRequest
            {
                Name = "Updated"
            }, 1);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UpdateLocationAsync_NullFields_DoesNotOverwriteExistingValues()
        {
            var location = await SeedLocationAsync(name: "Original", description: "Original Description");

            var result = await _service.UpdateLocationAsync(location.Id, new LocationUpdateRequest
            {
                Active = false
            }, 1);

            Assert.IsNotNull(result);
            Assert.AreEqual("Original", result.Name);
            Assert.AreEqual("Original Description", result.Description);
            Assert.IsFalse(result.Active);
        }

        [TestMethod]
        public async Task UpdateLocationAsync_SetsUpdatedByAndUpdatedAt()
        {
            var location = await SeedLocationAsync();
            var before = DateTimeOffset.UtcNow;

            var result = await _service.UpdateLocationAsync(location.Id, new LocationUpdateRequest
            {
                Name = "Updated"
            }, 42);

            Assert.IsNotNull(result);
            Assert.AreEqual(42, result.UpdatedBy);
            Assert.IsTrue(result.UpdatedAt >= before);
        }

        [TestMethod]
        public async Task UpdateLocationAsync_PersistsChangesToDatabase()
        {
            var location = await SeedLocationAsync(name: "Before");

            await _service.UpdateLocationAsync(location.Id, new LocationUpdateRequest
            {
                Name = "After"
            }, 1);

            var persisted = await _context.Locations.FindAsync(location.Id);
            Assert.AreEqual("After", persisted!.Name);
        }

        [TestMethod]
        public async Task UpdateLocationAsync_ActiveToggle_UpdatesCorrectly()
        {
            var location = await SeedLocationAsync(active: true);

            var result = await _service.UpdateLocationAsync(location.Id, new LocationUpdateRequest
            {
                Active = false
            }, 1);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Active);
        }

        [TestMethod]
        public async Task DeleteLocationAsync_ExistingLocation_ReturnsTrue()
        {
            var location = await SeedLocationAsync();

            var result = await _service.DeleteLocationAsync(location.Id);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task DeleteLocationAsync_NonExistentId_ReturnsFalse()
        {
            var result = await _service.DeleteLocationAsync(999);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task DeleteLocationAsync_ExistingLocation_RemovesFromDatabase()
        {
            var location = await SeedLocationAsync();

            await _service.DeleteLocationAsync(location.Id);

            var persisted = await _context.Locations.FindAsync(location.Id);
            Assert.IsNull(persisted);
        }

        [TestMethod]
        public async Task DeleteLocationAsync_OnlyDeletesTargetLocation()
        {
            var location1 = await SeedLocationAsync(name: "Warehouse A");
            var location2 = await SeedLocationAsync(name: "Warehouse B");

            await _service.DeleteLocationAsync(location1.Id);

            var remaining = await _context.Locations.FindAsync(location2.Id);
            Assert.IsNotNull(remaining);
        }

        private async Task<Location> SeedLocationAsync(
            string name = "Test Location",
            string? description = null,
            bool active = true)
        {
            var location = new Location
            {
                Name = name,
                Description = description,
                Active = active,
                CreatedBy = 1,
                UpdatedBy = 1
            };

            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            
            return location;
        }

        private static LocationCreateRequest BuildCreateRequest(
            string name = "Test Location",
            string? description = null,
            bool active = true) => new()
            {
                Name = name,
                Description = description,
                Active = active
            };
    }
}