using CardMerchantSystem.API.Auth.Models;

namespace CardMerchantSystem.API.Auth.Services;

public interface IJwtService
{
    string GenerateToken(User user);
}