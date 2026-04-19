using Pallas.API.Services.Hash;

namespace Pallas.Test.Services
{
    [TestClass]
    public class PasswordHashServiceTests
    {
        private PasswordHashService _service = null!;

        [TestInitialize]
        public void Setup() => _service = new PasswordHashService();


        [TestMethod]
        public void Hash_ReturnsValidFormat()
        {
            var chunks = _service.Hash("test").Split('.');

            Assert.AreEqual(3, chunks.Length, "Hash should have three seperated chunks");
            Assert.IsTrue(int.TryParse(chunks[0], out _), "First part should be iteration count");
        }

        [TestMethod]
        public void Hash_DifferentInvocations_ProduceDifferentHashes()
        {
            Assert.AreNotEqual(_service.Hash("test"), _service.Hash("test"), "Different hash invocations should produce different hashes");
        }

        [TestMethod]
        public void Verify_CorrectPassword_ReturnsTrue()
        {
            var result = _service.Verify("correct", _service.Hash("correct"));

            Assert.IsTrue(result, "Should return true for correct password");
        }

        [TestMethod]
        public void Verify_IncorrectPassword_ReturnsFalse()
        {
            var result = _service.Verify("wrong", _service.Hash("correct"));

            Assert.IsFalse(result, "Should return false for incorrect password");
        }

        [TestMethod]
        public void Verify_EmptyPassword_ReturnsFalse()
        {
            var result = _service.Verify(string.Empty, _service.Hash("test"));

            Assert.IsFalse(result, "Should return false for empty password");
        }

        [TestMethod]
        public void Verify_InvalidHashFormat_ReturnsFalse()
        {
            var result = _service.Verify("test", "invalid.hash.format.extra");

            Assert.IsFalse(result, "Should return false for invalid hash format");
        }
    }
}