namespace CardMerchantSystem.API.Auth.Models;

public class LoginResponse
{
    public string Token { get; set; } = null!;
    public string? RefreshToken { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
    public DateTime ExpiresAt { get; set; }
}