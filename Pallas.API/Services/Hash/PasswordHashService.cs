using System.Security.Cryptography;

namespace Pallas.API.Services.Hash
{
    public class PasswordHashService : IPasswordHashService
    {
        public string Hash(string password)
        {
            const int iterations = 100_000;

            byte[] salt = RandomNumberGenerator.GetBytes(0x10);

            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256
            );

            byte[] key = pbkdf2.GetBytes(0x20);

            return $"{iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
        }

        public bool Verify(string password, string store)
        {
            try
            {
                var parts = store.Split('.', 3);

                if (parts.Length != 3)
                    return false;

                var iterations = int.Parse(parts[0]);
                var salt = Convert.FromBase64String(parts[1]);
                var stored = Convert.FromBase64String(parts[2]);

                using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
                var computed = pbkdf2.GetBytes(stored.Length);

                return CryptographicOperations.FixedTimeEquals(stored, computed);
            }
            catch
            {
                return false;
            }
        }
    }
}