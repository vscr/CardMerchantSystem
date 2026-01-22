namespace CardMerchantSystem.API.Auth.Models;

public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public List<string> Roles { get; set; } = new();
}

public class UserDetailDto : UserDto
{
    public List<UserRoleDto> RoleDetails { get; set; } = new();
}

public class UserRoleDto
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = null!;
    public string RoleDisplayName { get; set; } = null!;
    public DateTime AssignedAt { get; set; }
    public string? AssignedBy { get; set; }
}

public class CreateUserDto
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
}

public class UpdateUserDto
{
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public bool IsActive { get; set; }
}

public class ChangePasswordDto
{
    public string NewPassword { get; set; } = null!;
}

public class UpdateUserRolesDto
{
    public List<string> Roles { get; set; } = new();
}