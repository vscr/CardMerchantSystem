namespace CardMerchantSystem.API.Auth.Models;

/// <summary>
/// Login/Token işlemleri için kullanılan model
/// </summary>
public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public bool IsActive { get; set; }
    public List<string> Roles { get; set; } = new();
}