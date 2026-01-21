namespace CardMerchantSystem.API.Auth.Entities;

public class UserRoleEntity
{
    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;

    public int RoleId { get; set; }
    public RoleEntity Role { get; set; } = null!;

    public DateTime AssignedAt { get; set; }
    public string? AssignedBy { get; set; }
}