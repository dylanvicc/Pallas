using Microsoft.EntityFrameworkCore;
using Moq;
using Pallas.API.Models.Users;
using Pallas.API;
using Microsoft.Extensions.Logging;
using Pallas.API.Services.Hash;
using Pallas.API.Services.Users;

namespace Pallas.Test.Services
{
    [TestClass]
    public class UserServiceTests
    {
        private UserService _service = null!;
        private ApplicationDatabaseContext _context = null!;

        private Mock<IPasswordHashService> _mockHasher = null!;
        private Mock<ILogger<UserService>> _mockLogger = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDatabaseContext>()
                .UseInMemoryDatabase(databaseName: $"test_db_{Guid.NewGuid()}")
                .Options;

            _context = new ApplicationDatabaseContext(options);
            _mockHasher = new Mock<IPasswordHashService>();
            _mockLogger = new Mock<ILogger<UserService>>();

            _service = new UserService(_context, _mockHasher.Object, _mockLogger.Object);
        }

        [TestCleanup]
        public void Cleanup() => _context?.Dispose();

        [TestMethod]
        public async Task CreateUserAsync_LogsSuccessfulCreation()
        {
            var user = new User
            {
                Username = "new_user",
                Email = "test@example.com",
                Password = "plain_password",
                CreatedAt = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow
            };

            _mockHasher.Setup(service => service.Hash(user.Password)).Returns("hashed_password");

            await _service.CreateUserAsync(new UserCreateRequest
            {
                Username = user.Username,
                Email = user.Email,
                Password = user.Password
            });

            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Created new user for")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task GetUserAsync_ReturnsUser()
        {
            var user = new User
            {
                Username = "test_user",
                Email = "test@example.com",
                Password = "password",
                CreatedAt = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = await _service.GetUserAsync(user.Id);

            Assert.IsNotNull(result, "Should return the user");
            Assert.AreEqual(user.Username, result.Username, "Username should match");
            Assert.AreEqual(user.Email, result.Email, "Email should match");
        }

        [TestMethod]
        public async Task GetUserByEmailAsync_ReturnsUser()
        {
            var user = new User
            {
                Username = "test_user",
                Email = "test@example.com",
                Password = "password",
                CreatedAt = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = await _service.GetUserByEmailAsync(user.Email);

            Assert.IsNotNull(result, "Should return the user");
            Assert.AreEqual(user.Email, result.Email, "Email should match");
        }

        [TestMethod]
        public async Task GetUserByUsernameAsync_ReturnsUser()
        {
            var user = new User
            {
                Username = "test_user",
                Email = "test@example.com",
                Password = "password",
                CreatedAt = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = await _service.GetUserByUsernameAsync(user.Username);

            Assert.IsNotNull(result, "Should return the user");
            Assert.AreEqual(user.Username, result.Username, "Username should match");
        }

        [TestMethod]
        public async Task UpdateUserAsync_OnlyHashesNewPassword()
        {
            var user = new User
            {
                Username = "test_user",
                Email = "test@example.com",
                Password = "original_hashed_password",
                CreatedAt = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var request = new UserUpdateRequest
            {
                Username = "updated_user",
                Password = "new_password"
            };

            var hash = string.Empty;
            _mockHasher.Setup(service => service.Hash("new_password")).Returns(hash);

            var result = await _service.UpdateUserAsync(user.Id, request);

            Assert.IsNotNull(result, "User should be updated");
            Assert.AreEqual("updated_user", result.Username, "Username should be updated");
            Assert.AreEqual(hash, result.Password, "Password should be updated");

            _mockHasher.Verify(service => service.Hash("new_password"), Times.Once, "New password should be hashed");
        }
    }
}