using Microsoft.EntityFrameworkCore;
using Pallas.API.Models.Users;
using Pallas.API.Services.Hash;

namespace Pallas.API.Services.Users
{
    public class UserService(
        ApplicationDatabaseContext context,
        IPasswordHashService hasher,
        ILogger<UserService> logger) : IUserService
    {
        public async Task<User?> GetUserAsync(long id) =>
            await context.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Id == id);

        public async Task<User?> GetUserByEmailAsync(string email) =>
            await context.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Email == email);

        public async Task<User?> GetUserByUsernameAsync(string username) =>
            await context.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Username == username);

        public async Task<User> CreateUserAsync(UserCreateRequest request)
        {
            try
            {
                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    Password = hasher.Hash(request.Password)
                };

                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                logger.LogInformation("Created new user for {Email}.", user.Email);

                return user;
            }
            catch (DbUpdateException exception)
            {
                logger.LogError(exception, "Database update failed while creating user for {Email}.", request.Email);
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unexpected error while creating user for {Email}.", request.Email);
                throw;
            }
        }

        public async Task<User?> UpdateUserAsync(long id, UserUpdateRequest request)
        {
            try
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                    return null;

                if (!string.IsNullOrWhiteSpace(request.Username)) user.Username = request.Username;
                if (!string.IsNullOrWhiteSpace(request.Email)) user.Email = request.Email;
                if (!string.IsNullOrWhiteSpace(request.Password)) user.Password = hasher.Hash(request.Password);

                await context.SaveChangesAsync();

                logger.LogInformation("Updated user {Id}.", id);

                return user;
            }
            catch (DbUpdateException exception)
            {
                logger.LogError(exception, "Database update failed while updating user {Id}.", id);
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unexpected error while updating user {Id}.", id);
                throw;
            }
        }
    }
}