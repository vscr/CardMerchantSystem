namespace CardMerchantSystem.API.Auth.Models;

public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string? Description { get; set; }
    public int UserCount { get; set; }
}

public class RoleDetailDto : RoleDto
{
    public List<RoleUserDto> Users { get; set; } = new();
}

public class RoleUserDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public DateTime AssignedAt { get; set; }
    public string? AssignedBy { get; set; }
}

public class CreateRoleDto
{
    public string Name { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string? Description { get; set; }
}

public class UpdateRoleDto
{
    public string DisplayName { get; set; } = null!;
    public string? Description { get; set; }
}