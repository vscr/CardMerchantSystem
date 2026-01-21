namespace CardMerchantSystem.API.Auth.Entities;

public class UserEntity
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    // Navigation
    public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();

    // Helper
    public IEnumerable<string> GetRoleNames() => UserRoles.Select(ur => ur.Role.Name);
}