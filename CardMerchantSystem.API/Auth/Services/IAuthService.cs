using CardMerchantSystem.API.Auth.Models;

namespace CardMerchantSystem.API.Auth.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<bool> CreateUserAsync(string username, string email, string password, string fullName, List<string> roles);
}