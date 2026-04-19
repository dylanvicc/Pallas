using Pallas.API.Models.Users;

namespace Pallas.API.Services.Users
{
    public interface IUserService
    {
        Task<User?> GetUserAsync(long id);

        Task<User?> GetUserByEmailAsync(string email);

        Task<User?> GetUserByUsernameAsync(string username);

        Task<User> CreateUserAsync(UserCreateRequest request);

        Task<User?> UpdateUserAsync(long id, UserUpdateRequest request);
    }
}